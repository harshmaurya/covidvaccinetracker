using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using VaccineTracker.Domain;
using VaccineTracker.Services;
using VaccineTracker.ViewModel;
using VaccineTracker.Views;

namespace VaccineTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<TrackerComponentViewModel> _viewModels = new List<TrackerComponentViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettingsAsync();
        }


        private void SetBusy(bool isBusy)
        {
            if (isBusy)
            {
                BusyIndicator.IsBusy = true;
                BusyIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                BusyIndicator.IsBusy = false;
                BusyIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadSettingsAsync()
        {
            try
            {
                SetBusy(true);
                var persistence = new SettingsPersistService();
                var settings = await persistence.LoadSettingsAsync();
                if (settings?.TrackerSettings == null)
                {
                    SetBusy(false);
                    return;
                }
                foreach (var setting in settings.TrackerSettings)
                {
                    var vm = await AddTracker();
                    var trackerSettings = vm.TrackerSettings;
                    trackerSettings.Name = setting.Name;
                    trackerSettings.SelectedState = setting.State;
                    trackerSettings.SelectedDistrict = setting.District;
                    trackerSettings.SelectedAgeGroup = setting.AgeGroup;
                    trackerSettings.MinimumAvailability = setting.MinimumSlots;
                    trackerSettings.StartDate = setting.StartDate;
                    trackerSettings.EndDate = setting.EndDate;
                    trackerSettings.Email = setting.Email;
                    trackerSettings.SoundAlarm = setting.SoundAlarm;
                    vm.StartTracker();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task SaveSettingsAsync()
        {
            SetBusy(true);
            var persistence = new SettingsPersistService();
            var userSettings = new UserSettings
            {
                TrackerSettings = new List<TrackerSettings>()
            };
            foreach (var vm in _viewModels)
            {
                var vmSettings = vm.TrackerSettings;
                var setting = new TrackerSettings
                {
                    Name = vmSettings.Name,
                    State = vmSettings.SelectedState,
                    District = vmSettings.SelectedDistrict,
                    AgeGroup = vmSettings.SelectedAgeGroup,
                    MinimumSlots = vmSettings.MinimumAvailability,
                    StartDate = vmSettings.StartDate,
                    EndDate = vmSettings.EndDate,
                    Email = vmSettings.Email,
                    SoundAlarm = vmSettings.SoundAlarm
                };
                userSettings.TrackerSettings.Add(setting);
            }

            await persistence.SaveSettingsAsync(userSettings);
            SetBusy(false);
        }


        private async void OnAddTrackerClick(object sender, RoutedEventArgs e)
        {
            await AddTracker();
        }

        private async Task<TrackerComponentViewModel> AddTracker()
        {
            try
            {
                var vm = new TrackerComponentViewModel();
                await vm.InitializeAsync();
                var view = new TrackerComponent { DataContext = vm };
                Panel.Children.Add(view);
                vm.TrackerDeleted += async (o, args) =>
                {
                    Panel.Children.Remove(view);
                    _viewModels.Remove(vm);
                    await SaveSettingsAsync();
                };
                vm.TrackerSaved += async (sender, args) =>
                {
                    await SaveSettingsAsync();
                };
                _viewModels.Add(vm);
                return vm;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Tracking will stop if you close the application. Are you sure?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            const string url = @"https://www.harshmaurya.in/covid-vaccine-tracker-india/";
            Process.Start("explorer.exe", url);
        }
    }

    public class UserSettings
    {
        public List<TrackerSettings> TrackerSettings { get; set; }
    }
}
