using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal partial class PageTwo : PageBase
    {
        private readonly EventManager _eventManager;

        // Search and Filter Controls
        private readonly TextBox txtSearch;
        private readonly ComboBox cmbCategoryFilter;
        private readonly DateTimePicker dtpStartDate;
        private readonly DateTimePicker dtpEndDate;
        private readonly CheckBox chkDateFilter;
        private readonly ComboBox cmbPriorityFilter;
        private readonly Button btnSearch;
        private readonly Button btnClearFilters;
        private readonly Button btnShowRecent;
        private readonly Button btnShowPriority;
        private readonly Button btnShowStats;
        private readonly Button btnBack;

        // Display Controls
        private readonly ListBox lstEvents;
        private readonly RichTextBox rtbEventDetails;
        private readonly Label lblResultCount;

        public PageTwo() : base("Local Events and Announcements")
        {
            _eventManager = new EventManager();

            // Initialize controls
            txtSearch = new TextBox { Width = 250 };
            cmbCategoryFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
            dtpStartDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 120 };
            dtpEndDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 120 };
            chkDateFilter = new CheckBox { Text = "Filter by Date Range", AutoSize = true };
            cmbPriorityFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            btnSearch = new Button { Text = "🔍 Search", AutoSize = true };
            btnClearFilters = new Button { Text = "Clear Filters", AutoSize = true };
            btnShowRecent = new Button { Text = "📋 Recent Events", AutoSize = true };
            btnShowPriority = new Button { Text = "⚡ High Priority", AutoSize = true };
            btnShowStats = new Button { Text = "📊 Statistics", AutoSize = true };
            btnBack = new Button { Text = "← Back", AutoSize = true };

            lstEvents = new ListBox { Font = new Font(FontFamily.GenericMonospace, 9) };
            rtbEventDetails = new RichTextBox { ReadOnly = true, Font = new Font(FontFamily.GenericSansSerif, 10) };
            lblResultCount = new Label { Text = "Loading events...", AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Italic) };

            LoadSampleData();
            BuildUI();
            WireEvents();
            RefreshEventList(_eventManager.GetAllEvents());

            BackRequested += () => { };
        }

        private void BuildUI()
        {
            Dock = DockStyle.Fill;

            // Main container
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Search section
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Content section
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Bottom buttons

            // === SEARCH AND FILTER SECTION ===
            var searchPanel = new GroupBox
            {
                Text = "Search and Filter Events",
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10)
            };

            var searchLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true
            };

            // Row 0: Keyword Search
            var lblSearch = new Label { Text = "Keyword:", AutoSize = true, Margin = new Padding(3, 5, 3, 3) };
            var searchRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Dock = DockStyle.Fill };
            searchRow.Controls.Add(txtSearch);
            searchRow.Controls.Add(btnSearch);
            searchLayout.Controls.Add(lblSearch, 0, 0);
            searchLayout.Controls.Add(searchRow, 1, 0);

            // Row 1: Category and Priority Filters
            var lblCategory = new Label { Text = "Category:", AutoSize = true, Margin = new Padding(3, 5, 3, 3) };
            var filterRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Dock = DockStyle.Fill };
            filterRow.Controls.Add(cmbCategoryFilter);
            filterRow.Controls.Add(new Label { Text = "Priority:", AutoSize = true, Margin = new Padding(10, 5, 3, 3) });
            filterRow.Controls.Add(cmbPriorityFilter);
            searchLayout.Controls.Add(lblCategory, 0, 1);
            searchLayout.Controls.Add(filterRow, 1, 1);

            // Row 2: Date Range
            var dateRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Dock = DockStyle.Fill };
            dateRow.Controls.Add(chkDateFilter);
            dateRow.Controls.Add(new Label { Text = "From:", AutoSize = true, Margin = new Padding(10, 5, 3, 3) });
            dateRow.Controls.Add(dtpStartDate);
            dateRow.Controls.Add(new Label { Text = "To:", AutoSize = true, Margin = new Padding(10, 5, 3, 3) });
            dateRow.Controls.Add(dtpEndDate);
            searchLayout.Controls.Add(new Label { Text = "", AutoSize = true }, 0, 2);
            searchLayout.Controls.Add(dateRow, 1, 2);

            // Row 3: Action Buttons
            var actionRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Dock = DockStyle.Fill };
            actionRow.Controls.Add(btnClearFilters);
            actionRow.Controls.Add(btnShowRecent);
            actionRow.Controls.Add(btnShowPriority);
            actionRow.Controls.Add(btnShowStats);
            searchLayout.Controls.Add(new Label { Text = "", AutoSize = true }, 0, 3);
            searchLayout.Controls.Add(actionRow, 1, 3);

            searchPanel.Controls.Add(searchLayout);
            mainPanel.Controls.Add(searchPanel, 0, 0);

            // === CONTENT SECTION ===
            var contentPanel = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 250,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Top: Events List
            var listPanel = new Panel { Dock = DockStyle.Fill };
            var listHeader = new Label
            {
                Text = "📅 Upcoming Events",
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold),
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            lblResultCount.Dock = DockStyle.Top;
            lblResultCount.Padding = new Padding(5, 0, 5, 5);
            lstEvents.Dock = DockStyle.Fill;
            listPanel.Controls.Add(lstEvents);
            listPanel.Controls.Add(lblResultCount);
            listPanel.Controls.Add(listHeader);
            contentPanel.Panel1.Controls.Add(listPanel);

            // Bottom: Event Details
            var detailsPanel = new Panel { Dock = DockStyle.Fill };
            var detailsHeader = new Label
            {
                Text = "📝 Event Details",
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold),
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            rtbEventDetails.Dock = DockStyle.Fill;
            detailsPanel.Controls.Add(rtbEventDetails);
            detailsPanel.Controls.Add(detailsHeader);
            contentPanel.Panel2.Controls.Add(detailsPanel);

            mainPanel.Controls.Add(contentPanel, 0, 1);

            // === BOTTOM SECTION ===
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(5)
            };
            bottomPanel.Controls.Add(btnBack);
            mainPanel.Controls.Add(bottomPanel, 0, 2);

            Controls.Add(mainPanel);

            // Populate filters
            PopulateFilters();
        }

        private void PopulateFilters()
        {
            // Category filter
            cmbCategoryFilter.Items.Add("All Categories");
            cmbCategoryFilter.Items.AddRange(_eventManager.GetAllCategories().ToArray());
            cmbCategoryFilter.SelectedIndex = 0;

            // Priority filter
            cmbPriorityFilter.Items.Add("All Priorities");
            foreach (Priority p in Enum.GetValues(typeof(Priority)))
            {
                cmbPriorityFilter.Items.Add(p.ToString());
            }
            cmbPriorityFilter.SelectedIndex = 0;

            // Date range defaults
            dtpStartDate.Value = DateTime.Now.Date;
            dtpEndDate.Value = DateTime.Now.Date.AddMonths(3);
            dtpStartDate.Enabled = false;
            dtpEndDate.Enabled = false;
        }

        private void WireEvents()
        {
            btnSearch.Click += (s, e) => PerformSearch();
            btnClearFilters.Click += (s, e) => ClearFilters();
            btnShowRecent.Click += (s, e) => ShowRecentEvents();
            btnShowPriority.Click += (s, e) => ShowHighPriorityEvents();
            btnShowStats.Click += (s, e) => ShowStatistics();
            btnBack.Click += (s, e) => OnBackRequested();

            txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    PerformSearch();
                    e.Handled = true;
                }
            };

            chkDateFilter.CheckedChanged += (s, e) =>
            {
                dtpStartDate.Enabled = chkDateFilter.Checked;
                dtpEndDate.Enabled = chkDateFilter.Checked;
            };

            lstEvents.SelectedIndexChanged += (s, e) =>
            {
                if (lstEvents.SelectedItem is Event selectedEvent)
                {
                    ShowEventDetails(selectedEvent);
                    _eventManager.RecordEventView(selectedEvent); // Stack usage
                }
            };

            lstEvents.DoubleClick += (s, e) =>
            {
                if (lstEvents.SelectedItem is Event selectedEvent)
                {
                    MessageBox.Show(selectedEvent.GetDisplayText(), selectedEvent.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
        }

        private void PerformSearch()
        {
            string category = cmbCategoryFilter.SelectedItem?.ToString();
            DateTime? startDate = chkDateFilter.Checked ? dtpStartDate.Value.Date : (DateTime?)null;
            DateTime? endDate = chkDateFilter.Checked ? dtpEndDate.Value.Date : (DateTime?)null;
            string keyword = txtSearch.Text.Trim();
            Priority? minPriority = cmbPriorityFilter.SelectedIndex > 0
                ? (Priority?)Enum.Parse(typeof(Priority), cmbPriorityFilter.SelectedItem.ToString())
                : null;

            var results = _eventManager.AdvancedSearch(category, startDate, endDate, keyword, minPriority);
            RefreshEventList(results);
        }

        private void ClearFilters()
        {
            txtSearch.Clear();
            cmbCategoryFilter.SelectedIndex = 0;
            cmbPriorityFilter.SelectedIndex = 0;
            chkDateFilter.Checked = false;
            RefreshEventList(_eventManager.GetAllEvents());
        }

        private void ShowRecentEvents()
        {
            var recentEvents = _eventManager.GetRecentlyAddedEvents(); // Queue usage
            RefreshEventList(recentEvents);
            rtbEventDetails.Text = "📋 Showing Recently Added Events (Queue - FIFO)\n\n" +
                                  $"This demonstrates the Queue data structure.\n" +
                                  $"Events are displayed in the order they were added.\n\n" +
                                  $"Total recent events: {recentEvents.Count}";
        }

        private void ShowHighPriorityEvents()
        {
            var priorityEvents = _eventManager.GetHighPriorityEvents(); // Priority Queue usage
            RefreshEventList(priorityEvents);
            rtbEventDetails.Text = "⚡ Showing High Priority Events (Priority Queue)\n\n" +
                                  $"This demonstrates the Priority Queue data structure.\n" +
                                  $"Critical and High priority events are shown first.\n\n" +
                                  $"Total high priority events: {priorityEvents.Count}";
        }

        private void ShowStatistics()
        {
            rtbEventDetails.Text = _eventManager.GetStatistics();
            MessageBox.Show(_eventManager.GetStatistics(), "Event Manager Statistics",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshEventList(System.Collections.Generic.List<Event> events)
        {
            lstEvents.BeginUpdate();
            lstEvents.Items.Clear();

            foreach (var ev in events)
            {
                lstEvents.Items.Add(ev);
            }

            lstEvents.EndUpdate();

            lblResultCount.Text = $"📊 Showing {events.Count} event(s)";

            if (events.Count > 0)
            {
                rtbEventDetails.Text = "Select an event to view details.\nDouble-click an event for a popup view.";
            }
            else
            {
                rtbEventDetails.Text = "No events found matching your search criteria.\nTry adjusting your filters.";
            }
        }

        private void ShowEventDetails(Event ev)
        {
            rtbEventDetails.Clear();
            rtbEventDetails.SelectionFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
            rtbEventDetails.AppendText($"{ev.Title}\n\n");

            rtbEventDetails.SelectionFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
            rtbEventDetails.AppendText(ev.GetDisplayText());

            rtbEventDetails.AppendText("\n" + new string('─', 50) + "\n");
            rtbEventDetails.AppendText($"Event ID: {ev.Id}\n");
            rtbEventDetails.AppendText("\nℹ️ This event was retrieved using optimized data structures:\n");
            rtbEventDetails.AppendText("• SortedDictionary for date-based organization\n");
            rtbEventDetails.AppendText("• Dictionary for category-based lookup\n");
            rtbEventDetails.AppendText("• Recorded in Stack for viewing history");
        }

        private void LoadSampleData()
        {
            // Add diverse sample events to demonstrate all data structures
            var today = DateTime.Now.Date;

            _eventManager.AddEvent(
                "Community Town Hall Meeting",
                "Join us for our monthly town hall meeting to discuss community improvements and upcoming projects. All residents are welcome.",
                today.AddDays(7).AddHours(18),
                "Community",
                "City Hall - Main Auditorium",
                Priority.High,
                "City Council"
            );

            _eventManager.AddEvent(
                "Annual Summer Festival",
                "Celebrate summer with live music, food vendors, and family activities. Free admission for all ages.",
                today.AddDays(30).AddHours(10),
                "Recreation",
                "Central Park",
                Priority.Normal,
                "Parks & Recreation Department"
            );

            _eventManager.AddEvent(
                "Emergency Preparedness Workshop",
                "Learn essential emergency preparedness skills including first aid, disaster planning, and emergency communications.",
                today.AddDays(14).AddHours(14),
                "Safety",
                "Fire Station #1",
                Priority.Critical,
                "Emergency Services"
            );

            _eventManager.AddEvent(
                "Public Library Book Sale",
                "Annual book sale featuring thousands of gently used books at great prices. Support your local library!",
                today.AddDays(21).AddHours(9),
                "Culture",
                "Public Library",
                Priority.Low,
                "Library Foundation"
            );

            _eventManager.AddEvent(
                "Road Closure Notice - Main Street",
                "Main Street will be closed between 1st and 5th Avenue for infrastructure repairs. Please plan alternate routes.",
                today.AddDays(3).AddHours(6),
                "Infrastructure",
                "Main Street (1st-5th Ave)",
                Priority.Critical,
                "Public Works Department"
            );

            _eventManager.AddEvent(
                "Youth Sports Registration",
                "Sign up for fall youth sports programs including soccer, basketball, and baseball. Ages 5-17.",
                today.AddDays(10).AddHours(17),
                "Recreation",
                "Community Center",
                Priority.Normal,
                "Youth Sports League"
            );

            _eventManager.AddEvent(
                "Recycling Collection Schedule Change",
                "Recycling collection has moved to Fridays starting next month. Please adjust your schedule accordingly.",
                today.AddDays(5).AddHours(8),
                "Sanitation",
                "All Districts",
                Priority.High,
                "Sanitation Department"
            );

            _eventManager.AddEvent(
                "Small Business Workshop",
                "Free workshop for entrepreneurs and small business owners. Topics include marketing, financing, and growth strategies.",
                today.AddDays(12).AddHours(13),
                "Business",
                "Chamber of Commerce",
                Priority.Normal,
                "Economic Development Office"
            );

            _eventManager.AddEvent(
                "Senior Center Open House",
                "Tour our newly renovated senior center and learn about programs and activities for adults 55+.",
                today.AddDays(8).AddHours(11),
                "Community",
                "Senior Wellness Center",
                Priority.Normal,
                "Senior Services"
            );

            _eventManager.AddEvent(
                "Water Main Maintenance Notice",
                "Temporary water service interruption scheduled for maintenance. Affected areas will be notified by mail.",
                today.AddDays(2).AddHours(22),
                "Utilities",
                "Northside Residential Area",
                Priority.High,
                "Water Department"
            );

            _eventManager.AddEvent(
                "Art in the Park Exhibition",
                "Local artists showcase their work in an outdoor exhibition. Free and open to the public.",
                today.AddDays(25).AddHours(12),
                "Culture",
                "Riverside Park",
                Priority.Low,
                "Arts Council"
            );

            _eventManager.AddEvent(
                "Voter Registration Drive",
                "Register to vote or update your registration. Staff will be available to assist with forms and questions.",
                today.AddDays(18).AddHours(10),
                "Civic",
                "City Hall Plaza",
                Priority.High,
                "Election Commission"
            );

            _eventManager.AddEvent(
                "Farmers Market Opening Day",
                "Weekly farmers market begins! Fresh produce, baked goods, crafts, and more from local vendors.",
                today.AddDays(6).AddHours(7),
                "Community",
                "Market Square",
                Priority.Normal,
                "Local Farmers Association"
            );

            _eventManager.AddEvent(
                "Public Safety Forum",
                "Meet with police and fire department leadership to discuss public safety concerns and initiatives.",
                today.AddDays(15).AddHours(19),
                "Safety",
                "Community Center - Room 201",
                Priority.High,
                "Public Safety Department"
            );

            _eventManager.AddEvent(
                "Parks Cleanup Volunteer Day",
                "Join fellow community members in cleaning and beautifying our local parks. Supplies provided.",
                today.AddDays(20).AddHours(8),
                "Recreation",
                "Various Parks",
                Priority.Low,
                "Parks Department"
            );
        }
    }
}

