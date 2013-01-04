using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;          // for ApplicationData

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BabyCountdown
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SettingsPage : BabyCountdown.Common.LayoutAwarePage
    {

        private Windows.Foundation.Collections.IPropertySet appSettings;
        private const string genderKey = "genderKey";           // For the baby's gender 
        private const string nameKey = "nameKey";               // For the baby's name 

        public SettingsPage()
        {
            this.InitializeComponent();
            genderComboBox.SelectedIndex = 0;
            appSettings = ApplicationData.Current.LocalSettings.Values;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (appSettings.ContainsKey(genderKey))    // If no key is contained, use the default blue 
            {
                switch (appSettings[genderKey].ToString())
                {
                    case "Male":
                        genderComboBox.SelectedIndex = 1;
                        break;
                    case "Female":
                        genderComboBox.SelectedIndex = 2;
                        break;
                    default:
                        genderComboBox.SelectedIndex = 0;
                        break;

                }
            }
            

            if(appSettings.ContainsKey(nameKey))
            {
                babyNameTxtBox.Text = appSettings[nameKey].ToString(); 
            }
        }
        #endregion

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (babyNameTxtBox.Text == "")
            {
                appSettings[nameKey] = ""; 
            }
            base.OnNavigatedFrom(e);
        }

        private void genderComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (genderComboBox.SelectedIndex != 0 && genderComboBox.SelectedItem != null && appSettings != null)
            {
                switch (genderComboBox.SelectedIndex)
                {
                    case 1:
                        appSettings[genderKey] = "Male";
                        break; 
                    case 2:
                        appSettings[genderKey] = "Female";
                        break; 
                    default: break; 
                }
            }

        }
        private void saveBabyNameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (babyNameTxtBox.Text != "" && appSettings != null)
                appSettings[nameKey] = babyNameTxtBox.Text; 
        }
    }
}
