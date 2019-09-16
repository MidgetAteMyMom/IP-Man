using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
using IP_Man.Model;
using IP_Man.Contracts;

namespace IP_Man
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        // IpMan (Singleton) initialisieren
        private IpMan ipMan = IpMan.GetInstance;

        NotifyIcon nIcon = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            nIcon.Icon = Properties.Resources.icon;
            nIcon.Click += nIcon_Clicked;
            nIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            this.StateChanged += MainWindow_StateChanged;
            this.WindowState = WindowState.Minimized;
            this.Hide();
            nIcon.Visible = true;
        }

        private void nIcon_Clicked(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    break;
                case WindowState.Minimized:
                    nIcon.Visible = true;
                    this.Hide();
                    break;
                case WindowState.Normal:
                    nIcon.Visible = false;
                    break;
            }
        }
    }
}
