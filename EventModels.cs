using System;

namespace GovernmentServices
{
    /// <summary>
    /// Represents a local event or announcement with all relevant details
    /// </summary>
    public class Event : IComparable<Event>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public Priority Priority { get; set; }
        public string Organizer { get; set; }

        public Event(int id, string title, string description, DateTime date, string category,
                     string location, Priority priority, string organizer)
        {
            Id = id;
            Title = title;
            Description = description;
            Date = date;
            Category = category;
            Location = location;
            Priority = priority;
            Organizer = organizer;
        }

        // IComparable implementation for sorting by date
        public int CompareTo(Event other)
        {
            if (other == null) return 1;
            return this.Date.CompareTo(other.Date);
        }

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd} | {Category} | {Title}";
        }

        /// <summary>
        /// Returns a formatted display string for the event
        /// </summary>
        public string GetDisplayText()
        {
            return $"📅 {Date:dddd, MMMM dd, yyyy} at {Date:HH:mm}\n" +
                   $"📌 {Title}\n" +
                   $"🏷️ Category: {Category}\n" +
                   $"📍 Location: {Location}\n" +
                   $"👤 Organizer: {Organizer}\n" +
                   $"⚡ Priority: {Priority}\n" +
                   $"📝 {Description}\n";
        }
    }

    /// <summary>
    /// Priority levels for events (used in Priority Queue)
    /// </summary>
    public enum Priority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }
}
