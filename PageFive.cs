using System;
using System.Drawing;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal partial class PageFive : PageBase
    {
        private TextBox nameBox;
        private TextBox emailBox;
        private TextBox phoneBox;
        private TextBox addressBox;
        private Label statusLabel;

        public PageFive() : base("Account")
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20)
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            // Content panel
            Panel contentPanel = CreateContentPanel();
            mainLayout.Controls.Add(contentPanel, 0, 0);

            // Status label
            statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Green,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            mainLayout.Controls.Add(statusLabel, 0, 1);

            Controls.Add(mainLayout);
            BackRequested += () => { };
        }

        private Panel CreateContentPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(20)
            };

            // Set column styles
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Title
            Label titleLabel = new Label
            {
                Text = "User Account Information",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(0, 102, 204),
                Padding = new Padding(0, 10, 0, 20),
                Dock = DockStyle.Fill
            };
            layout.Controls.Add(titleLabel, 0, 0);
            layout.SetColumnSpan(titleLabel, 2);

            // Name
            Label nameLabel = new Label
            {
                Text = "Full Name:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };
            nameBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(nameLabel, 0, 1);
            layout.Controls.Add(nameBox, 1, 1);

            // Email
            Label emailLabel = new Label
            {
                Text = "Email Address:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };
            emailBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(emailLabel, 0, 2);
            layout.Controls.Add(emailBox, 1, 2);

            // Phone
            Label phoneLabel = new Label
            {
                Text = "Phone Number:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };
            phoneBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(phoneLabel, 0, 3);
            layout.Controls.Add(phoneBox, 1, 3);

            // Address
            Label addressLabel = new Label
            {
                Text = "Address:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };
            addressBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                Height = 80
            };
            layout.Controls.Add(addressLabel, 0, 4);
            layout.Controls.Add(addressBox, 1, 4);

            // Buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 20, 0, 0)
            };

            Button saveButton = new Button
            {
                Text = "Save Changes",
                Width = 120,
                Height = 35,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.Click += SaveButton_Click;

            Button loadButton = new Button
            {
                Text = "Load Profile",
                Width = 120,
                Height = 35,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(10, 0, 0, 0)
            };
            loadButton.Click += LoadButton_Click;

            Button clearButton = new Button
            {
                Text = "Clear",
                Width = 120,
                Height = 35,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(10, 0, 0, 0)
            };
            clearButton.Click += (s, e) => ClearFields();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(loadButton);
            buttonPanel.Controls.Add(clearButton);

            layout.Controls.Add(buttonPanel, 1, 5);

            // Info section
            GroupBox infoBox = new GroupBox
            {
                Text = "Account Statistics",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            RichTextBox infoText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                Text = "Your account allows you to:\n\n" +
                       "• Submit and track service requests\n" +
                       "• Report municipal issues\n" +
                       "• Stay updated on local events\n" +
                       "• Receive notifications about your requests\n" +
                       "• View your submission history\n\n" +
                       "All your information is stored locally and securely."
            };

            infoBox.Controls.Add(infoText);

            layout.Controls.Add(infoBox, 0, 6);
            layout.SetColumnSpan(infoBox, 2);

            // Set row styles
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));  // Name
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));  // Email
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));  // Phone
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));  // Address
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Buttons
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Info

            panel.Controls.Add(layout);
            return panel;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    MessageBox.Show("Please enter your name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(emailBox.Text))
                {
                    MessageBox.Show("Please enter your email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Save to database
                string dbPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GovernmentServices",
                    "government.db"
                );

                using (var db = new DBHandler(dbPath))
                {
                    db.add_item("user_profile", "name", nameBox.Text);
                    db.add_item("user_profile", "email", emailBox.Text);
                    db.add_item("user_profile", "phone", phoneBox.Text);
                    db.add_item("user_profile", "address", addressBox.Text);
                }

                statusLabel.Text = "Profile saved successfully!";
                statusLabel.ForeColor = Color.Green;

                MessageBox.Show("Your profile has been saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Error saving profile.";
                statusLabel.ForeColor = Color.Red;
                MessageBox.Show($"Error saving profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            try
            {
                string dbPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GovernmentServices",
                    "government.db"
                );

                using (var db = new DBHandler(dbPath))
                {
                    nameBox.Text = db.return_item("user_profile", "name", "").Trim('"');
                    emailBox.Text = db.return_item("user_profile", "email", "").Trim('"');
                    phoneBox.Text = db.return_item("user_profile", "phone", "").Trim('"');
                    addressBox.Text = db.return_item("user_profile", "address", "").Trim('"');
                }

                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    statusLabel.Text = "No profile found. Please create one.";
                    statusLabel.ForeColor = Color.Orange;
                }
                else
                {
                    statusLabel.Text = "Profile loaded successfully!";
                    statusLabel.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Error loading profile.";
                statusLabel.ForeColor = Color.Red;
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            nameBox.Clear();
            emailBox.Clear();
            phoneBox.Clear();
            addressBox.Clear();
            statusLabel.Text = "";
        }
    }
}
