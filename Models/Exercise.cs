using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SprintTrack.ViewModels;

namespace SprintTrack.Models
{
    public class Exercise : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = string.Empty;
        private string _description = string.Empty;
        private int _sets = 1;
        private int _reps = 1;
        private double _weight = 0;
        private TimeSpan _duration = TimeSpan.Zero;
        private double _distance = 0;
        private string _unit = "kg";
        private ExerciseType _exerciseType = ExerciseType.Strength;
        private ObservableCollection<ExerciseSet> _exerciseSets = new();
        
        // Running-specific properties
        private ObservableCollection<RunningSet> _runningSets = new();
        private int _sprintSeconds = 0;
        private int _sprintHundredths = 0;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
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

        public int Sets
        {
            get => _sets;
            set
            {
                _sets = Math.Max(1, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public int Reps
        {
            get => _reps;
            set
            {
                _reps = Math.Max(1, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                _weight = Math.Max(0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public double Distance
        {
            get => _distance;
            set
            {
                _distance = Math.Max(0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                
                // Update all exercise sets with new unit
                foreach (var exerciseSet in ExerciseSets)
                {
                    exerciseSet.Unit = value;
                }
            }
        }

        public ExerciseType ExerciseType
        {
            get => _exerciseType;
            set
            {
                _exerciseType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public ObservableCollection<ExerciseSet> ExerciseSets
        {
            get => _exerciseSets;
            set
            {
                _exerciseSets = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RunningSet> RunningSets
        {
            get => _runningSets;
            set
            {
                _runningSets = value;
                OnPropertyChanged();
            }
        }

        // Add properties for manual time input
        public int DurationHours
        {
            get => Duration.Hours;
            set
            {
                var newDuration = new TimeSpan(Math.Max(0, value), Duration.Minutes, Duration.Seconds);
                Duration = newDuration;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }

        public int DurationMinutes
        {
            get => Duration.Minutes;
            set
            {
                var newDuration = new TimeSpan(Duration.Hours, Math.Max(0, Math.Min(59, value)), Duration.Seconds);
                Duration = newDuration;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }

        public int DurationSeconds
        {
            get => Duration.Seconds;
            set
            {
                var newDuration = new TimeSpan(Duration.Hours, Duration.Minutes, Math.Max(0, Math.Min(59, value)));
                Duration = newDuration;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }

        public string DurationDisplay => $"{DurationHours:D2}:{DurationMinutes:D2}:{DurationSeconds:D2}";

        public int SprintSeconds
        {
            get => _sprintSeconds;
            set
            {
                _sprintSeconds = Math.Max(0, Math.Min(59, value));
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SprintTimeDisplay));
            }
        }

        public int SprintHundredths
        {
            get => _sprintHundredths;
            set
            {
                _sprintHundredths = Math.Max(0, Math.Min(99, value));
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SprintTimeDisplay));
            }
        }

        public string SprintTimeDisplay => $"{SprintSeconds:D2}.{SprintHundredths:D2}s";

        public string DisplayText
        {
            get
            {
                return ExerciseType switch
                {
                    ExerciseType.Strength => Weight > 0 ? $"{Sets} set × {Reps} reps @ {Weight} {Unit}" : $"{Sets} set × {Reps} reps",
                    ExerciseType.Cardio => Distance > 0 ? $"{Distance:F1} km - {Duration.Hours:D2}:{Duration.Minutes:D2}:{Duration.Seconds:D2}" : $"{Duration.Hours:D2}:{Duration.Minutes:D2}:{Duration.Seconds:D2}",
                    ExerciseType.Time => $"{Duration.Hours:D2}:{Duration.Minutes:D2}:{Duration.Seconds:D2}",
                    ExerciseType.Running => RunningSets.Count > 0 ? $"{RunningSets.Count} sets - {Duration.Hours:D2}:{Duration.Minutes:D2}:{Duration.Seconds:D2}" : $"{Duration.Hours:D2}:{Duration.Minutes:D2}:{Duration.Seconds:D2}",
                    ExerciseType.Sprinting => RunningSets.Count > 0 ? $"{RunningSets.Count} sets - {SprintTimeDisplay}" : SprintTimeDisplay,
                    ExerciseType.SledSprint => RunningSets.Count > 0 ? $"{RunningSets.Count} sets - {SprintTimeDisplay} @ {Weight}kg" : $"{SprintTimeDisplay} @ {Weight}kg",
                    _ => $"{Sets} set × {Reps} reps"
                };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ExerciseType
    {
        Strength,
        Cardio,
        Time,
        Running,
        Sprinting,
        SledSprint
    }

    public class RunningSet : INotifyPropertyChanged
    {
        private int _setNumber = 1;
        private TimeSpan _duration = TimeSpan.Zero;
        private double _distance = 0;
        private double _weight = 0; // For sled sprint
        private int _sprintSeconds = 0;
        private int _sprintHundredths = 0;
        private bool _isWarmupSet = false;

        public int SetNumber
        {
            get => _setNumber;
            set
            {
                _setNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SetDisplayText));
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public double Distance
        {
            get => _distance;
            set
            {
                _distance = Math.Max(0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                _weight = Math.Max(0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public int SprintSeconds
        {
            get => _sprintSeconds;
            set
            {
                _sprintSeconds = Math.Max(0, Math.Min(59, value));
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SprintTimeDisplay));
            }
        }

        public int SprintHundredths
        {
            get => _sprintHundredths;
            set
            {
                _sprintHundredths = Math.Max(0, Math.Min(99, value));
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SprintTimeDisplay));
            }
        }

        public bool IsWarmupSet
        {
            get => _isWarmupSet;
            set
            {
                _isWarmupSet = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SetDisplayText));
            }
        }

        // Add properties for manual time input for RunningSet
        public int DurationHours
        {
            get => Duration.Hours;
            set
            {
                var newDuration = new TimeSpan(Math.Max(0, value), Duration.Minutes, Duration.Seconds);
                Duration = newDuration;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }

        public int DurationMinutes
        {
            get => Duration.Minutes;
            set
            {
                var newDuration = new TimeSpan(Duration.Hours, Math.Max(0, Math.Min(59, value)), Duration.Seconds);
                Duration = newDuration;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }

        public int DurationSeconds
        {
            get => Duration.Seconds;
            set
            {
                var newDuration = new TimeSpan(Duration.Hours, Duration.Minutes, Math.Max(0, Math.Min(59, value)));
                Duration = newDuration;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }

        public string DurationDisplay => $"{DurationHours:D2}:{DurationMinutes:D2}:{DurationSeconds:D2}";

        public string SetDisplayText => IsWarmupSet ? "W" : $"Set {SetNumber}";

        public string SprintTimeDisplay => $"{SprintSeconds:D2}.{SprintHundredths:D2}s";

        public string DisplayText
        {
            get
            {
                var prefix = IsWarmupSet ? "Uppvärmning: " : $"Set {SetNumber}: ";
                
                if (SprintSeconds > 0 || SprintHundredths > 0)
                {
                    var result = $"{prefix}{SprintTimeDisplay}";
                    if (Distance > 0) result += $" - {Distance}m";
                    if (Weight > 0) result += $" @ {Weight}kg";
                    return result;
                }
                else
                {
                    return $"{prefix}{DurationDisplay}";
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ExerciseSet : INotifyPropertyChanged
    {
        private int _setNumber = 1;
        private int _reps = 10;
        private double _weight = 0;
        private string _unit = "kg";
        private bool _isWarmupSet = false;

        public int SetNumber
        {
            get => _setNumber;
            set
            {
                _setNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SetDisplayText));
            }
        }

        public int Reps
        {
            get => _reps;
            set
            {
                _reps = Math.Max(1, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                _weight = Math.Max(0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public bool IsWarmupSet
        {
            get => _isWarmupSet;
            set
            {
                _isWarmupSet = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(SetDisplayText));
            }
        }

        public string SetDisplayText => IsWarmupSet ? "W" : $"Set {SetNumber}";

        public string DisplayText => Weight > 0 ? $"{Reps} reps @ {Weight:F2} {Unit}" : $"{Reps} reps";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}