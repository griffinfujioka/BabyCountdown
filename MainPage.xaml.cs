﻿using System;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BabyCountdown
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : BabyCountdown.Common.LayoutAwarePage
    {

        #region Variable declarations
        private Windows.Foundation.Collections.IPropertySet appSettings;
        private const string dateKey = "dateKey";
        private const string TASKNAMEUSERPRESENT = "TileSchedulerTaskUserPresent";
        private const string TASKNAMETIMER = "TileSchedulerTaskTimerGraduation";
        private const string TASKENTRYPOINT = "Clock.WinRT.TileSchedulerTaskGraduation";
        public static DateTime DueDate;
        private DispatcherTimer timer;
        #endregion 

        public MainPage()
        {
            this.InitializeComponent();
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

        #region datePopUp Save Button Clicked
        private void saveDateBtn_Click(object sender, RoutedEventArgs e)
        {
            string monthString = monthComboBox.SelectedValue.ToString();
            int monthInt = 0;
            int day = (int)dayComboBox.SelectedValue;
            int year = (int)yearComboBox.SelectedValue;

            #region switch-statement to setup month as integer value
            switch (monthString)
            {
                case "January":
                    monthInt = 1;
                    break;
                case "February":
                    monthInt = 2;
                    break;
                case "March":
                    monthInt = 3;
                    break;
                case "April":
                    monthInt = 4;
                    break;
                case "May":
                    monthInt = 5;
                    break;
                case "June":
                    monthInt = 6;
                    break;
                case "July":
                    monthInt = 7;
                    break;
                case "August":
                    monthInt = 8;
                    break;
                case "September":
                    monthInt = 9;
                    break;
                case "October":
                    monthInt = 10;
                    break;
                case "November":
                    monthInt = 11;
                    break;
                case "December":
                    monthInt = 12;
                    break;
                default: break;
            }
            #endregion

            DueDate = new DateTime(year, monthInt, day);

            appSettings[dateKey] = DueDate.ToString();
            datePopUp.IsOpen = false;   // Close the PopUp
            untilTxtBlock.Visibility = Visibility.Visible;
            countdownTxtBlock.Visibility = Visibility.Visible;
        }
        #endregion 

        #region Change date button clicked
        private void changeDateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (datePopUp.IsOpen)   // If the popup is already open 
            {
                untilTxtBlock.Visibility = Visibility.Visible;
                countdownTxtBlock.Visibility = Visibility.Visible;
                datePopUp.IsOpen = false;   // close it 
            }
            else
            {
                untilTxtBlock.Visibility = Visibility.Collapsed;
                countdownTxtBlock.Visibility = Visibility.Collapsed;
                datePopUp.IsOpen = true;
            }
        }
        #endregion

        #region datePopup Cancel Button Clicked
        private void cancelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            untilTxtBlock.Visibility = Visibility.Visible;
            countdownTxtBlock.Visibility = Visibility.Visible;
            datePopUp.IsOpen = false;
        }
        #endregion

        #region About button clicked
        private void aboutBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }
        #endregion

        #region Settings button clicked
        private void settingsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }
        #endregion
    }
}