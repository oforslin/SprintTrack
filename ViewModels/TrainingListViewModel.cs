using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SprintTrack.Models;
using SprintTrack.Services;

namespace SprintTrack.ViewModels
{
    public class TrainingListViewModel : INotifyPropertyChanged
    {
        private bool _isAddingSession;
        private string _searchText = string.Empty;
        private readonly ITrainingDataService _trainingDataService;

        public TrainingListViewModel(ITrainingDataService trainingDataService)
        {
            _trainingDataService = trainingDataService;
            FilteredTrainingSessions = new ObservableCollection<TrainingSession>();
            
            // Subscribe to changes in the training sessions collection
            _trainingDataService.TrainingSessions.CollectionChanged += OnTrainingSessionsChanged;
            
            // Commands
            AddSessionCommand = new Command(AddSession);
            DeleteSessionCommand = new Command<TrainingSession>(DeleteSession);
            SearchCommand = new Command(FilterSessions);
            CancelAddSessionCommand = new Command(CancelAddSession);
            ViewSessionCommand = new Command<TrainingSession>(ViewSession);
            
            FilterSessions();
        }

        public ObservableCollection<TrainingSession> TrainingSessions => _trainingDataService.TrainingSessions;
        public ObservableCollection<TrainingSession> FilteredTrainingSessions { get; }

        public bool IsAddingSession
        {
            get => _isAddingSession;
            set
            {
                _isAddingSession = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterSessions();
            }
        }

        public int TotalTrainingsCount => TrainingSessions.Count;
        public int FilteredTrainingsCount => FilteredTrainingSessions.Count;

        // Commands
        public ICommand AddSessionCommand { get; }
        public ICommand DeleteSessionCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand CancelAddSessionCommand { get; }
        public ICommand ViewSessionCommand { get; }

        // Navigation events
        public event EventHandler<TrainingSession>? RequestNavigateToSession;
        
        // Property to check if navigation is available
        public bool HasNavigationHandlers => RequestNavigateToSession != null;

        private void AddSession()
        {
            // For now, just show a message. In a full implementation,
            // we could navigate to calendar view or show a simplified add dialog
            IsAddingSession = true;
        }

        private void CancelAddSession()
        {
            IsAddingSession = false;
        }

        private void ViewSession(TrainingSession? session)
        {
            if (session != null)
            {
                // Add logging for debugging
                System.Diagnostics.Debug.WriteLine($"ViewSession called for: {session.DisplayName} (ID: {session.Id})");
                System.Diagnostics.Debug.WriteLine($"Has navigation handlers: {HasNavigationHandlers}");
                
                // Ensure we have event listeners before invoking
                if (HasNavigationHandlers)
                {
                    RequestNavigateToSession?.Invoke(this, session);
                    System.Diagnostics.Debug.WriteLine("RequestNavigateToSession event invoked successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: No listeners for RequestNavigateToSession event!");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ViewSession called with null session");
            }
        }

        private void DeleteSession(TrainingSession session)
        {
            if (session != null)
            {
                _trainingDataService.RemoveTrainingSession(session);
                FilterSessions();
                OnPropertyChanged(nameof(TotalTrainingsCount));
            }
        }

        private void FilterSessions()
        {
            FilteredTrainingSessions.Clear();
            
            var filtered = string.IsNullOrWhiteSpace(SearchText) 
                ? TrainingSessions.OrderByDescending(s => s.Date)
                : TrainingSessions.Where(s => 
                    s.DisplayName.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()) ||
                    (s.Type?.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()) ?? false) ||
                    (s.Description?.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()) ?? false))
                    .OrderByDescending(s => s.Date);

            foreach (var session in filtered)
            {
                FilteredTrainingSessions.Add(session);
            }
            
            OnPropertyChanged(nameof(FilteredTrainingsCount));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnTrainingSessionsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Automatically refresh the filtered list when the source collection changes
            FilterSessions();
            OnPropertyChanged(nameof(TotalTrainingsCount));
        }
    }
}