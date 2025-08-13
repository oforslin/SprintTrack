using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SprintTrack.Models;

namespace SprintTrack.ViewModels
{
    public class TrainingSessionDetailViewModel : INotifyPropertyChanged
    {
        private TrainingSession _trainingSession;
        private Exercise? _selectedExercise;
        private bool _isAddingExercise;
        private string _newExerciseName = string.Empty;
        private string _newExerciseDescription = string.Empty;
        private int _newExerciseSets = 1;
        private int _newExerciseReps = 1;
        private double _newExerciseWeight = 0;
        private TimeSpan _newExerciseDuration = TimeSpan.Zero;
        private double _newExerciseDistance = 0;
        private string _newExerciseUnit = "kg";
        private ExerciseType _newExerciseType = ExerciseType.Strength;
        private string _exerciseSearchText = string.Empty;
        private bool _showExerciseDropdown = false;
        private ObservableCollection<ExerciseSet> _newExerciseSetsList = new();
        private ObservableCollection<RunningSet> _newRunningSetsList = new();
        private int _newSprintSeconds = 0;
        private int _newSprintHundredths = 0;

        // Field to store the set being dragged
        private object? _draggedSet;

        public TrainingSessionDetailViewModel(TrainingSession trainingSession)
        {
            _trainingSession = trainingSession;

            InitializeCommonExercises();

            AddExerciseCommand = new Command(AddExercise);
            SaveExerciseCommand = new Command(SaveExercise);
            CancelAddExerciseCommand = new Command(CancelAddExercise);
            DeleteExerciseCommand = new Command<Exercise>(DeleteExercise);
            EditExerciseCommand = new Command<Exercise>(EditExercise);
            BackCommand = new Command(GoBack);
            AddCustomExerciseCommand = new Command(AddCustomExercise);
            SelectExerciseCommand = new Command<string>(SelectExercise);
            ExerciseEntryFocusedCommand = new Command(OnExerciseEntryFocused);
            ExerciseEntryUnfocusedCommand = new Command(OnExerciseEntryUnfocused);
            HideExerciseDropdownCommand = new Command(HideExerciseDropdown);
            ToggleExerciseDropdownCommand = new Command(ToggleExerciseDropdown);
            IncreaseRepsCommand = new Command<ExerciseSet>(IncreaseReps);
            DecreaseRepsCommand = new Command<ExerciseSet>(DecreaseReps);
            IncreaseWeightCommand = new Command<ExerciseSet>(IncreaseWeight);
            DecreaseWeightCommand = new Command<ExerciseSet>(DecreaseWeight);
            AddSetCommand = new Command<Exercise>(AddSet);
            RemoveSetCommand = new Command<ExerciseSet>(RemoveSet);
            IncreaseDurationCommand = new Command<Exercise>(IncreaseDuration);
            DecreaseDurationCommand = new Command<Exercise>(DecreaseDuration);
            IncreaseDistanceCommand = new Command<Exercise>(IncreaseDistance);
            DecreaseDistanceCommand = new Command<Exercise>(DecreaseDistance);
            AddNewSetCommand = new Command(AddNewSet);
            RemoveNewSetCommand = new Command<ExerciseSet>(RemoveNewSet);
            IncreaseNewRepsCommand = new Command<ExerciseSet>(IncreaseNewReps);
            DecreaseNewRepsCommand = new Command<ExerciseSet>(DecreaseNewReps);
            IncreaseNewWeightCommand = new Command<ExerciseSet>(IncreaseNewWeight);
            DecreaseNewWeightCommand = new Command<ExerciseSet>(DecreaseNewWeight);

            AddNewRunningSetCommand = new Command(AddNewRunningSet);
            RemoveNewRunningSetCommand = new Command<RunningSet>(RemoveNewRunningSet);
            AddRunningSetCommand = new Command<Exercise>(AddRunningSet);
            RemoveRunningSetCommand = new Command<RunningSet>(RemoveRunningSet);

            IncreaseSprintSecondsCommand = new Command<RunningSet>(IncreaseSprintSeconds);
            DecreaseSprintSecondsCommand = new Command<RunningSet>(DecreaseSprintSeconds);
            IncreaseSprintHundredthsCommand = new Command<RunningSet>(IncreaseSprintHundredths);
            DecreaseSprintHundredthsCommand = new Command<RunningSet>(DecreaseSprintHundredths);
            IncreaseRunningDistanceCommand = new Command<RunningSet>(IncreaseRunningDistance);
            DecreaseRunningDistanceCommand = new Command<RunningSet>(DecreaseRunningDistance);
            IncreaseRunningWeightCommand = new Command<RunningSet>(IncreaseRunningWeight);
            DecreaseRunningWeightCommand = new Command<RunningSet>(DecreaseRunningWeight);

            IncreaseNewSprintSecondsCommand = new Command<RunningSet>(IncreaseNewSprintSeconds);
            DecreaseNewSprintSecondsCommand = new Command<RunningSet>(DecreaseNewSprintSeconds);
            IncreaseNewSprintHundredthsCommand = new Command<RunningSet>(IncreaseNewSprintHundredths);
            DecreaseNewSprintHundredthsCommand = new Command<RunningSet>(DecreaseNewSprintHundredths);
            IncreaseNewRunningDistanceCommand = new Command<RunningSet>(IncreaseNewRunningDistance);
            DecreaseNewRunningDistanceCommand = new Command<RunningSet>(DecreaseNewRunningDistance);
            IncreaseNewRunningWeightCommand = new Command<RunningSet>(IncreaseNewRunningWeight);
            DecreaseNewRunningWeightCommand = new Command<RunningSet>(DecreaseNewRunningWeight);

            ToggleWarmupSetCommand = new Command<ExerciseSet>(ToggleWarmupSet);
            ToggleWarmupRunningSetCommand = new Command<RunningSet>(ToggleWarmupRunningSet);
            ToggleNewWarmupSetCommand = new Command<ExerciseSet>(ToggleNewWarmupSet);
            ToggleNewWarmupRunningSetCommand = new Command<RunningSet>(ToggleNewWarmupRunningSet);

            // Drag and Drop Commands
            DragStartingCommand = new Command<object>(OnDragStarting);
            DropCommand = new Command<object>(OnDrop);
        }

        public TrainingSession TrainingSession { get => _trainingSession; set { _trainingSession = value; OnPropertyChanged(); } }
        public Exercise? SelectedExercise { get => _selectedExercise; set { _selectedExercise = value; OnPropertyChanged(); } }
        public bool IsAddingExercise { get => _isAddingExercise; set { _isAddingExercise = value; OnPropertyChanged(); } }
        public string NewExerciseName { get => _newExerciseName; set { _newExerciseName = value; OnPropertyChanged(); } }
        public string NewExerciseDescription { get => _newExerciseDescription; set { _newExerciseDescription = value; OnPropertyChanged(); } }
        public int NewExerciseSets { get => _newExerciseSets; set { _newExerciseSets = Math.Max(1, value); OnPropertyChanged(); } }
        public int NewExerciseReps { get => _newExerciseReps; set { _newExerciseReps = Math.Max(1, value); OnPropertyChanged(); } }
        public double NewExerciseWeight { get => _newExerciseWeight; set { _newExerciseWeight = Math.Max(0, value); OnPropertyChanged(); } }
        public TimeSpan NewExerciseDuration { get => _newExerciseDuration; set { _newExerciseDuration = value; OnPropertyChanged(); } }
        public double NewExerciseDistance { get => _newExerciseDistance; set { _newExerciseDistance = Math.Max(0, value); OnPropertyChanged(); } }
        public string NewExerciseUnit { get => _newExerciseUnit; set { _newExerciseUnit = value; OnPropertyChanged(); } }
        public ExerciseType NewExerciseType { get => _newExerciseType; set { _newExerciseType = value; OnPropertyChanged(nameof(IsStrengthExercise)); OnPropertyChanged(nameof(IsCardioExercise)); OnPropertyChanged(nameof(IsTimeExercise)); OnPropertyChanged(nameof(IsRunningExercise)); OnPropertyChanged(nameof(IsSprintingExercise)); OnPropertyChanged(nameof(IsSledSprintExercise)); } }
        public string ExerciseSearchText { get => _exerciseSearchText; set { _exerciseSearchText = value; OnPropertyChanged(); FilterCommonExercises(); ShowExerciseDropdown = true; OnPropertyChanged(nameof(CanAddNewExercise)); NewExerciseName = value; } }
        public bool ShowExerciseDropdown { get => _showExerciseDropdown; set { _showExerciseDropdown = value; OnPropertyChanged(); } }
        public bool CanAddNewExercise => !string.IsNullOrWhiteSpace(ExerciseSearchText) && !_allCommonExercises.Any(e => e.Name.Equals(ExerciseSearchText, StringComparison.OrdinalIgnoreCase));
        public bool IsStrengthExercise => NewExerciseType == ExerciseType.Strength;
        public bool IsCardioExercise => NewExerciseType == ExerciseType.Cardio;
        public bool IsTimeExercise => NewExerciseType == ExerciseType.Time;
        public bool IsRunningExercise => NewExerciseType == ExerciseType.Running;
        public bool IsSprintingExercise => NewExerciseType == ExerciseType.Sprinting;
        public bool IsSledSprintExercise => NewExerciseType == ExerciseType.SledSprint;
        public ObservableCollection<ExerciseSet> NewExerciseSetsList { get => _newExerciseSetsList; set { _newExerciseSetsList = value; OnPropertyChanged(); } }
        public ObservableCollection<RunningSet> NewRunningSetsList { get => _newRunningSetsList; set { _newRunningSetsList = value; OnPropertyChanged(); } }
        public int NewSprintSeconds { get => _newSprintSeconds; set { _newSprintSeconds = Math.Max(0, Math.Min(59, value)); OnPropertyChanged(); OnPropertyChanged(nameof(NewSprintTimeDisplay)); } }
        public int NewSprintHundredths { get => _newSprintHundredths; set { _newSprintHundredths = Math.Max(0, Math.Min(99, value)); OnPropertyChanged(); OnPropertyChanged(nameof(NewSprintTimeDisplay)); } }
        public string NewSprintTimeDisplay => $"{NewSprintSeconds:D2}.{NewSprintHundredths:D2}s";
        private readonly ObservableCollection<CommonExercise> _allCommonExercises = new();
        public ObservableCollection<CommonExercise> CommonExercises { get; } = new();
        public int FilteredExercisesCount => CommonExercises.Count;

        public ICommand AddExerciseCommand { get; }
        public ICommand SaveExerciseCommand { get; }
        public ICommand CancelAddExerciseCommand { get; }
        public ICommand DeleteExerciseCommand { get; }
        public ICommand EditExerciseCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand AddCustomExerciseCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand ExerciseEntryFocusedCommand { get; }
        public ICommand ExerciseEntryUnfocusedCommand { get; }
        public ICommand HideExerciseDropdownCommand { get; }
        public ICommand ToggleExerciseDropdownCommand { get; }
        public ICommand IncreaseRepsCommand { get; }
        public ICommand DecreaseRepsCommand { get; }
        public ICommand IncreaseWeightCommand { get; }
        public ICommand DecreaseWeightCommand { get; }
        public ICommand AddSetCommand { get; }
        public ICommand RemoveSetCommand { get; }
        public ICommand IncreaseDurationCommand { get; }
        public ICommand DecreaseDurationCommand { get; }
        public ICommand IncreaseDistanceCommand { get; }
        public ICommand DecreaseDistanceCommand { get; }
        public ICommand AddNewSetCommand { get; }
        public ICommand RemoveNewSetCommand { get; }
        public ICommand IncreaseNewRepsCommand { get; }
        public ICommand DecreaseNewRepsCommand { get; }
        public ICommand IncreaseNewWeightCommand { get; }
        public ICommand DecreaseNewWeightCommand { get; }
        public ICommand AddNewRunningSetCommand { get; }
        public ICommand RemoveNewRunningSetCommand { get; }
        public ICommand AddRunningSetCommand { get; }
        public ICommand RemoveRunningSetCommand { get; }
        public ICommand IncreaseSprintSecondsCommand { get; }
        public ICommand DecreaseSprintSecondsCommand { get; }
        public ICommand IncreaseSprintHundredthsCommand { get; }
        public ICommand DecreaseSprintHundredthsCommand { get; }
        public ICommand IncreaseRunningDistanceCommand { get; }
        public ICommand DecreaseRunningDistanceCommand { get; }
        public ICommand IncreaseRunningWeightCommand { get; }
        public ICommand DecreaseRunningWeightCommand { get; }
        public ICommand IncreaseNewSprintSecondsCommand { get; }
        public ICommand DecreaseNewSprintSecondsCommand { get; }
        public ICommand IncreaseNewSprintHundredthsCommand { get; }
        public ICommand DecreaseNewSprintHundredthsCommand { get; }
        public ICommand IncreaseNewRunningDistanceCommand { get; }
        public ICommand DecreaseNewRunningDistanceCommand { get; }
        public ICommand IncreaseNewRunningWeightCommand { get; }
        public ICommand DecreaseNewRunningWeightCommand { get; }
        public ICommand ToggleWarmupSetCommand { get; }
        public ICommand ToggleWarmupRunningSetCommand { get; }
        public ICommand ToggleNewWarmupSetCommand { get; }
        public ICommand ToggleNewWarmupRunningSetCommand { get; }
        public ICommand DragStartingCommand { get; }
        public ICommand DropCommand { get; }

        public event EventHandler? RequestBack;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDragStarting(object? set)
        {
            _draggedSet = set;
        }

        private void OnDrop(object? targetSet)
        {
            if (_draggedSet == null || targetSet == null || _draggedSet == targetSet)
                return;

            var exercise = TrainingSession.Exercises.FirstOrDefault(ex =>
                (ex.ExerciseSets.Contains(_draggedSet as ExerciseSet) && ex.ExerciseSets.Contains(targetSet as ExerciseSet)) ||
                (ex.RunningSets.Contains(_draggedSet as RunningSet) && ex.RunningSets.Contains(targetSet as RunningSet))
            );

            if (exercise == null)
                return;

            if (_draggedSet is ExerciseSet draggedStrengthSet && targetSet is ExerciseSet targetStrengthSet)
            {
                var collection = exercise.ExerciseSets;
                int oldIndex = collection.IndexOf(draggedStrengthSet);
                int newIndex = collection.IndexOf(targetStrengthSet);

                if (oldIndex != -1 && newIndex != -1)
                {
                    collection.Move(oldIndex, newIndex);
                    RenumberExerciseSets(exercise);
                }
            }
            else if (_draggedSet is RunningSet draggedRunningSet && targetSet is RunningSet targetRunningSet)
            {
                var collection = exercise.RunningSets;
                int oldIndex = collection.IndexOf(draggedRunningSet);
                int newIndex = collection.IndexOf(targetRunningSet);

                if (oldIndex != -1 && newIndex != -1)
                {
                    collection.Move(oldIndex, newIndex);
                    RenumberRunningSets(exercise);
                }
            }
            _draggedSet = null;
        }

        private void AddExercise()
        {
            ResetNewExerciseProperties();
            IsAddingExercise = true;
        }

        private void SaveExercise()
        {
            if (!string.IsNullOrWhiteSpace(NewExerciseName))
            {
                var exercise = new Exercise
                {
                    Name = NewExerciseName,
                    Description = NewExerciseDescription,
                    Sets = NewExerciseType == ExerciseType.Strength ? NewExerciseSetsList.Count :
                           (NewExerciseType == ExerciseType.Running || NewExerciseType == ExerciseType.Sprinting || NewExerciseType == ExerciseType.SledSprint) ? NewRunningSetsList.Count :
                           NewExerciseSets,
                    Reps = NewExerciseReps,
                    Weight = NewExerciseWeight,
                    Duration = NewExerciseDuration,
                    Distance = NewExerciseDistance,
                    Unit = NewExerciseUnit,
                    ExerciseType = NewExerciseType,
                    SprintSeconds = NewSprintSeconds,
                    SprintHundredths = NewSprintHundredths
                };

                if (NewExerciseType == ExerciseType.Strength)
                {
                    foreach (var set in NewExerciseSetsList)
                    {
                        exercise.ExerciseSets.Add(new ExerciseSet
                        {
                            SetNumber = set.SetNumber,
                            Reps = set.Reps,
                            Weight = set.Weight,
                            Unit = set.Unit,
                            IsWarmupSet = set.IsWarmupSet
                        });
                    }
                }
                else if (NewExerciseType == ExerciseType.Running || NewExerciseType == ExerciseType.Sprinting || NewExerciseType == ExerciseType.SledSprint)
                {
                    foreach (var set in NewRunningSetsList)
                    {
                        exercise.RunningSets.Add(new RunningSet
                        {
                            SetNumber = set.SetNumber,
                            Duration = set.Duration,
                            Distance = set.Distance,
                            Weight = set.Weight,
                            SprintSeconds = set.SprintSeconds,
                            SprintHundredths = set.SprintHundredths,
                            IsWarmupSet = set.IsWarmupSet
                        });
                    }
                }

                TrainingSession.Exercises.Add(exercise);
            }
            IsAddingExercise = false;
        }

        private void CancelAddExercise()
        {
            IsAddingExercise = false;
        }

        private void DeleteExercise(Exercise? exercise)
        {
            if (exercise != null && TrainingSession.Exercises.Contains(exercise))
            {
                TrainingSession.Exercises.Remove(exercise);
            }
        }

        private void EditExercise(Exercise? exercise)
        {
            if (exercise != null)
            {
                NewExerciseName = exercise.Name;
                NewExerciseDescription = exercise.Description;
                NewExerciseSets = exercise.Sets;
                NewExerciseReps = exercise.Reps;
                NewExerciseWeight = exercise.Weight;
                NewExerciseDuration = exercise.Duration;
                NewExerciseDistance = exercise.Distance;
                NewExerciseUnit = exercise.Unit;
                NewExerciseType = exercise.ExerciseType;

                TrainingSession.Exercises.Remove(exercise);
                IsAddingExercise = true;
            }
        }

        private void GoBack()
        {
            RequestBack?.Invoke(this, EventArgs.Empty);
        }

        private void ResetNewExerciseProperties()
        {
            NewExerciseName = string.Empty;
            NewExerciseDescription = string.Empty;
            NewExerciseSets = 1;
            NewExerciseReps = 1;
            NewExerciseWeight = 0;
            NewExerciseDuration = TimeSpan.Zero;
            NewExerciseDistance = 0;
            NewExerciseUnit = "kg";
            NewExerciseType = ExerciseType.Strength;
            ExerciseSearchText = string.Empty;
            ShowExerciseDropdown = false;
            _newExerciseSetsList.Clear();
            _newRunningSetsList.Clear();
            NewSprintSeconds = 0;
            NewSprintHundredths = 0;
        }

        private void InitializeNewExerciseSets()
        {
            _newExerciseSetsList.Clear();
            for (int i = 1; i <= 3; i++)
            {
                _newExerciseSetsList.Add(new ExerciseSet
                {
                    SetNumber = i,
                    Reps = NewExerciseReps,
                    Weight = NewExerciseWeight,
                    Unit = NewExerciseUnit
                });
            }
        }

        private void InitializeNewRunningSets()
        {
            _newRunningSetsList.Clear();
            for (int i = 1; i <= 1; i++)
            {
                _newRunningSetsList.Add(new RunningSet
                {
                    SetNumber = i,
                    Duration = NewExerciseDuration,
                    Distance = NewExerciseDistance
                });
            }
        }

        private void InitializeNewSprintSets()
        {
            _newRunningSetsList.Clear();
            for (int i = 1; i <= 3; i++)
            {
                _newRunningSetsList.Add(new RunningSet
                {
                    SetNumber = i,
                    SprintSeconds = NewSprintSeconds,
                    SprintHundredths = NewSprintHundredths,
                    Distance = NewExerciseDistance
                });
            }
        }

        private void InitializeNewSledSprintSets()
        {
            _newRunningSetsList.Clear();
            for (int i = 1; i <= 3; i++)
            {
                _newRunningSetsList.Add(new RunningSet
                {
                    SetNumber = i,
                    SprintSeconds = NewSprintSeconds,
                    SprintHundredths = NewSprintHundredths,
                    Distance = NewExerciseDistance,
                    Weight = NewExerciseWeight
                });
            }
        }

        private void AddNewSet()
        {
            var newSet = new ExerciseSet
            {
                SetNumber = _newExerciseSetsList.Count + 1,
                Reps = _newExerciseSetsList.LastOrDefault()?.Reps ?? NewExerciseReps,
                Weight = _newExerciseSetsList.LastOrDefault()?.Weight ?? NewExerciseWeight,
                Unit = NewExerciseUnit
            };
            _newExerciseSetsList.Add(newSet);
        }

        private void RemoveNewSet(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null && _newExerciseSetsList.Count > 1)
            {
                _newExerciseSetsList.Remove(exerciseSet);
                for (int i = 0; i < _newExerciseSetsList.Count; i++)
                {
                    _newExerciseSetsList[i].SetNumber = i + 1;
                }
            }
        }

        private void IncreaseNewReps(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null) exerciseSet.Reps++;
        }

        private void DecreaseNewReps(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null && exerciseSet.Reps > 1) exerciseSet.Reps--;
        }

        private void IncreaseNewWeight(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null) exerciseSet.Weight += 1.25;
        }

        private void DecreaseNewWeight(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null && exerciseSet.Weight > 0) exerciseSet.Weight = Math.Max(0, exerciseSet.Weight - 1.25);
        }

        private void AddNewRunningSet()
        {
            var newSet = new RunningSet
            {
                SetNumber = _newRunningSetsList.Count + 1,
                Duration = _newRunningSetsList.LastOrDefault()?.Duration ?? NewExerciseDuration,
                Distance = _newRunningSetsList.LastOrDefault()?.Distance ?? NewExerciseDistance,
                Weight = _newRunningSetsList.LastOrDefault()?.Weight ?? NewExerciseWeight,
                SprintSeconds = _newRunningSetsList.LastOrDefault()?.SprintSeconds ?? NewSprintSeconds,
                SprintHundredths = _newRunningSetsList.LastOrDefault()?.SprintHundredths ?? NewSprintHundredths
            };
            _newRunningSetsList.Add(newSet);
        }

        private void RemoveNewRunningSet(RunningSet? runningSet)
        {
            if (runningSet != null && _newRunningSetsList.Count > 1)
            {
                _newRunningSetsList.Remove(runningSet);
                for (int i = 0; i < _newRunningSetsList.Count; i++)
                {
                    _newRunningSetsList[i].SetNumber = i + 1;
                }
            }
        }

        private void IncreaseNewSprintSeconds(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.SprintSeconds < 59) runningSet.SprintSeconds++;
        }

        private void DecreaseNewSprintSeconds(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.SprintSeconds > 0) runningSet.SprintSeconds--;
        }

        private void IncreaseNewSprintHundredths(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                runningSet.SprintHundredths = (runningSet.SprintHundredths + 1) % 100;
                if (runningSet.SprintHundredths == 0 && runningSet.SprintSeconds < 59) runningSet.SprintSeconds++;
                else if (runningSet.SprintHundredths == 0 && runningSet.SprintSeconds >= 59) runningSet.SprintHundredths = 99;
            }
        }

        private void DecreaseNewSprintHundredths(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                if (runningSet.SprintHundredths > 0) runningSet.SprintHundredths--;
                else if (runningSet.SprintSeconds > 0) { runningSet.SprintSeconds--; runningSet.SprintHundredths = 99; }
            }
        }

        private void IncreaseNewRunningDistance(RunningSet? runningSet)
        {
            if (runningSet != null) runningSet.Distance += NewExerciseType == ExerciseType.SledSprint ? 5 : 100;
        }

        private void DecreaseNewRunningDistance(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.Distance > 0)
            {
                var decrement = NewExerciseType == ExerciseType.SledSprint ? 5 : 100;
                runningSet.Distance = Math.Max(0, runningSet.Distance - decrement);
            }
        }

        private void IncreaseNewRunningWeight(RunningSet? runningSet)
        {
            if (runningSet != null) runningSet.Weight += 1.25;
        }

        private void DecreaseNewRunningWeight(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.Weight > 0) runningSet.Weight = Math.Max(0, runningSet.Weight - 1.25);
        }

        private void AddRunningSet(Exercise? exercise)
        {
            if (exercise != null)
            {
                var newSet = new RunningSet
                {
                    SetNumber = exercise.RunningSets.Count + 1,
                    Duration = exercise.RunningSets.LastOrDefault()?.Duration ?? exercise.Duration,
                    Distance = exercise.RunningSets.LastOrDefault()?.Distance ?? exercise.Distance,
                    Weight = exercise.RunningSets.LastOrDefault()?.Weight ?? exercise.Weight,
                    SprintSeconds = exercise.RunningSets.LastOrDefault()?.SprintSeconds ?? exercise.SprintSeconds,
                    SprintHundredths = exercise.RunningSets.LastOrDefault()?.SprintHundredths ?? exercise.SprintHundredths
                };
                exercise.RunningSets.Add(newSet);
            }
        }

        private void RemoveRunningSet(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                var exercise = TrainingSession.Exercises.FirstOrDefault(e => e.RunningSets.Contains(runningSet));
                if (exercise != null && exercise.RunningSets.Count > 1)
                {
                    exercise.RunningSets.Remove(runningSet);
                    for (int i = 0; i < exercise.RunningSets.Count; i++) { exercise.RunningSets[i].SetNumber = i + 1; }
                }
            }
        }

        private void IncreaseSprintSeconds(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.SprintSeconds < 59) runningSet.SprintSeconds++;
        }

        private void DecreaseSprintSeconds(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.SprintSeconds > 0) runningSet.SprintSeconds--;
        }

        private void IncreaseSprintHundredths(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                runningSet.SprintHundredths = (runningSet.SprintHundredths + 1) % 100;
                if (runningSet.SprintHundredths == 0 && runningSet.SprintSeconds < 59) runningSet.SprintSeconds++;
                else if (runningSet.SprintHundredths == 0 && runningSet.SprintSeconds >= 59) runningSet.SprintHundredths = 99;
            }
        }

        private void DecreaseSprintHundredths(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                if (runningSet.SprintHundredths > 0) runningSet.SprintHundredths--;
                else if (runningSet.SprintSeconds > 0) { runningSet.SprintSeconds--; runningSet.SprintHundredths = 99; }
            }
        }

        private void IncreaseRunningDistance(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                var exercise = TrainingSession.Exercises.FirstOrDefault(e => e.RunningSets.Contains(runningSet));
                var increment = exercise?.ExerciseType == ExerciseType.SledSprint ? 5 : 100;
                runningSet.Distance += increment;
            }
        }

        private void DecreaseRunningDistance(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.Distance > 0)
            {
                var exercise = TrainingSession.Exercises.FirstOrDefault(e => e.RunningSets.Contains(runningSet));
                var decrement = exercise?.ExerciseType == ExerciseType.SledSprint ? 5 : 100;
                runningSet.Distance = Math.Max(0, runningSet.Distance - decrement);
            }
        }

        private void IncreaseRunningWeight(RunningSet? runningSet)
        {
            if (runningSet != null) runningSet.Weight += 1.25;
        }

        private void DecreaseRunningWeight(RunningSet? runningSet)
        {
            if (runningSet != null && runningSet.Weight > 0) runningSet.Weight = Math.Max(0, runningSet.Weight - 1.25);
        }

        private void InitializeCommonExercises()
        {
            _allCommonExercises.Add(new CommonExercise("Bench Press", ExerciseType.Strength, "Chest exercise with barbell or dumbbells"));
            _allCommonExercises.Add(new CommonExercise("Squat", ExerciseType.Strength, "Leg exercise with barbell"));
            _allCommonExercises.Add(new CommonExercise("Deadlift", ExerciseType.Strength, "Full body exercise with barbell"));
            _allCommonExercises.Add(new CommonExercise("Running", ExerciseType.Running, "Distance running with time tracking (hours:minutes:seconds)"));
            _allCommonExercises.Add(new CommonExercise("Jogging", ExerciseType.Running, "Light running with time tracking"));
            _allCommonExercises.Add(new CommonExercise("Sprint", ExerciseType.Sprinting, "High-intensity sprinting with seconds and hundredths"));
            _allCommonExercises.Add(new CommonExercise("100m Sprint", ExerciseType.Sprinting, "100 meter sprint timing"));
            _allCommonExercises.Add(new CommonExercise("Sled Sprint", ExerciseType.SledSprint, "Weighted sled sprint with time, distance and weight"));
            _allCommonExercises.Add(new CommonExercise("Prowler Push", ExerciseType.SledSprint, "Prowler sled push exercise"));
            _allCommonExercises.Add(new CommonExercise("Cycling", ExerciseType.Cardio, "Bike riding exercise"));
            _allCommonExercises.Add(new CommonExercise("Swimming", ExerciseType.Cardio, "Full body cardio in water"));
            FilterCommonExercises();
        }

        private void FilterCommonExercises()
        {
            CommonExercises.Clear();
            var filtered = string.IsNullOrWhiteSpace(ExerciseSearchText)
                ? _allCommonExercises
                : _allCommonExercises.Where(e =>
                    e.Name.ToLowerInvariant().Contains(ExerciseSearchText.ToLowerInvariant()) ||
                    e.Description.ToLowerInvariant().Contains(ExerciseSearchText.ToLowerInvariant()));

            foreach (var exercise in filtered)
            {
                CommonExercises.Add(exercise);
            }
            OnPropertyChanged(nameof(FilteredExercisesCount));
            OnPropertyChanged(nameof(CanAddNewExercise));
        }

        private void SelectExercise(string? selectedExerciseName)
        {
            if (!string.IsNullOrEmpty(selectedExerciseName))
            {
                var selectedExercise = _allCommonExercises.FirstOrDefault(e => e.Name == selectedExerciseName);
                if (selectedExercise != null)
                {
                    NewExerciseName = selectedExercise.Name;
                    NewExerciseType = selectedExercise.Type;
                    ExerciseSearchText = selectedExercise.Name;

                    switch (selectedExercise.Type)
                    {
                        case ExerciseType.Strength:
                            NewExerciseSets = 3; NewExerciseReps = 10; NewExerciseWeight = 0; NewExerciseUnit = "kg";
                            InitializeNewExerciseSets();
                            break;
                        case ExerciseType.Cardio:
                            NewExerciseDuration = TimeSpan.FromMinutes(30); NewExerciseDistance = 5.0;
                            break;
                        case ExerciseType.Time:
                            NewExerciseDuration = TimeSpan.FromMinutes(15);
                            break;
                        case ExerciseType.Running:
                            NewExerciseDuration = TimeSpan.FromMinutes(30); NewExerciseDistance = 5.0;
                            InitializeNewRunningSets();
                            break;
                        case ExerciseType.Sprinting:
                            NewExerciseDistance = 100; NewSprintSeconds = 12; NewSprintHundredths = 50;
                            InitializeNewSprintSets();
                            break;
                        case ExerciseType.SledSprint:
                            NewExerciseDistance = 20; NewExerciseWeight = 20; NewExerciseUnit = "kg"; NewSprintSeconds = 8; NewSprintHundredths = 0;
                            InitializeNewSledSprintSets();
                            break;
                    }
                }
                ShowExerciseDropdown = false;
            }
        }

        private void AddCustomExercise() { }
        private void OnExerciseEntryFocused() { ShowExerciseDropdown = true; FilterCommonExercises(); }
        private void OnExerciseEntryUnfocused() { ShowExerciseDropdown = false; }
        private void HideExerciseDropdown() { ShowExerciseDropdown = false; }
        private void ToggleExerciseDropdown() { ShowExerciseDropdown = !ShowExerciseDropdown; }
        private void IncreaseReps(ExerciseSet? exerciseSet) { if (exerciseSet != null) exerciseSet.Reps++; }
        private void DecreaseReps(ExerciseSet? exerciseSet) { if (exerciseSet != null && exerciseSet.Reps > 1) exerciseSet.Reps--; }
        private void IncreaseWeight(ExerciseSet? exerciseSet) { if (exerciseSet != null) exerciseSet.Weight += 1.25; }
        private void DecreaseWeight(ExerciseSet? exerciseSet) { if (exerciseSet != null && exerciseSet.Weight > 0) exerciseSet.Weight = Math.Max(0, exerciseSet.Weight - 1.25); }
        private void AddSet(Exercise? exercise)
        {
            if (exercise != null && exercise.ExerciseType == ExerciseType.Strength)
            {
                var nextSetNumber = exercise.ExerciseSets.Count(s => !s.IsWarmupSet) + 1;
                var newSet = new ExerciseSet
                {
                    SetNumber = nextSetNumber,
                    Reps = exercise.ExerciseSets.LastOrDefault()?.Reps ?? exercise.Reps,
                    Weight = exercise.ExerciseSets.LastOrDefault()?.Weight ?? exercise.Weight,
                    Unit = exercise.Unit,
                    IsWarmupSet = false
                };
                exercise.ExerciseSets.Add(newSet);
            }
        }

        private void RemoveSet(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null)
            {
                var exercise = TrainingSession.Exercises.FirstOrDefault(e => e.ExerciseSets.Contains(exerciseSet));
                if (exercise != null && exercise.ExerciseSets.Count > 1)
                {
                    exercise.ExerciseSets.Remove(exerciseSet);
                    RenumberExerciseSets(exercise);
                }
            }
        }

        private void IncreaseDuration(Exercise? exercise)
        {
            if (exercise != null) exercise.Duration = exercise.Duration.Add(TimeSpan.FromMinutes(5));
        }

        private void DecreaseDuration(Exercise? exercise)
        {
            if (exercise != null && exercise.Duration > TimeSpan.Zero)
            {
                var newDuration = exercise.Duration.Subtract(TimeSpan.FromMinutes(5));
                exercise.Duration = newDuration > TimeSpan.Zero ? newDuration : TimeSpan.Zero;
            }
        }

        private void IncreaseDistance(Exercise? exercise)
        {
            if (exercise != null)
            {
                var increment = exercise.ExerciseType == ExerciseType.SledSprint ? 5 :
                               exercise.ExerciseType == ExerciseType.Sprinting ? 100 : 1;
                exercise.Distance += increment;
            }
        }

        private void DecreaseDistance(Exercise? exercise)
        {
            if (exercise != null && exercise.Distance > 0)
            {
                var decrement = exercise.ExerciseType == ExerciseType.SledSprint ? 5 :
                               exercise.ExerciseType == ExerciseType.Sprinting ? 100 : 1;
                exercise.Distance = Math.Max(0, exercise.Distance - decrement);
            }
        }

        private void ToggleWarmupSet(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null)
            {
                var exercise = TrainingSession.Exercises.FirstOrDefault(e => e.ExerciseSets.Contains(exerciseSet));
                if (exercise != null)
                {
                    if (exerciseSet.IsWarmupSet)
                    {
                        exerciseSet.IsWarmupSet = false;
                        var allSets = exercise.ExerciseSets.OrderBy(s => s.SetNumber).ToList();
                        var clickedIndex = allSets.IndexOf(exerciseSet);
                        for (int i = clickedIndex + 1; i < allSets.Count; i++)
                        {
                            if (allSets[i].IsWarmupSet) allSets[i].IsWarmupSet = false;
                            else break;
                        }
                    }
                    else
                    {
                        var allSets = exercise.ExerciseSets.OrderBy(s => s.SetNumber).ToList();
                        var clickedIndex = allSets.IndexOf(exerciseSet);
                        for (int i = 0; i <= clickedIndex; i++) { allSets[i].IsWarmupSet = true; }
                    }
                    RenumberExerciseSets(exercise);
                }
            }
        }

        private void ToggleWarmupRunningSet(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                var exercise = TrainingSession.Exercises.FirstOrDefault(e => e.RunningSets.Contains(runningSet));
                if (exercise != null)
                {
                    if (runningSet.IsWarmupSet)
                    {
                        runningSet.IsWarmupSet = false;
                        var allSets = exercise.RunningSets.OrderBy(s => s.SetNumber).ToList();
                        var clickedIndex = allSets.IndexOf(runningSet);
                        for (int i = clickedIndex + 1; i < allSets.Count; i++)
                        {
                            if (allSets[i].IsWarmupSet) allSets[i].IsWarmupSet = false;
                            else break;
                        }
                    }
                    else
                    {
                        var allSets = exercise.RunningSets.OrderBy(s => s.SetNumber).ToList();
                        var clickedIndex = allSets.IndexOf(runningSet);
                        for (int i = 0; i <= clickedIndex; i++) { allSets[i].IsWarmupSet = true; }
                    }
                    RenumberRunningSets(exercise);
                }
            }
        }

        private void RenumberExerciseSets(Exercise exercise)
        {
            var nonWarmupSets = exercise.ExerciseSets.Where(s => !s.IsWarmupSet).OrderBy(s => s.SetNumber).ToList();
            for (int i = 0; i < nonWarmupSets.Count; i++) { nonWarmupSets[i].SetNumber = i + 1; }
            var warmupSets = exercise.ExerciseSets.Where(s => s.IsWarmupSet).ToList();
            foreach (var warmupSet in warmupSets) { warmupSet.SetNumber = 0; }
        }

        private void RenumberRunningSets(Exercise exercise)
        {
            var nonWarmupSets = exercise.RunningSets.Where(s => !s.IsWarmupSet).OrderBy(s => s.SetNumber).ToList();
            for (int i = 0; i < nonWarmupSets.Count; i++) { nonWarmupSets[i].SetNumber = i + 1; }
            var warmupSets = exercise.RunningSets.Where(s => s.IsWarmupSet).ToList();
            foreach (var warmupSet in warmupSets) { warmupSet.SetNumber = 0; }
        }

        private void ToggleNewWarmupSet(ExerciseSet? exerciseSet)
        {
            if (exerciseSet != null)
            {
                if (exerciseSet.IsWarmupSet)
                {
                    exerciseSet.IsWarmupSet = false;
                    var allSets = _newExerciseSetsList.OrderBy(s => s.SetNumber).ToList();
                    var clickedIndex = allSets.IndexOf(exerciseSet);
                    for (int i = clickedIndex + 1; i < allSets.Count; i++)
                    {
                        if (allSets[i].IsWarmupSet) allSets[i].IsWarmupSet = false;
                        else break;
                    }
                }
                else
                {
                    var allSets = _newExerciseSetsList.OrderBy(s => s.SetNumber).ToList();
                    var clickedIndex = allSets.IndexOf(exerciseSet);
                    for (int i = 0; i <= clickedIndex; i++) { allSets[i].IsWarmupSet = true; }
                }
                RenumberNewExerciseSets();
            }
        }

        private void ToggleNewWarmupRunningSet(RunningSet? runningSet)
        {
            if (runningSet != null)
            {
                if (runningSet.IsWarmupSet)
                {
                    runningSet.IsWarmupSet = false;
                    var allSets = _newRunningSetsList.OrderBy(s => s.SetNumber).ToList();
                    var clickedIndex = allSets.IndexOf(runningSet);
                    for (int i = clickedIndex + 1; i < allSets.Count; i++)
                    {
                        if (allSets[i].IsWarmupSet) allSets[i].IsWarmupSet = false;
                        else break;
                    }
                }
                else
                {
                    var allSets = _newRunningSetsList.OrderBy(s => s.SetNumber).ToList();
                    var clickedIndex = allSets.IndexOf(runningSet);
                    for (int i = 0; i <= clickedIndex; i++) { allSets[i].IsWarmupSet = true; }
                }
                RenumberNewRunningSets();
            }
        }

        private void RenumberNewExerciseSets()
        {
            var nonWarmupSets = _newExerciseSetsList.Where(s => !s.IsWarmupSet).OrderBy(s => s.SetNumber).ToList();
            for (int i = 0; i < nonWarmupSets.Count; i++) { nonWarmupSets[i].SetNumber = i + 1; }
            var warmupSets = _newExerciseSetsList.Where(s => s.IsWarmupSet).ToList();
            foreach (var warmupSet in warmupSets) { warmupSet.SetNumber = 0; }
        }

        private void RenumberNewRunningSets()
        {
            var nonWarmupSets = _newRunningSetsList.Where(s => !s.IsWarmupSet).OrderBy(s => s.SetNumber).ToList();
            for (int i = 0; i < nonWarmupSets.Count; i++)
            {
                nonWarmupSets[i].SetNumber = i + 1;
            }
            var warmupSets = _newRunningSetsList.Where(s => s.IsWarmupSet).ToList();
            foreach (var warmupSet in warmupSets)
            {
                warmupSet.SetNumber = 0;
            }
        }
    }

    // Helper class for common exercises
    public class CommonExercise
    {
        public CommonExercise(string name, ExerciseType type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }

        public string Name { get; }
        public ExerciseType Type { get; }
        public string Description { get; }
    }
}