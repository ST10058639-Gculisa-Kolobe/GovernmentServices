using System;
using System.IO;
using System.Windows.Forms;

namespace GovernmentServices
{
    public partial class Form1 : Form
    {
        private readonly Panel _content = new Panel { Dock = DockStyle.Fill };

        // Keep one instance of each page so state is preserved when navigating back/forth
        private readonly StartPage _startPage;
        private readonly PageOne _pageOne;
        private readonly PageTwo _pageTwo;
        private readonly PageThree _pageThree;
        private readonly PageFour _pageFour;
        private readonly PageFive _pageFive;

        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(_content);

            // Instantiate pages
            _startPage = new StartPage();
            _pageOne = new PageOne();
            _pageTwo = new PageTwo();
            _pageThree = new PageThree();
            _pageFour = new PageFour();
            _pageFive = new PageFive();

            // Wire navigation
            _startPage.NavigateRequested += id =>
            {
                switch (id)
                {
                    case StartPage.PageId.One: ShowPage(_pageOne); break;
                    case StartPage.PageId.Two: ShowPage(_pageTwo); break;
                    case StartPage.PageId.Three: ShowPage(_pageThree); break;
                    case StartPage.PageId.Four: ShowPage(_pageFour); break;
                    case StartPage.PageId.Five: ShowPage(_pageFive); break;
                }
            };

            _pageOne.BackRequested += () => ShowPage(_startPage);
            _pageTwo.BackRequested += () => ShowPage(_startPage);
            _pageThree.BackRequested += () => ShowPage(_startPage);
            _pageFour.BackRequested += () => ShowPage(_startPage);
            _pageFive.BackRequested += () => ShowPage(_startPage);

            // Initial page
            ShowPage(_startPage);
        }

        private void ShowPage(UserControl page)
        {
            _content.SuspendLayout();
            _content.Controls.Clear();
            page.Dock = DockStyle.Fill;
            _content.Controls.Add(page);
            _content.ResumeLayout();
        }
    }
}