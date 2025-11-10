using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace GovernmentServices
{
    internal partial class PageThree : PageBase
    {
        private ServiceRequestManager requestManager;
        private DBHandler dbHandler;
        private ListBox requestListBox;
        private TextBox searchBox;
        private ComboBox statusFilter;
        private ComboBox categoryFilter;
        private ComboBox priorityFilter;
        private RichTextBox detailsBox;
        private Button searchButton;
        private Button clearButton;
        private Button addTestDataButton;
        private Button refreshButton;
        private Button statsButton;
        private Button updateStatusButton;
        private Label statsLabel;

        public PageThree() : base("Service Request Status")
        {
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            // Initialize manager and database
            requestManager = new ServiceRequestManager();
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GovernmentServices",
                "government.db"
            );
            dbHandler = new DBHandler(dbPath);
            dbHandler.EnsureServiceRequestsTable();

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Search panel
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Stats

            // Search Panel
            Panel searchPanel = CreateSearchPanel();
            mainLayout.Controls.Add(searchPanel, 0, 0);

            // Content Panel with split container
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400
            };

            // Left side - Request List
            Panel listPanel = CreateListPanel();
            splitContainer.Panel1.Controls.Add(listPanel);

            // Right side - Details
            Panel detailsPanel = CreateDetailsPanel();
            splitContainer.Panel2.Controls.Add(detailsPanel);

            mainLayout.Controls.Add(splitContainer, 0, 1);

            // Stats label
            statsLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Loading statistics...",
                AutoSize = false
            };
            mainLayout.Controls.Add(statsLabel, 0, 2);

            Controls.Add(mainLayout);
            BackRequested += () => { };
        }

        private Panel CreateSearchPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 3,
                Padding = new Padding(5)
            };

            // Row 1: Search
            Label searchLabel = new Label { Text = "Search:", AutoSize = true, Anchor = AnchorStyles.Left };
            searchBox = new TextBox { Dock = DockStyle.Fill };
            searchButton = new Button { Text = "Search", Dock = DockStyle.Fill };
            clearButton = new Button { Text = "Clear", Dock = DockStyle.Fill };

            layout.Controls.Add(searchLabel, 0, 0);
            layout.Controls.Add(searchBox, 1, 0);
            layout.Controls.Add(searchButton, 2, 0);
            layout.Controls.Add(clearButton, 3, 0);

            // Row 2: Filters
            Label statusLabel = new Label { Text = "Status:", AutoSize = true, Anchor = AnchorStyles.Left };
            statusFilter = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            statusFilter.Items.Add("All Statuses");
            foreach (RequestStatus status in Enum.GetValues(typeof(RequestStatus)))
                statusFilter.Items.Add(status.ToString());
            statusFilter.SelectedIndex = 0;

            Label categoryLabel = new Label { Text = "Category:", AutoSize = true, Anchor = AnchorStyles.Left };
            categoryFilter = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            categoryFilter.Items.Add("All Categories");
            categoryFilter.SelectedIndex = 0;

            layout.Controls.Add(statusLabel, 0, 1);
            layout.Controls.Add(statusFilter, 1, 1);
            layout.Controls.Add(categoryLabel, 2, 1);
            layout.Controls.Add(categoryFilter, 3, 1);

            // Row 3: Priority filter and action buttons
            Label priorityLabel = new Label { Text = "Priority:", AutoSize = true, Anchor = AnchorStyles.Left };
            priorityFilter = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            priorityFilter.Items.Add("All Priorities");
            foreach (RequestPriority priority in Enum.GetValues(typeof(RequestPriority)))
                priorityFilter.Items.Add(priority.ToString());
            priorityFilter.SelectedIndex = 0;

            addTestDataButton = new Button { Text = "Add Test Data", Dock = DockStyle.Fill };
            refreshButton = new Button { Text = "Refresh", Dock = DockStyle.Fill };

            layout.Controls.Add(priorityLabel, 0, 2);
            layout.Controls.Add(priorityFilter, 1, 2);
            layout.Controls.Add(addTestDataButton, 2, 2);
            layout.Controls.Add(refreshButton, 3, 2);

            // Set column styles
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Event handlers
            searchButton.Click += (s, e) => ApplyFilters();
            clearButton.Click += (s, e) => ClearFilters();
            searchBox.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) ApplyFilters(); };
            statusFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            categoryFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            priorityFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            addTestDataButton.Click += (s, e) => AddTestData();
            refreshButton.Click += (s, e) => LoadData();

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateListPanel()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

            // Header
            Label header = new Label
            {
                Text = "Service Requests",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(header, 0, 0);

            // List box
            requestListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9)
            };
            requestListBox.SelectedIndexChanged += RequestListBox_SelectedIndexChanged;
            requestListBox.DoubleClick += RequestListBox_DoubleClick;
            layout.Controls.Add(requestListBox, 0, 1);

            // Action buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight
            };

            updateStatusButton = new Button { Text = "Update Status", Width = 120, Height = 30 };
            statsButton = new Button { Text = "View Statistics", Width = 120, Height = 30 };

            updateStatusButton.Click += (s, e) => UpdateRequestStatus();
            statsButton.Click += (s, e) => ShowStatistics();

            buttonPanel.Controls.Add(updateStatusButton);
            buttonPanel.Controls.Add(statsButton);

            layout.Controls.Add(buttonPanel, 0, 2);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateDetailsPanel()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Header
            Label header = new Label
            {
                Text = "Request Details",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(header, 0, 0);

            // Details box
            detailsBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                Text = "Select a request to view details..."
            };
            layout.Controls.Add(detailsBox, 0, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private void LoadData()
        {
            try
            {
                requestManager.Clear();

                // Load from database
                var requests = dbHandler.GetAllServiceRequests();

                foreach (var request in requests)
                {
                    requestManager.AddRequest(request);
                }

                // Update categories
                categoryFilter.Items.Clear();
                categoryFilter.Items.Add("All Categories");
                foreach (var category in requestManager.GetCategories())
                {
                    categoryFilter.Items.Add(category);
                }
                categoryFilter.SelectedIndex = 0;

                ApplyFilters();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                List<ServiceRequest> filteredRequests = requestManager.GetAllRequestsSorted();

                // Apply search
                if (!string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    filteredRequests = requestManager.SearchRequests(searchBox.Text);
                }

                // Apply status filter
                if (statusFilter.SelectedIndex > 0)
                {
                    RequestStatus selectedStatus = (RequestStatus)Enum.Parse(typeof(RequestStatus), statusFilter.SelectedItem.ToString());
                    filteredRequests = filteredRequests.Where(r => r.Status == selectedStatus).ToList();
                }

                // Apply category filter
                if (categoryFilter.SelectedIndex > 0)
                {
                    string selectedCategory = categoryFilter.SelectedItem.ToString();
                    filteredRequests = filteredRequests.Where(r => r.Category == selectedCategory).ToList();
                }

                // Apply priority filter
                if (priorityFilter.SelectedIndex > 0)
                {
                    RequestPriority selectedPriority = (RequestPriority)Enum.Parse(typeof(RequestPriority), priorityFilter.SelectedItem.ToString());
                    filteredRequests = filteredRequests.Where(r => r.Priority == selectedPriority).ToList();
                }

                // Update list
                requestListBox.Items.Clear();
                foreach (var request in filteredRequests)
                {
                    requestListBox.Items.Add(request.GetDisplayText());
                }

                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFilters()
        {
            searchBox.Clear();
            statusFilter.SelectedIndex = 0;
            categoryFilter.SelectedIndex = 0;
            priorityFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void RequestListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (requestListBox.SelectedIndex >= 0)
            {
                string displayText = requestListBox.SelectedItem.ToString();
                string requestId = ExtractRequestId(displayText);

                var request = requestManager.GetRequestById(requestId);
                if (request != null)
                {
                    detailsBox.Text = request.GetDetailedInfo();
                }
            }
        }

        private void RequestListBox_DoubleClick(object sender, EventArgs e)
        {
            if (requestListBox.SelectedIndex >= 0)
            {
                string displayText = requestListBox.SelectedItem.ToString();
                string requestId = ExtractRequestId(displayText);

                var request = requestManager.GetRequestById(requestId);
                if (request != null)
                {
                    MessageBox.Show(request.GetDetailedInfo(), "Request Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private string ExtractRequestId(string displayText)
        {
            // Extract request ID from display text (format: "📝 SR-20231201-ABC12345 - Title [🟢 Normal]")
            int startIndex = displayText.IndexOf("SR-");
            if (startIndex < 0) return "";

            int endIndex = displayText.IndexOf(" - ", startIndex);
            if (endIndex < 0) return "";

            return displayText.Substring(startIndex, endIndex - startIndex);
        }

        private void UpdateRequestStatus()
        {
            if (requestListBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a request first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string displayText = requestListBox.SelectedItem.ToString();
            string requestId = ExtractRequestId(displayText);
            var request = requestManager.GetRequestById(requestId);

            if (request == null) return;

            // Create a simple dialog to update status
            Form statusForm = new Form
            {
                Text = "Update Request Status",
                Width = 400,
                Height = 250,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            Label statusLabel = new Label { Text = "New Status:", AutoSize = true };
            ComboBox statusCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (RequestStatus status in Enum.GetValues(typeof(RequestStatus)))
                statusCombo.Items.Add(status);
            statusCombo.SelectedItem = request.Status;

            Label noteLabel = new Label { Text = "Update Note:", AutoSize = true };
            TextBox noteBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };

            Button okButton = new Button { Text = "Update", Dock = DockStyle.Fill };
            Button cancelButton = new Button { Text = "Cancel", Dock = DockStyle.Fill };

            okButton.Click += (s, e) =>
            {
                RequestStatus newStatus = (RequestStatus)statusCombo.SelectedItem;
                string note = noteBox.Text;

                requestManager.UpdateRequestStatus(requestId, newStatus, note);
                dbHandler.UpdateServiceRequest(request);

                statusForm.DialogResult = DialogResult.OK;
                statusForm.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                statusForm.DialogResult = DialogResult.Cancel;
                statusForm.Close();
            };

            layout.Controls.Add(statusLabel, 0, 0);
            layout.Controls.Add(statusCombo, 1, 0);
            layout.Controls.Add(noteLabel, 0, 1);
            layout.Controls.Add(noteBox, 1, 1);
            layout.Controls.Add(okButton, 0, 3);
            layout.Controls.Add(cancelButton, 1, 3);

            statusForm.Controls.Add(layout);

            if (statusForm.ShowDialog() == DialogResult.OK)
            {
                ApplyFilters();
                MessageBox.Show("Status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowStatistics()
        {
            var stats = requestManager.GetStatistics();

            string statsText = "=== Service Request Statistics ===\n\n";

            statsText += "Data Structure Statistics:\n";
            statsText += $"  • Total Requests: {stats["Total Requests"]}\n";
            statsText += $"  • BST Height: {stats["BST Height"]}\n";
            statsText += $"  • AVL Height: {stats["AVL Height"]} (Balanced: {stats["AVL Is Balanced"]})\n";
            statsText += $"  • Red-Black Height: {stats["Red-Black Height"]}\n";
            statsText += $"  • Red-Black Black Height: {stats["Red-Black Black Height"]}\n";
            statsText += $"  • Priority Heap Size: {stats["Priority Heap Size"]}\n";
            statsText += $"  • Heap Height: {stats["Heap Height"]}\n";
            statsText += $"  • Graph Vertices: {stats["Graph Vertices"]}\n";
            statsText += $"  • Graph Edges: {stats["Graph Edges"]}\n";
            statsText += $"  • Categories: {stats["Categories"]}\n";
            statsText += $"  • Connected Components: {stats["Connected Components"]}\n\n";

            statsText += "Status Breakdown:\n";
            foreach (RequestStatus status in Enum.GetValues(typeof(RequestStatus)))
            {
                statsText += $"  • {status}: {stats[$"Status: {status}"]}\n";
            }

            MessageBox.Show(statsText, "Data Structure Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatistics()
        {
            int total = requestManager.TotalRequests;
            int unresolved = requestManager.GetUnresolvedRequests().Count;
            int overdue = requestManager.GetOverdueRequests().Count;

            statsLabel.Text = $"Total: {total} | Unresolved: {unresolved} | Overdue: {overdue} | " +
                            $"BST Height: {requestManager.GetStatistics()["BST Height"]} | " +
                            $"AVL Height: {requestManager.GetStatistics()["AVL Height"]} | " +
                            $"Showing: {requestListBox.Items.Count} requests";
        }

        private void AddTestData()
        {
            try
            {
                var testRequests = GenerateTestRequests();

                foreach (var request in testRequests)
                {
                    requestManager.AddRequest(request);
                    dbHandler.InsertServiceRequest(request);
                }

                LoadData();
                MessageBox.Show($"Added {testRequests.Count} test requests successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding test data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<ServiceRequest> GenerateTestRequests()
        {
            var requests = new List<ServiceRequest>();

            var categories = new[] { "Roads", "Water", "Electricity", "Sanitation", "Parks", "Safety" };
            var locations = new[] { "Downtown", "Suburb A", "Suburb B", "Industrial Area", "City Center" };
            var titles = new[] {
                "Pothole on Main Street",
                "Water leak reported",
                "Street light not working",
                "Garbage not collected",
                "Park bench damaged",
                "Traffic signal malfunction",
                "Broken water pipe",
                "Road sign missing",
                "Sidewalk repair needed",
                "Tree trimming request"
            };

            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                var request = new ServiceRequest
                {
                    Title = titles[random.Next(titles.Length)],
                    Description = $"This is a test service request #{i + 1}. The issue requires attention from the relevant department.",
                    Category = categories[random.Next(categories.Length)],
                    Location = locations[random.Next(locations.Length)],
                    Status = (RequestStatus)random.Next(0, 6),
                    Priority = (RequestPriority)random.Next(0, 5),
                    SubmittedBy = "Test User",
                    AssignedTo = random.Next(0, 2) == 0 ? "Maintenance Team" : null
                };

                request.SubmittedDate = DateTime.Now.AddDays(-random.Next(0, 60));

                if (request.Status == RequestStatus.Resolved || request.Status == RequestStatus.Closed)
                {
                    request.ResolvedDate = request.SubmittedDate.AddDays(random.Next(1, 15));
                }

                requests.Add(request);
            }

            return requests;
        }
    }
}
