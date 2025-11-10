using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal abstract class PageBase : UserControl
    {
        public event Action? BackRequested;

        protected PageBase(string titleText)
        {
            var top = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };

            var title = new Label
            {
                Text = titleText,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold)
            };

            Controls.Add(title);
            Controls.Add(top);
        }

        protected void AddBackButton(Panel top, DockStyle style, string name)
        {
            var backButton = new Button
            {
                Text = name,
                Dock = style,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            backButton.Click += (s, e) => OnBackRequested();
            top.Controls.Add(backButton);
        }

        protected void OnBackRequested() => BackRequested?.Invoke();
    }

}
