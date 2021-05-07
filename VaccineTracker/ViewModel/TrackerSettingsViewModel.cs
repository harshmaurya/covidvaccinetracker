using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VaccineTracker.Annotations;
using VaccineTracker.Core;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.ViewModel
{
    public class TrackerSettingsViewModel : INotifyPropertyChanged
    {
        private District _selectedDistrict;
        private State _selectedState;
        private readonly VaccineApi _api;
        private int _minimumAvailability;
        private string _name;
        private bool _soundAlarm;
        private string _email;

        public TrackerSettingsViewModel()
        {
            States = new ObservableCollection<State>();
            Districts = new ObservableCollection<District>();
            AgeGroup = new ObservableCollection<string>();
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(14);
            Name = "Default Tracker";
            MinimumAvailability = 1;
            SoundAlarm = true;

            ClearAddAgeGroups();
            ClearAddDefaultDistrict();
            _api = new VaccineApi();
        }

        private void ClearAddDefaultDistrict()
        {
            Districts.Clear();
            Districts.Add(District.Any());
        }

        private void ClearAddAgeGroups()
        {
            AgeGroup.Clear();
            AgeGroup.Add(Core.AgeGroup.Over18.ToString());
            AgeGroup.Add(Core.AgeGroup.Over45.ToString());
        }

        public async Task InitializeAsync()
        {
            var states = await _api.GetStates();
            foreach (var state in states)
            {
                States.Add(state);
            }
        }

        public ObservableCollection<State> States { get; set; }

        public ObservableCollection<District> Districts { get; set; }

        public ObservableCollection<string> AgeGroup { get; set; }

        public string SelectedAgeGroup { get; set; }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public bool SoundAlarm
        {
            get => _soundAlarm;
            set
            {
                _soundAlarm = value;
                OnPropertyChanged();
            }
        }

        public State SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState = value;
                PopulateDistricts(value);
                OnPropertyChanged();
            }
        }

        private async void PopulateDistricts(State state)
        {
            ClearAddDefaultDistrict();
            var districts = await _api.GetDistricts(state.StateId);
            foreach (var district in districts)
            {
                Districts.Add(district);
            }
        }

        public District SelectedDistrict
        {
            get => _selectedDistrict;
            set
            {
                _selectedDistrict = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public int MinimumAvailability
        {
            get => _minimumAvailability;
            set
            {
                _minimumAvailability = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}