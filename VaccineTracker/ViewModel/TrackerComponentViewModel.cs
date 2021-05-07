using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using VaccineTracker.Annotations;
using VaccineTracker.Core;
using VaccineTracker.Core.Domain;
using VaccineTracker.Core.Dto;
using VaccineTracker.Domain;

namespace VaccineTracker.ViewModel
{
    public class TrackerComponentViewModel : INotifyPropertyChanged, IAlertHandler
    {

        private bool _isViewMode;
        private VaccineAlert _alert;
        private DateTime _lastUpdated;
        private bool _areSlotsAvailable;
        private bool _hasErrors;
        private bool _showErrors;

        public TrackerComponentViewModel()
        {
            TrackerSettings = new TrackerSettingsViewModel();
            AvailableSlots = new ObservableCollection<AvailableSlot>();
            Errors = new ObservableCollection<TrackingError>();
            IsViewMode = false;
        }

        public async Task InitializeAsync()
        {
            await TrackerSettings.InitializeAsync();
        }

        public event EventHandler TrackerDeleted;
        public event EventHandler TrackerSaved;

        public ICommand SaveCommand => new DelegateCommand(o =>
        {
            StartTracker();
            OnTrackerSaved();
        });

        public void StartTracker()
        {
            var error = ValidationError();
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }

            var handlers = new List<IAlertHandler> { this };
            if (TrackerSettings.SoundAlarm)
            {
                handlers.Add(new SoundHandler());
            }

            if (!string.IsNullOrEmpty(TrackerSettings.Email))
            {
                var password = Environment.GetEnvironmentVariable("EmailPassword");
                handlers.Add(new EmailSender(new EmailSettings
                {
                    Password = password,
                    EmailFrom = "covidvaccinetrack@gmail.com",
                    EmailTo = TrackerSettings.Email,
                    PortNumber = 587,
                    SmtpAddress = "smtp.gmail.com"
                }));
            }

            ReSubscribe(handlers);
            IsViewMode = true;
        }

        private async Task ReSubscribe(IEnumerable<IAlertHandler> handlers)
        {
            _alert?.Stop();
            _alert?.Dispose();
            _alert = new VaccineAlert(GetCriteria(), handlers.ToArray());
            await StartTrackerAsync();
        }

        public string ValidationError()
        {
            if (string.IsNullOrEmpty(TrackerSettings.SelectedAgeGroup)
                || string.IsNullOrEmpty(TrackerSettings.Name)
                || TrackerSettings.SelectedState == null
                || TrackerSettings.SelectedDistrict == null)
            {
                return "Please fill all the fields";
            }

            if (TrackerSettings.EndDate > DateTime.Today.AddDays(14))
            {
                return "Maximum value for end date is 14 days from today";
            }

            return "";
        }

        public ICommand EditCommand => new DelegateCommand(o =>
        {
            IsViewMode = false;
        });

        public ICommand PauseResumeCommand => new DelegateCommand(o =>
        {
            if (TrackingOn)
            {
                StopTracker();
            }
            else
            {
                StartTrackerAsync();
            }
        });

        private bool _trackingOn;
        private string _pauseResumeActionText;

        public string PauseResumeActionText
        {
            get => _pauseResumeActionText;
            set
            {
                _pauseResumeActionText = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteCommand => new DelegateCommand(o =>
        {
            StopTracker();
            OnTrackerDeleted();
        });


        public ObservableCollection<AvailableSlot> AvailableSlots { get; set; }

        public ObservableCollection<TrackingError> Errors { get; set; }

        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }


        public bool ShowErrors
        {
            get => _showErrors;
            set
            {
                _showErrors = value;
                OnPropertyChanged();
            }
        }


        public bool TrackingOn
        {
            get => _trackingOn;
            set
            {
                _trackingOn = value;
                OnPropertyChanged();
            }
        }

        private Criteria GetCriteria()
        {
            return new Criteria
            {
                StartDate = TrackerSettings.StartDate,
                EndDate = TrackerSettings.EndDate,
                MinimumAvailability = TrackerSettings.MinimumAvailability,
                AgeGroup = Enum.Parse<AgeGroup>(TrackerSettings.SelectedAgeGroup),
                State = TrackerSettings.SelectedState.StateName,
                District = TrackerSettings.SelectedDistrict
            };
        }


        private async Task StartTrackerAsync()
        {
            PauseResumeActionText = "Pause";
            TrackingOn = true;
            await _alert.Start();
        }

        private void StopTracker()
        {
            PauseResumeActionText = "Resume";
            TrackingOn = false;
            _alert?.Stop();
        }

        public TrackerSettingsViewModel TrackerSettings { get; set; }

        public bool IsViewMode
        {
            get => _isViewMode;
            set
            {
                _isViewMode = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                _lastUpdated = value;
                OnPropertyChanged();
            }
        }

        public bool AreSlotsAvailable
        {
            get => _areSlotsAvailable;
            set
            {
                _areSlotsAvailable = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void HandleVaccineAvailability(VaccineAvailability vaccineAvailability)
        {
            LastUpdated = DateTime.Now;
            AvailableSlots.Clear();
            AreSlotsAvailable = vaccineAvailability.AvailableSlots.Count > 0;
            vaccineAvailability.AvailableSlots.ForEach(slot => AvailableSlots.Add(slot));
        }

        public void HandlerError(Exception exception)
        {
            Errors.Add(new TrackingError
            {
                Time = DateTime.Now,
                Message = exception.Message
            });
            //HasErrors = true;
        }

        protected virtual void OnTrackerDeleted()
        {
            TrackerDeleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTrackerSaved()
        {
            TrackerSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}
