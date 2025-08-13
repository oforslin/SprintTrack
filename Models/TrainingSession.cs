using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SprintTrack.Models
{
    public class TrainingSession : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private string _title = string.Empty;
        private string _description = string.Empty;
        private DateTime _date;
        private TimeSpan _duration;
        private string _type = string.Empty;
        private int _intensity = 1;
        private ObservableCollection<Exercise> _exercises = new();

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public int Intensity
        {
            get => _intensity;
            set
            {
                _intensity = Math.Max(1, Math.Min(10, value));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Exercise> Exercises
        {
            get => _exercises;
            set
            {
                _exercises = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasExercises));
                OnPropertyChanged(nameof(ExerciseCount));
            }
        }

        public bool HasExercises => Exercises?.Any() == true;
        public int ExerciseCount => Exercises?.Count ?? 0;

        public string DurationDisplay => $"{Duration.Hours:D2}:{Duration.Minutes:D2}";

        // DisplayName property that shows appropriate text when title is empty
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Title))
                    return Title;
                
                if (!string.IsNullOrWhiteSpace(Type))
                    return $"{Type} - {Date:yyyy-MM-dd}";
                
                return $"Training Session - {Date:yyyy-MM-dd}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}