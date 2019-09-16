using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using IP_Man.Contracts;
using IP_Man.Model;

namespace IP_Man
{
    /// <summary>
    /// Interaktionslogik für IpManEditor.xaml
    /// </summary>
    public partial class IpManEditor : Page
    {
        private IpMan ipMan = IpMan.GetInstance;
        public IpManEditor()
        {
            InitializeComponent();
        }

        public IpManEditor(IpProfile ipProfile) : this()
        {
            // Bind to injected data
            this.DataContext = ipProfile;
            List<string> nicNames = ipMan.GetInterfaceNames();
            foreach (string name in nicNames)
            {
                Interface_Selection.Items.Add(name);
                if (ipProfile.GetInterfaceName() == name)
                {
                    Interface_Selection.SelectedItem = name;
                }
            }
            if (!ipProfile.GetDynamicIp())
            {
                DynamicDNS_Input.IsEnabled = false;
            }
        }

        // save changes to profile
        private void SaveChangedProfile(object sender, RoutedEventArgs e)
        {
            if (Interface_Selection.SelectedItem != null)
            {
                IpProfile oldProfile = (IpProfile)this.DataContext;
                IpProfile newProfile = new IpProfile(oldProfile.GetGuid(), ProfileName_Input.Text, Interface_Selection.SelectedItem.ToString(), (bool)DynamicIP_Input.IsChecked, IPAddress_Input.Text, SubnetMask_Input.Text, DefaultGateway_Input.Text, (bool)DynamicDNS_Input.IsChecked, new List<string>() { Nameserver1_Input.Text, Nameserver2_Input.Text });
                if (newProfile.IsValid())
                {
                    ipMan.UpdateProfile(newProfile);
                    NavigationService.Navigate(new IpManHome());
                }
                else
                {
                    MessageBox.Show("Bitte überprüfen Sie Ihre eingaben.");
                }
            } else
            {
                MessageBox.Show("Bitte wählen Sie einen Netzwerkadapter aus.");
            }
        }

        // delete profile
        private void DeleteProfile(object sender, RoutedEventArgs e)
        {
            ipMan.DeleteProfile((IpProfile)this.DataContext);
            NavigationService.Navigate(new IpManHome());
        }

        // disable static ip input
        private void DisableStaticIPInput(object sender, RoutedEventArgs e)
        {
            // Dynamic IP Checkbox Checked
            DynamicDNS_Input.IsEnabled = true;
            IPAddress_Input.IsEnabled = false;
            SubnetMask_Input.IsEnabled = false;
            DefaultGateway_Input.IsEnabled = false;
        }

        // disable static DNS Input
        private void DisableStaticDNSInput(object sender, RoutedEventArgs e)
        {
            Nameserver1_Input.IsEnabled = false;
            Nameserver2_Input.IsEnabled = false;
        }

        // enable static IP Input
        private void EnableStaticIPInput(object sender, RoutedEventArgs e)
        {
            IPAddress_Input.IsEnabled = true;
            SubnetMask_Input.IsEnabled = true;
            DefaultGateway_Input.IsEnabled = true;

            DynamicDNS_Input.IsChecked = false;
            DynamicDNS_Input.IsEnabled = false;
        }

        // enable static dns input
        private void EnableStaticDNSInput(object sender, RoutedEventArgs e)
        {
            Nameserver1_Input.IsEnabled = true;
            Nameserver2_Input.IsEnabled = true;
        }
    }
}
