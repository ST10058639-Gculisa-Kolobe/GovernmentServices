using System;
using System.Drawing;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal partial class StartPage : UserControl
    {
        public enum PageId { One, Two, Three, Four, Five }

        public event Action<PageId>? NavigateRequested;

        public StartPage()
        {
            var title = new Label
            {
                Text = "Welcome to the Government Services App",
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 60
            };

            var secondHeader = new Label
            {
                Text = "Please choose a service",
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 60
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                Anchor = AnchorStyles.None,
                WrapContents = false,
                Padding = new Padding(20),
                AutoScroll = true
            };

            var userPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };

            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            container.RowStyles.Add(new RowStyle(SizeType.Percent, 40)); // top 40%
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 20)); // bottom 20%

            var btn1 = NavButton("Report Issues", () => NavigateRequested?.Invoke(PageId.One));
            var btn2 = NavButton("Local Events and Announcements", () => NavigateRequested?.Invoke(PageId.Two));
            var btn3 = NavButton("Service Request Status", () => NavigateRequested?.Invoke(PageId.Three));

            var aboutBtn = UserButton("About", () => NavigateRequested?.Invoke(PageId.Four));
            var accountBtn = UserButton("Account", () => NavigateRequested?.Invoke(PageId.Five));

            // All pages are now fully implemented and enabled

            panel.Controls.AddRange(new Control[] { btn1, btn2, btn3 });
            userPanel.Controls.AddRange(new Control[] { aboutBtn, accountBtn });
            container.Controls.Add(panel, 0, 0);
            container.Controls.Add(userPanel, 0, 1);

            Controls.Add(container);
            Controls.Add(secondHeader);
            Controls.Add(title);
        }

        private static Button NavButton(string text, Action onClick)
        {
            var b = new Button
            {
                Text = text,
                Width = 220,
                Height = 140,
                Margin = new Padding(20),
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold)
            };
            b.Click += (_, __) => onClick();
            return b;
        }

        private static Button UserButton(string text, Action onClick)
            {
            var b = new Button
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(10),
                Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold),

            };
            b.Click += (_, __) => onClick();
            return b;
        }
    }
}