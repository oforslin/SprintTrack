using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SprintTrack.Models;
using SprintTrack.Services;

namespace SprintTrack.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private DateTime _currentMonth;
        private TrainingSession? _selectedSession;
        private bool _isAddingSession;
        private DateTime? _selectedDate;
        private readonly ITrainingDataService _trainingDataService;

        public CalendarViewModel(ITrainingDataService trainingDataService)
        {
            _trainingDataService = trainingDataService;
            
            // Initialize collections
            CalendarDays = new ObservableCollection<CalendarDay>();
            SelectedDayTrainingSessions = new ObservableCollection<TrainingSession>();
            
            // Initialize training types
            InitializeTrainingTypes();
            
            // Now set CurrentMonth which will trigger GenerateCalendarDays()
            CurrentMonth = DateTime.Today;
            
            // Commands
            PreviousMonthCommand = new Command(PreviousMonth);
            NextMonthCommand = new Command(NextMonth);
            AddSessionCommand = new Command<DateTime>(AddSession);
            SelectDayCommand = new Command<DateTime>(SelectDay);
            DeleteSessionCommand = new Command<TrainingSession>(DeleteSession);
            SaveSessionCommand = new Command(SaveSession);
            CancelAddSessionCommand = new Command(CancelAddSession);
            ViewSessionCommand = new Command<TrainingSession>(ViewSession);
            AddCustomTrainingTypeCommand = new Command(AddCustomTrainingType);
            ClearSearchCommand = new Command(ClearSearch);
            SelectTrainingTypeCommand = new Command<string>(SelectTrainingType);
            EntryFocusedCommand = new Command(OnEntryFocused);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            HideDropdownCommand = new Command(HideDropdown);
        }

        public ObservableCollection<TrainingSession> TrainingSessions => _trainingDataService.TrainingSessions;
        public ObservableCollection<CalendarDay> CalendarDays { get; }
        public ObservableCollection<TrainingSession> SelectedDayTrainingSessions { get; }

        public DateTime CurrentMonth
        {
            get => _currentMonth;
            set
            {
                _currentMonth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentMonthDisplay));
                GenerateCalendarDays();
            }
        }

        public string CurrentMonthDisplay => CurrentMonth.ToString("MMMM yyyy");

        public TrainingSession? SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddingSession
        {
            get => _isAddingSession;
            set
            {
                _isAddingSession = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand AddSessionCommand { get; }
        public ICommand SelectDayCommand { get; }
        public ICommand DeleteSessionCommand { get; }
        public ICommand SaveSessionCommand { get; }
        public ICommand CancelAddSessionCommand { get; }
        public ICommand ViewSessionCommand { get; }
        public ICommand AddCustomTrainingTypeCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand SelectTrainingTypeCommand { get; }
        public ICommand EntryFocusedCommand { get; }
        public ICommand EntryUnfocusedCommand { get; }
        public ICommand HideDropdownCommand { get; }

        // New session properties for adding
        private string _newSessionTitle = string.Empty;
        private string _newSessionDescription = string.Empty;
        private string _newSessionType = string.Empty;
        private TimeSpan _newSessionDuration = TimeSpan.FromHours(1);
        private int _newSessionIntensity = 5;
        private DateTime _newSessionDate = DateTime.Today;
        private string _trainingTypeSearchText = string.Empty;
        private bool _showDropdownList = false;

        public string NewSessionTitle 
        { 
            get => _newSessionTitle;
            set
            {
                _newSessionTitle = value;
                OnPropertyChanged();
            }
        }

        public string NewSessionDescription 
        { 
            get => _newSessionDescription;
            set
            {
                _newSessionDescription = value;
                OnPropertyChanged();
            }
        }

        public string NewSessionType 
        { 
            get => _newSessionType;
            set
            {
                _newSessionType = value;
                OnPropertyChanged();
                
                // Update the search text to match the selection
                if (!string.IsNullOrEmpty(value))
                {
                    _trainingTypeSearchText = value;
                    OnPropertyChanged(nameof(TrainingTypeSearchText));
                }
            }
        }

        public TimeSpan NewSessionDuration 
        { 
            get => _newSessionDuration;
            set
            {
                _newSessionDuration = value;
                OnPropertyChanged();
            }
        }

        public int NewSessionIntensity 
        { 
            get => _newSessionIntensity;
            set
            {
                _newSessionIntensity = value;
                OnPropertyChanged();
            }
        }

        public DateTime NewSessionDate 
        { 
            get => _newSessionDate;
            set
            {
                _newSessionDate = value;
                OnPropertyChanged();
            }
        }

        public string TrainingTypeSearchText
        {
            get => _trainingTypeSearchText;
            set
            {
                _trainingTypeSearchText = value;
                OnPropertyChanged();
                FilterTrainingTypes();
                
                // Show dropdown when user types or when entry gets focus
                ShowDropdownList = true;
                OnPropertyChanged(nameof(CanAddNewType));
                
                // Set as selected type
                NewSessionType = value;
            }
        }

        public bool ShowDropdownList
        {
            get => _showDropdownList;
            set
            {
                _showDropdownList = value;
                OnPropertyChanged();
            }
        }

        public bool CanAddNewType
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TrainingTypeSearchText) && 
                       !_allTrainingTypes.Any(t => t.Equals(TrainingTypeSearchText, StringComparison.OrdinalIgnoreCase));
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedDateDisplay));
                OnPropertyChanged(nameof(HasSelectedDate));
                UpdateSelectedDayTrainingSessions();
                UpdateCalendarDaysSelection();
            }
        }

        public string SelectedDateDisplay => SelectedDate?.ToString("dddd, dd MMMM yyyy") ?? "Ingen dag vald";

        public bool HasSelectedDate => SelectedDate.HasValue;

        // List of common training types
        private readonly ObservableCollection<string> _allTrainingTypes = new ObservableCollection<string>
        {
            "", // Empty option for "None selected"
            "Styrketräning / Strength Training",
            "Löpning / Running", 
            "Sprint",
            "Cirkelträning / Circuit Training",
            "Cykling / Cycling",
            "Mountainbike / MTB",
            "Simning / Swimming",
            "Yoga",
            "Pilates", 
            "CrossFit",
            "Funktionell träning / Functional Training",
            "Boxning / Boxing",
            "Kampsport / Martial Arts",
            "Konditionsträning / Cardio",
            "HIIT",
            "Tabata",
            "Stretching / Töjning",
            "Promenader / Walking",
            "Vandring / Hiking", 
            "Löpband / Treadmill",
            "Crosstrainer / Elliptical",
            "Spinning",
            "Dans / Dance",
            "Zumba",
            "Aerobics",
            "Tennis",
            "Squash",
            "Basket / Basketball",
            "Fotboll / Soccer",
            "Volleyboll / Volleyball", 
            "Badminton",
            "Golf",
            "Klättring / Rock Climbing",
            "Rodd / Rowing",
            "Kajakpaddling / Kayaking",
            "Skidåkning / Skiing",
            "Snowboard",
            "Skridskor / Ice Skating",
            "Innebandy / Floorball",
            "Handboll / Handball",
            "Annat / Other"
        };

        public ObservableCollection<string> TrainingTypes { get; } = new ObservableCollection<string>();

        public int FilteredTrainingTypesCount => TrainingTypes.Count;

        // Navigation events
        public event EventHandler<TrainingSession>? RequestNavigateToSession;

        private void PreviousMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
        }

        private void NextMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
        }

        private void AddSession(DateTime date)
        {
            NewSessionDate = date;
            NewSessionTitle = string.Empty;
            NewSessionDescription = string.Empty;
            NewSessionType = string.Empty;
            NewSessionDuration = TimeSpan.FromHours(1);
            NewSessionIntensity = 5;
            TrainingTypeSearchText = string.Empty;
            ShowDropdownList = false; // Hide dropdown initially
            
            // Ensure we show all training types initially
            FilterTrainingTypes();
            
            IsAddingSession = true;
        }

        private void ViewSession(TrainingSession? session)
        {
            if (session != null)
            {
                RequestNavigateToSession?.Invoke(this, session);
            }
        }

        private void AddCustomTrainingType()
        {
            if (!string.IsNullOrWhiteSpace(TrainingTypeSearchText) && 
                !_allTrainingTypes.Any(t => t.Equals(TrainingTypeSearchText, StringComparison.OrdinalIgnoreCase)))
            {
                // Add the custom type to our list (insert before "Annat / Other")
                var insertIndex = _allTrainingTypes.Count - 1; // Before "Annat / Other"
                _allTrainingTypes.Insert(insertIndex, TrainingTypeSearchText);
                
                // Refresh the filtered list
                FilterTrainingTypes();
                
                // Set as selected type
                NewSessionType = TrainingTypeSearchText;
                
                // Hide dropdown after adding
                ShowDropdownList = false;
                
                // Notify that CanAddNewType has changed
                OnPropertyChanged(nameof(CanAddNewType));
            }
        }

        private void InitializeTrainingTypes()
        {
            // Copy all training types to the filtered list
            TrainingTypes.Clear();
            foreach (var type in _allTrainingTypes)
            {
                TrainingTypes.Add(type);
            }
        }

        private void FilterTrainingTypes()
        {
            if (string.IsNullOrWhiteSpace(TrainingTypeSearchText))
            {
                // Show all types
                TrainingTypes.Clear();
                foreach (var type in _allTrainingTypes)
                {
                    TrainingTypes.Add(type);
                }
            }
            else
            {
                // Filter types based on search text
                var filtered = _allTrainingTypes
                    .Where(type => string.IsNullOrEmpty(type) || 
                                  type.ToLowerInvariant().Contains(TrainingTypeSearchText.ToLowerInvariant()))
                    .ToList();
                
                TrainingTypes.Clear();
                foreach (var type in filtered)
                {
                    TrainingTypes.Add(type);
                }
            }
            
            // Notify that the count has changed
            OnPropertyChanged(nameof(FilteredTrainingTypesCount));
            OnPropertyChanged(nameof(CanAddNewType));
        }

        private void SaveSession()
        {
            // Create training session without requiring title
            var session = new TrainingSession
            {
                Title = NewSessionTitle, // Can be empty
                Description = NewSessionDescription,
                Type = NewSessionType, // Can be empty
                Date = NewSessionDate,
                Duration = NewSessionDuration,
                Intensity = NewSessionIntensity
            };

            _trainingDataService.AddTrainingSession(session);
            GenerateCalendarDays(); // Refresh calendar to show new session
            UpdateSelectedDayTrainingSessions(); // Update selected day sessions
            IsAddingSession = false;
        }

        private void CancelAddSession()
        {
            IsAddingSession = false;
        }

        private void DeleteSession(TrainingSession session)
        {
            if (session != null)
            {
                _trainingDataService.RemoveTrainingSession(session);
                GenerateCalendarDays(); // Refresh calendar
                UpdateSelectedDayTrainingSessions(); // Update selected day sessions
            }
        }

        private void ClearSearch()
        {
            TrainingTypeSearchText = string.Empty;
            ShowDropdownList = false;
        }

        private void SelectTrainingType(string? selectedType)
        {
            if (!string.IsNullOrEmpty(selectedType))
            {
                NewSessionType = selectedType;
                TrainingTypeSearchText = selectedType;
                ShowDropdownList = false; // Hide dropdown after selection
            }
        }

        private void SelectDay(DateTime date)
        {
            if (date != DateTime.MinValue) // Only select valid dates
            {
                SelectedDate = date;
            }
        }

        public bool HasSelectedDaySessions => SelectedDayTrainingSessions.Any();

        private void GenerateCalendarDays()
        {
            // Defensive null check
            if (CalendarDays == null)
                return;
                
            CalendarDays.Clear();

            var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
            var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);

            // Add empty days for the beginning of the month
            for (int i = 0; i < firstDayOfWeek; i++)
            {
                CalendarDays.Add(new CalendarDay { Date = DateTime.MinValue, IsCurrentMonth = false });
            }

            // Add days of the current month
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, day);
                var sessionsForDay = TrainingSessions?.Where(s => s.Date.Date == date.Date).ToList() ?? new List<TrainingSession>();
                
                CalendarDays.Add(new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = true,
                    IsToday = date.Date == DateTime.Today,
                    Sessions = new ObservableCollection<TrainingSession>(sessionsForDay),
                    HasSessions = sessionsForDay.Any()
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnEntryFocused()
        {
            // När Entry får fokus - visa dropdown
            if (!string.IsNullOrEmpty(TrainingTypeSearchText))
            {
                // Om det redan finns en vald typ, rensa sökfältet så man ser alla alternativ
                TrainingTypeSearchText = string.Empty;
            }
            
            // Visa alla alternativ
            FilterTrainingTypes();
            ShowDropdownList = true;
        }

        private void OnEntryUnfocused()
        {
            // Hide dropdown when entry loses focus (with a small delay to allow selection)
            Task.Delay(300).ContinueWith(_ => 
            {
                MainThread.BeginInvokeOnMainThread(() => ShowDropdownList = false);
            });
        }

        private void HideDropdown()
        {
            ShowDropdownList = false;
        }

        private void UpdateSelectedDayTrainingSessions()
        {
            SelectedDayTrainingSessions.Clear();
            
            if (SelectedDate.HasValue)
            {
                var sessionsForDay = TrainingSessions.Where(s => s.Date.Date == SelectedDate.Value.Date).ToList();
                foreach (var session in sessionsForDay)
                {
                    SelectedDayTrainingSessions.Add(session);
                }
            }
        }

        private void UpdateCalendarDaysSelection()
        {
            foreach (var day in CalendarDays)
            {
                day.IsSelected = SelectedDate.HasValue && day.Date.Date == SelectedDate.Value.Date;
            }
        }
    }

    public class CalendarDay : INotifyPropertyChanged
    {
        private DateTime _date;
        private bool _isCurrentMonth;
        private bool _isToday;
        private bool _hasSessions;
        private bool _isSelected;
        private ObservableCollection<TrainingSession> _sessions = new();

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DayNumber));
            }
        }

        public bool IsCurrentMonth
        {
            get => _isCurrentMonth;
            set
            {
                _isCurrentMonth = value;
                OnPropertyChanged();
            }
        }

        public bool IsToday
        {
            get => _isToday;
            set
            {
                _isToday = value;
                OnPropertyChanged();
            }
        }

        public bool HasSessions
        {
            get => _hasSessions;
            set
            {
                _hasSessions = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TrainingSession> Sessions
        {
            get => _sessions;
            set
            {
                _sessions = value;
                OnPropertyChanged();
            }
        }

        public string DayNumber => Date == DateTime.MinValue ? "" : Date.Day.ToString();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}