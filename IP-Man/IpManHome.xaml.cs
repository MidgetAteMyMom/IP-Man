using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using IP_Man.Model;
using IP_Man.Contracts;

namespace IP_Man
{
    /// <summary>
    /// Interaktionslogik für IpManHome.xaml
    /// </summary>
    public partial class IpManHome : Page
    {
        private IpMan ipMan = IpMan.GetInstance;
        public IpManHome()
        {
            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();

            // Set ProfileName as displayed property in ListBox
            this.profilesListBox.DisplayMemberPath = "ProfileName";

            foreach (IpProfile profile in ipMan.GetProfiles())
            {
                this.profilesListBox.Items.Add(profile);
            }
        }

        private void SetSelectedProfile(object sender, RoutedEventArgs e)
        {
            IpProfile profile = (IpProfile)this.profilesListBox.SelectedItem;
            if (profile != null)
            {
                try
                {
                    ipMan.ApplyProfile(profile);
                    (this.Parent as Window).WindowState = WindowState.Minimized;
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("Bitte verbinden Sie die Schnittstelle mit einem Netzwerk und stelle sicher, dass IPv4 eingeschaltet ist.");
                }
            } else
            {
                MessageBox.Show("Bitte wählen Sie ein Profil aus, welches angewendet werden soll");
            }
        }

        private void NavigateToProfileEditor(object sender, RoutedEventArgs e)
        {
            // Navigate to IP Profile Editor
            IpProfile selectedProfile = (IpProfile)this.profilesListBox.SelectedItem;
            if (selectedProfile != null)
            {
                IpManEditor profileEditorPage = new IpManEditor(selectedProfile);
                this.NavigationService.Navigate(profileEditorPage);
            } else
            {
                MessageBox.Show("Bitte wählen Sie ein Profil aus, welches editiert werden soll");
            }
        }

        private void NavigateToEmptyProfileEditor(object sender, RoutedEventArgs e)
        {
            IpManEditor profileEditorPage = new IpManEditor(new IpProfile("Neues Profil", ""));
            this.NavigationService.Navigate(profileEditorPage);
        }
    }
}
