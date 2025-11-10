using System;
using System.Drawing;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal partial class PageFour : PageBase
    {
        public PageFour() : base("About")
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Main scroll panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };

            // Content panel
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 8,
                AutoSize = true,
                Padding = new Padding(10)
            };

            // Application Title
            Label titleLabel = new Label
            {
                Text = "Municipal Services Application",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 10)
            };
            contentLayout.Controls.Add(titleLabel, 0, 0);

            // Version
            Label versionLabel = new Label
            {
                Text = "Version 3.0 - Service Request Status Edition",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 20)
            };
            contentLayout.Controls.Add(versionLabel, 0, 1);

            // Description
            Label descLabel = new Label
            {
                Text = "This application provides citizens with easy access to essential municipal services, " +
                       "including issue reporting, event announcements, and service request tracking.",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                MaximumSize = new Size(800, 0),
                Padding = new Padding(0, 0, 0, 20)
            };
            contentLayout.Controls.Add(descLabel, 0, 2);

            // Features section
            GroupBox featuresBox = CreateFeaturesSection();
            contentLayout.Controls.Add(featuresBox, 0, 3);

            // Data Structures section
            GroupBox dataStructuresBox = CreateDataStructuresSection();
            contentLayout.Controls.Add(dataStructuresBox, 0, 4);

            // Technology Stack section
            GroupBox techBox = CreateTechnologySection();
            contentLayout.Controls.Add(techBox, 0, 5);

            // Credits
            Label creditsLabel = new Label
            {
                Text = "Developed as part of the PROG7312 Programming 3B course\n" +
                       "South African University Project - 2024",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Padding = new Padding(0, 20, 0, 10)
            };
            contentLayout.Controls.Add(creditsLabel, 0, 6);

            // Contact
            Label contactLabel = new Label
            {
                Text = "For support or feedback, please contact your municipal office.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 20)
            };
            contentLayout.Controls.Add(contactLabel, 0, 7);

            mainPanel.Controls.Add(contentLayout);
            Controls.Add(mainPanel);
            BackRequested += () => { };
        }

        private GroupBox CreateFeaturesSection()
        {
            GroupBox box = new GroupBox
            {
                Text = "Features",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(0, 10, 0, 10)
            };

            RichTextBox featuresText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                Height = 200,
                Text = "1. Report Issues\n" +
                       "   • Submit municipal issues with detailed descriptions\n" +
                       "   • Attach images and documents\n" +
                       "   • Track issue categories (Sanitation, Roads, Utilities, etc.)\n\n" +
                       "2. Local Events & Announcements\n" +
                       "   • Browse community events and announcements\n" +
                       "   • Advanced search and filtering\n" +
                       "   • Priority-based event display\n\n" +
                       "3. Service Request Status\n" +
                       "   • Track service requests with unique identifiers\n" +
                       "   • View detailed request history\n" +
                       "   • Filter by status, category, and priority\n" +
                       "   • Monitor dependencies between requests\n" +
                       "   • View comprehensive statistics"
            };

            box.Controls.Add(featuresText);
            return box;
        }

        private GroupBox CreateDataStructuresSection()
        {
            GroupBox box = new GroupBox
            {
                Text = "Advanced Data Structures",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(0, 10, 0, 10)
            };

            RichTextBox dataStructuresText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                Height = 220,
                Text = "The application utilizes multiple advanced data structures for efficient data management:\n\n" +
                       "• Binary Search Tree (BST) - Basic tree structure for sorted data\n" +
                       "• AVL Tree - Self-balancing BST ensuring O(log n) operations\n" +
                       "• Red-Black Tree - Alternative balanced tree with guaranteed performance\n" +
                       "• Min Heap - Priority queue for efficient priority-based operations\n" +
                       "• Graph - Manages dependencies between service requests\n" +
                       "  - Depth-First Search (DFS) for dependency traversal\n" +
                       "  - Breadth-First Search (BFS) for shortest paths\n" +
                       "  - Topological Sort for processing order\n" +
                       "  - Cycle Detection for circular dependency prevention\n" +
                       "  - Minimum Spanning Tree (Kruskal's algorithm)\n\n" +
                       "These structures ensure fast search, insertion, and retrieval operations\n" +
                       "even with large datasets."
            };

            box.Controls.Add(dataStructuresText);
            return box;
        }

        private GroupBox CreateTechnologySection()
        {
            GroupBox box = new GroupBox
            {
                Text = "Technology Stack",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(0, 10, 0, 10)
            };

            RichTextBox techText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                Height = 150,
                Text = "• Framework: .NET Framework 4.8\n" +
                       "• UI: Windows Forms (WinForms)\n" +
                       "• Database: SQLite with Microsoft.Data.Sqlite\n" +
                       "• Language: C# (Latest version)\n" +
                       "• Architecture: Multi-layered with separation of concerns\n" +
                       "• Data Persistence: Local SQLite database\n" +
                       "• Serialization: JSON for complex data types\n" +
                       "• Design Patterns: Singleton, Observer, Factory"
            };

            box.Controls.Add(techText);
            return box;
        }
    }
}
