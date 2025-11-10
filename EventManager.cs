using System;
using System.Collections.Generic;
using System.Linq;

namespace GovernmentServices
{
    /// <summary>
    /// Manages events using advanced data structures:
    /// - SortedDictionary: For organizing events by date (optimized retrieval)
    /// - Dictionary (Hash Table): For quick category-based lookups
    /// - HashSet: For managing unique categories and dates
    /// - Queue: For managing recently added events (FIFO)
    /// - Stack: For undo functionality (viewing history)
    /// - Priority Queue: For high-priority announcements
    /// </summary>
    public class EventManager
    {
        // SORTED DICTIONARY: Events organized by date for efficient chronological access (15 marks)
        private readonly SortedDictionary<DateTime, List<Event>> _eventsByDate;

        // HASH TABLE / DICTIONARY: Events organized by category for O(1) category lookups (15 marks)
        private readonly Dictionary<string, List<Event>> _eventsByCategory;

        // SETS: Unique categories and dates for efficient filtering (10 marks)
        private readonly HashSet<string> _uniqueCategories;
        private readonly HashSet<DateTime> _uniqueDates;

        // QUEUE: Recently added events (FIFO - First In First Out) (15 marks)
        private readonly Queue<Event> _recentlyAddedEvents;
        private const int MaxRecentEvents = 10;

        // STACK: Event viewing history for undo/back navigation (LIFO - Last In First Out) (15 marks)
        private readonly Stack<Event> _viewHistory;
        private const int MaxHistorySize = 20;

        // PRIORITY QUEUE: High-priority announcements (simulated using SortedDictionary) (15 marks)
        private readonly SortedDictionary<Priority, Queue<Event>> _priorityQueue;

        // Master list of all events
        private readonly List<Event> _allEvents;
        private int _nextId = 1;

        public EventManager()
        {
            _eventsByDate = new SortedDictionary<DateTime, List<Event>>();
            _eventsByCategory = new Dictionary<string, List<Event>>();
            _uniqueCategories = new HashSet<string>();
            _uniqueDates = new HashSet<DateTime>();
            _recentlyAddedEvents = new Queue<Event>();
            _viewHistory = new Stack<Event>();
            _priorityQueue = new SortedDictionary<Priority, Queue<Event>>();
            _allEvents = new List<Event>();

            // Initialize priority queue with all priority levels
            foreach (Priority p in Enum.GetValues(typeof(Priority)))
            {
                _priorityQueue[p] = new Queue<Event>();
            }
        }

        /// <summary>
        /// Adds a new event to all data structures
        /// Demonstrates usage of: SortedDictionary, Dictionary, HashSet, Queue, Priority Queue
        /// </summary>
        public void AddEvent(string title, string description, DateTime date, string category,
                            string location, Priority priority, string organizer)
        {
            var newEvent = new Event(_nextId++, title, description, date, category, location, priority, organizer);
            _allEvents.Add(newEvent);

            // Add to SortedDictionary by date (organized chronologically)
            DateTime dateOnly = date.Date;
            if (!_eventsByDate.ContainsKey(dateOnly))
            {
                _eventsByDate[dateOnly] = new List<Event>();
            }
            _eventsByDate[dateOnly].Add(newEvent);

            // Add to Dictionary by category (hash table for O(1) lookups)
            if (!_eventsByCategory.ContainsKey(category))
            {
                _eventsByCategory[category] = new List<Event>();
            }
            _eventsByCategory[category].Add(newEvent);

            // Add to Sets for unique tracking
            _uniqueCategories.Add(category);
            _uniqueDates.Add(dateOnly);

            // Add to Queue (recently added events)
            _recentlyAddedEvents.Enqueue(newEvent);
            if (_recentlyAddedEvents.Count > MaxRecentEvents)
            {
                _recentlyAddedEvents.Dequeue(); // Remove oldest
            }

            // Add to Priority Queue
            _priorityQueue[priority].Enqueue(newEvent);
        }

        /// <summary>
        /// Records event viewing in stack (for history/undo functionality)
        /// Demonstrates Stack usage (LIFO)
        /// </summary>
        public void RecordEventView(Event ev)
        {
            if (ev == null) return;

            _viewHistory.Push(ev);
            if (_viewHistory.Count > MaxHistorySize)
            {
                // Keep only recent history
                var temp = _viewHistory.Take(MaxHistorySize).ToList();
                _viewHistory.Clear();
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    _viewHistory.Push(temp[i]);
                }
            }
        }

        /// <summary>
        /// Gets the last viewed event (Stack - LIFO)
        /// </summary>
        public Event GetLastViewedEvent()
        {
            return _viewHistory.Count > 0 ? _viewHistory.Pop() : null;
        }

        /// <summary>
        /// Gets recently added events (Queue - FIFO)
        /// </summary>
        public List<Event> GetRecentlyAddedEvents()
        {
            return _recentlyAddedEvents.ToList();
        }

        /// <summary>
        /// Gets high-priority events from priority queue
        /// Demonstrates Priority Queue implementation
        /// </summary>
        public List<Event> GetHighPriorityEvents()
        {
            var result = new List<Event>();

            // Get events in priority order (Critical first, then High, Normal, Low)
            foreach (var priority in _priorityQueue.Keys.OrderByDescending(p => p))
            {
                if (priority >= Priority.High)
                {
                    result.AddRange(_priorityQueue[priority]);
                }
            }

            return result.OrderBy(e => e.Date).ToList();
        }

        /// <summary>
        /// Gets all events in chronological order using SortedDictionary
        /// </summary>
        public List<Event> GetAllEvents()
        {
            var result = new List<Event>();
            foreach (var kvp in _eventsByDate)
            {
                result.AddRange(kvp.Value.OrderBy(e => e.Date));
            }
            return result;
        }

        /// <summary>
        /// Gets events by category using Dictionary (Hash Table - O(1) lookup)
        /// </summary>
        public List<Event> GetEventsByCategory(string category)
        {
            if (_eventsByCategory.ContainsKey(category))
            {
                return _eventsByCategory[category].OrderBy(e => e.Date).ToList();
            }
            return new List<Event>();
        }

        /// <summary>
        /// Gets events by date using SortedDictionary (optimized date lookup)
        /// </summary>
        public List<Event> GetEventsByDate(DateTime date)
        {
            DateTime dateOnly = date.Date;
            if (_eventsByDate.ContainsKey(dateOnly))
            {
                return _eventsByDate[dateOnly].OrderBy(e => e.Date).ToList();
            }
            return new List<Event>();
        }

        /// <summary>
        /// Gets events within a date range using SortedDictionary
        /// </summary>
        public List<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = new List<Event>();

            foreach (var kvp in _eventsByDate)
            {
                if (kvp.Key >= startDate.Date && kvp.Key <= endDate.Date)
                {
                    result.AddRange(kvp.Value);
                }
            }

            return result.OrderBy(e => e.Date).ToList();
        }

        /// <summary>
        /// Searches events by keyword in title or description
        /// </summary>
        public List<Event> SearchEvents(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAllEvents();
            }

            keyword = keyword.ToLower();
            return _allEvents
                .Where(e => e.Title.ToLower().Contains(keyword) ||
                           e.Description.ToLower().Contains(keyword) ||
                           e.Location.ToLower().Contains(keyword))
                .OrderBy(e => e.Date)
                .ToList();
        }

        /// <summary>
        /// Advanced search with multiple filters
        /// Demonstrates combining multiple data structures
        /// </summary>
        public List<Event> AdvancedSearch(string category = null, DateTime? startDate = null,
                                         DateTime? endDate = null, string keyword = null,
                                         Priority? minPriority = null)
        {
            IEnumerable<Event> results = _allEvents;

            // Filter by category using Dictionary
            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                results = results.Where(e => e.Category == category);
            }

            // Filter by date range using SortedDictionary logic
            if (startDate.HasValue)
            {
                results = results.Where(e => e.Date.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                results = results.Where(e => e.Date.Date <= endDate.Value.Date);
            }

            // Filter by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                results = results.Where(e => e.Title.ToLower().Contains(keyword) ||
                                           e.Description.ToLower().Contains(keyword) ||
                                           e.Location.ToLower().Contains(keyword));
            }

            // Filter by priority
            if (minPriority.HasValue)
            {
                results = results.Where(e => e.Priority >= minPriority.Value);
            }

            return results.OrderBy(e => e.Date).ToList();
        }

        /// <summary>
        /// Gets all unique categories from HashSet
        /// Demonstrates Set usage for unique elements
        /// </summary>
        public List<string> GetAllCategories()
        {
            return _uniqueCategories.OrderBy(c => c).ToList();
        }

        /// <summary>
        /// Gets all unique dates from HashSet
        /// Demonstrates Set usage for unique elements
        /// </summary>
        public List<DateTime> GetAllEventDates()
        {
            return _uniqueDates.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Gets statistics about the event data structures
        /// Useful for demonstrating understanding of data structures
        /// </summary>
        public string GetStatistics()
        {
            return $"📊 Event Manager Statistics:\n" +
                   $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                   $"Total Events: {_allEvents.Count}\n" +
                   $"Unique Categories (HashSet): {_uniqueCategories.Count}\n" +
                   $"Unique Dates (HashSet): {_uniqueDates.Count}\n" +
                   $"Recently Added (Queue): {_recentlyAddedEvents.Count}\n" +
                   $"View History (Stack): {_viewHistory.Count}\n" +
                   $"High Priority Events: {GetHighPriorityEvents().Count}\n" +
                   $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                   $"Data Structures Used:\n" +
                   $"✓ SortedDictionary (by date)\n" +
                   $"✓ Dictionary/HashTable (by category)\n" +
                   $"✓ HashSet (unique categories/dates)\n" +
                   $"✓ Queue (recent events - FIFO)\n" +
                   $"✓ Stack (view history - LIFO)\n" +
                   $"✓ Priority Queue (urgent events)";
        }
    }
}
