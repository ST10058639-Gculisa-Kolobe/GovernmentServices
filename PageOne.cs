using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal partial class PageOne : PageBase
    {
        private readonly DBHandler local_db;

        // UI
        private readonly TextBox txtLocation = new() {  };
        private readonly ComboBox cmbCategory = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly RichTextBox rtbDescription = new() { BorderStyle = BorderStyle.FixedSingle };
        private readonly Button btnAttach = new() { Text = "Attach media…" };
        private readonly ListBox lstAttachments = new() { Height = 90 };
        private readonly Button btnSubmit = new() { Text = "Submit" };
        private readonly Button btnBack = new() { Text = "Cancel" };
        private readonly ProgressBar progress = new() { Minimum = 0, Maximum = 100, Height = 16 };
        private readonly Label lblStatus = new() { Text = "Fill in the details and click Submit.", AutoSize = true };

        private readonly List<string> attachments = new();

        // Prefer injecting DBHandler so the app can share one connection
        public PageOne(DBHandler db) : base("Report Issue")
        {
            local_db = db;
            local_db.EnsureIssuesTable();

            BuildUi();
            WireEvents();

            // Expose Back event from base to Form1.
            BackRequested += () => { };
        }

        // If you need a parameterless ctor, we can create our own DB path:
        public PageOne() : this(CreateDefaultDb()) { }

        private static DBHandler CreateDefaultDb()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "GovernmentServices");
            Directory.CreateDirectory(dir);
            var dbPath = Path.Combine(dir, "government.db");
            return new DBHandler(dbPath);
        }

        private void BuildUi()
        {
            Dock = DockStyle.Fill;

            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 6,
                Padding = new Padding(12)
            };
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // (PageBase header is already added above)
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // form grid
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // attachments list
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // progress+status
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // buttons
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));// spacer

            // FORM grid
            var form = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 3,
                RowCount = 3,
                AutoSize = true
            };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // labels
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // inputs
            form.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // attach btn

            // Row 0: Location
            form.Controls.Add(new Label { Text = "Location:", AutoSize = true, Margin = new Padding(0, 6, 8, 6) }, 0, 0);
            txtLocation.Width = 520;
            form.Controls.Add(txtLocation, 1, 0);

            // Row 1: Category
            form.Controls.Add(new Label { Text = "Category:", AutoSize = true, Margin = new Padding(0, 6, 8, 6) }, 0, 1);
            cmbCategory.Items.AddRange(new object[] { "Sanitation", "Roads", "Utilities", "Safety", "Other" });
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            form.Controls.Add(cmbCategory, 1, 1);

            // Row 2: Description + Attach
            form.Controls.Add(new Label { Text = "Description:", AutoSize = true, Margin = new Padding(0, 6, 8, 6) }, 0, 2);
            rtbDescription.Width = 520;
            rtbDescription.Height = 140;
            form.Controls.Add(rtbDescription, 1, 2);
            btnAttach.AutoSize = true;
            form.Controls.Add(btnAttach, 2, 2);

            outer.Controls.Add(form, 1, 1);

            // ATTACHMENTS list
            var attachPanel = new Panel { Dock = DockStyle.Top, Height = lstAttachments.Height + 28 };
            var attachLbl = new Label { Text = "Attached files:", AutoSize = true, Dock = DockStyle.Top };
            lstAttachments.Dock = DockStyle.Fill;
            attachPanel.Controls.Add(lstAttachments);
            attachPanel.Controls.Add(attachLbl);
            outer.Controls.Add(attachPanel, 1, 2);

            // PROGRESS + STATUS
            var statusPanel = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 2, AutoSize = true };
            statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            progress.Dock = DockStyle.Fill;
            lblStatus.Dock = DockStyle.Fill;
            statusPanel.Controls.Add(progress, 0, 0);
            statusPanel.Controls.Add(lblStatus, 1, 0);
            outer.Controls.Add(statusPanel, 1, 3);

            // BUTTONS
            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };
            btnSubmit.AutoSize = true;
            btnBack.AutoSize = true;
            buttons.Controls.Add(btnSubmit);
            buttons.Controls.Add(btnBack);
            outer.Controls.Add(buttons, 1, 4);

            Controls.Add(outer);

        }

        private void WireEvents()
        {
            btnAttach.Click += (_, __) => DoAttach();
                               
            btnSubmit.Click += (_, __) =>
            {
                try
                {
                    SetBusy(true);
                    SetProgress(10, "Validating…");

                    if (!ValidateInputs(out var msg))
                    {
                        SetProgress(0, msg);
                        return;
                    }

                    SetProgress(70, "Saving to database…");
                    local_db.InsertIssue(
                        location: txtLocation.Text.Trim(),
                        category: cmbCategory.SelectedItem?.ToString() ?? "",
                        description: rtbDescription.Text.Trim(),
                        mediaPaths: attachments
                    );

                    ClearInputs();
                    SetProgress(100, "Saved! Thank you for your report.");
                    OnBackRequested();
                }
                catch (Exception ex)
                {
                    SetProgress(0, "Error: " + ex.Message);
                }
                finally
                {
                    SetBusy(false);
                }
            };

            btnBack.Click += (_, __) =>
            {
                SetProgress(0, "Fill in the details and click Submit.");
                ClearInputs();
                OnBackRequested();
            };

        }

        private void DoAttach()
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Attach images or documents",
                Filter = "Images and Documents|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.txt|All files|*.*",
                Multiselect = true
            };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                foreach (var path in dlg.FileNames)
                    if (!attachments.Contains(path)) attachments.Add(path);
                RefreshAttachmentList();
                lblStatus.Text = $"{attachments.Count} file(s) attached.";
            }
        }

        private void RefreshAttachmentList()
        {
            lstAttachments.BeginUpdate();
            lstAttachments.Items.Clear();
            lstAttachments.Items.AddRange(attachments.ToArray());
            lstAttachments.EndUpdate();
        }

        private bool ValidateInputs(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                message = "Location is required.";
                txtLocation.Focus();
                return false;
            }
            if (cmbCategory.SelectedItem is null)
            {
                message = "Please select a category.";
                cmbCategory.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                message = "Please provide a short description.";
                rtbDescription.Focus();
                return false;
            }
            message = "OK";
            return true;
        }

        private void SetBusy(bool busy)
        {
            btnSubmit.Enabled = !busy;
            btnBack.Enabled = !busy;
            btnAttach.Enabled = !busy;
            UseWaitCursor = busy;
        }

        private void SetProgress(int value, string message)
        {
            progress.Value = Math.Max(progress.Minimum, Math.Min(progress.Maximum, value));
            lblStatus.Text = message;
        }

        private void ClearInputs()
        {
            txtLocation.Clear();
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            rtbDescription.Clear();
            attachments.Clear();
            RefreshAttachmentList();
        }
    }
}
