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
using Windows.Storage;          // for ApplicationData
using Windows.Globalization.DateTimeFormatting; // datetime formatting
using Windows.ApplicationModel.Background;      // background tasks 
using Windows.UI;
using Clock.WinRT;
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
        private const string genderKey = "genderKey";           // For the baby's gender 
        private const string livetileKey = "livetileKey";       // For live tile style 
        private const string nameKey = "nameKey";               // For the baby's name 
        private const string TASKNAMEUSERPRESENT = "TileSchedulerTaskUserPresent";
        private const string TASKNAMETIMER = "TileSchedulerTaskTimerDeliveryDate";
        private const string TASKENTRYPOINT = "Clock.WinRT.TileSchedulerTaskDeliveryDate";
        public static DateTime DueDate;
        private DispatcherTimer timer;
        #endregion 

        #region MainPage Constructor 
        public MainPage()
        {
            this.InitializeComponent();
            appSettings = ApplicationData.Current.LocalSettings.Values;

            Loaded += OnLoaded;

            #region Initialize all selected values in the delivery date popup
            monthComboBox.SelectedIndex = 0;
            dayComboBox.SelectedIndex = 0;
            yearComboBox.SelectedIndex = 0;
            genderComboBox.SelectedIndex = 0;
            #endregion

            #region Show the date PopUp if there isn't a saved date
            if (!appSettings.ContainsKey(dateKey))
            {
                untilTxtBlock.Visibility = Visibility.Collapsed;
                countdownTxtBlock.Visibility = Visibility.Collapsed;
                datePopUp.IsOpen = true;
            }
            else
            {
                datePopUp.IsOpen = false;
                string date = appSettings[dateKey].ToString();
                var tempArray = date.Split(' ');    // Results in  tempArray[0] = xx/xx/xx
                var dateArray = tempArray[0].Split('/');
                int month = Convert.ToInt32(dateArray[0]);
                int day = Convert.ToInt32(dateArray[1]);
                int year = Convert.ToInt32(dateArray[2]);
                DueDate = new DateTime(year, month, day);
                //Clock.WinRT.ClockTileScheduler.SetGraduationDate(year, month, day); 
            }
            #endregion 

            
        }
        #endregion 

        #region Load State 
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
        #endregion 

        #region Save State 
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
        #endregion 

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!appSettings.ContainsKey(genderKey))    // If no key is contained, use the default blue 
            {
                grid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255));        // Blue by default 
            }
            else
            {
                string gender = appSettings[genderKey].ToString();
                if(gender == "Male")
                    grid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255));        // Blue
                else
                    grid.Background = new SolidColorBrush(Color.FromArgb(255, 244, 194, 202));      // Pink 

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

            #region Countdown to a baby's specific name if one is provided
            if (appSettings.ContainsKey(nameKey))
            {
                if (appSettings[nameKey].ToString() != "")
                {
                    untilTxtBlock.Text = "Until " + appSettings[nameKey].ToString() + "!";
                    babyNameTxtBox.Text = appSettings[nameKey].ToString();
                    
                }

            }
            else
            {
                untilTxtBlock.Text = "Until the baby!";
            }
            #endregion 
        }
        #endregion

        #region OnLoaded
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int CurrentYear = DateTime.Now.Year;                /* Get Current Year */
            DateTime NewYear = new DateTime(DateTime.Now.Year + 1, 1, 1);

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += new EventHandler<object>(OnTick);

            try
            {
                timer.Start();
                CreateClockTask();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region OnTick
        private void OnTick(object sender, object e)
        {
            var timeLeft = DueDate - DateTime.Now;

            countdownTxtBlock.Text = string.Format("{0:D2} days\n{1:D2} hours\n{2:D2} minutes\n{3:D2} seconds", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }
        #endregion 

        #region CreateClockTask
        public static async void CreateClockTask()
        {
            try
            {

                var result = await BackgroundExecutionManager.RequestAccessAsync();
                if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    foreach (var task in BackgroundTaskRegistration.AllTasks)
                    {
                        if (task.Value.Name == TASKNAMEUSERPRESENT)
                            task.Value.Unregister(true);
                    }
                    ClockTileScheduler.CreateSchedule();

                    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                    builder.Name = TASKNAMEUSERPRESENT;
                    builder.TaskEntryPoint = TASKENTRYPOINT;
                    builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
                    builder.Register();
                    var registration = builder.Register();
                }
            }
            catch
            {

            }
        }
        #endregion 

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

            if (babyNameTxtBox.Text != "" && appSettings != null)
                appSettings[nameKey] = babyNameTxtBox.Text;
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

        private void genderComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (genderComboBox.SelectedIndex != 0 && genderComboBox.SelectedItem != null && appSettings != null)
            {
                switch (genderComboBox.SelectedIndex)
                {
                    case 1:
                        appSettings[genderKey] = "Male";
                        grid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255));        // Blue
                        break;
                    case 2:
                        appSettings[genderKey] = "Female";
                        grid.Background = new SolidColorBrush(Color.FromArgb(255, 244, 194, 202));      // Pink 
                        break;
                    default: break;
                }
            }

        }

        private void datePopUp_Closed(object sender, object e)
        {
            if (appSettings.ContainsKey(genderKey))
            {
                string gender = appSettings[genderKey].ToString();
                if (gender == "Male")
                    grid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255));        // Blue
                else
                    grid.Background = new SolidColorBrush(Color.FromArgb(255, 244, 194, 202));      // Pink 
            }

            #region Countdown to a baby's specific name if one is provided
            if (appSettings.ContainsKey(nameKey))
            {
                if (appSettings[nameKey].ToString() != "")
                {
                    untilTxtBlock.Text = "Until " + appSettings[nameKey].ToString() + "!";
                    babyNameTxtBox.Text = appSettings[nameKey].ToString();

                }

            }
            else
            {
                untilTxtBlock.Text = "Until the baby!";
            }
            #endregion
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (babyNameTxtBox.Text == "")
            {
                appSettings[nameKey] = "";
            }
            base.OnNavigatedFrom(e);
        }

        private void datePopUp_Opened(object sender, object e)
        {
            if (!appSettings.ContainsKey(genderKey))    // If no key is contained, use the default blue 
            {
                grid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255));        // Blue by default 
            }
            else
            {
                string gender = appSettings[genderKey].ToString();
                if (gender == "Male")
                    grid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255));        // Blue
                else
                    grid.Background = new SolidColorBrush(Color.FromArgb(255, 244, 194, 202));      // Pink 

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

            #region Show the date PopUp if there isn't a saved date
            if (appSettings.ContainsKey(dateKey))
            {
                
                string date = appSettings[dateKey].ToString();
                var tempArray = date.Split(' ');    // Results in  tempArray[0] = xx/xx/xx
                var dateArray = tempArray[0].Split('/');
                int month = Convert.ToInt32(dateArray[0]);
                int day = Convert.ToInt32(dateArray[1]);
                int year = Convert.ToInt32(dateArray[2]);
                DueDate = new DateTime(year, month, day);

                dayComboBox.SelectedIndex = day - 1;

                monthComboBox.SelectedIndex = month - 1; 
                switch (year)
                {
                    case 2013:
                        yearComboBox.SelectedIndex = 0; 
                        break; 
                    case 2014:
                        yearComboBox.SelectedIndex = 1;
                        break; 
                    case 2015:
                        yearComboBox.SelectedIndex = 2;
                        break; 
                    case 2016:
                        yearComboBox.SelectedIndex = 3;
                        break; 
                    case 2017:
                        yearComboBox.SelectedIndex = 4;
                        break; 
                    case 2018:
                        yearComboBox.SelectedIndex = 5;
                        break; 
                    case 2019:
                        yearComboBox.SelectedIndex = 6;
                        break; 
                    default:
                        yearComboBox.SelectedIndex = 7;
                        break; 
                }
                //Clock.WinRT.ClockTileScheduler.SetGraduationDate(year, month, day); 
            }
            #endregion 
            
        }
    }
}
