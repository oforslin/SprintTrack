using SprintTrack.ViewModels;
using SprintTrack.Models;

namespace SprintTrack;

public partial class TrainingSessionDetailPage : ContentPage
{
    private TrainingSessionDetailViewModel? _viewModel;

    public TrainingSessionDetailPage(TrainingSession trainingSession)
    {
        InitializeComponent();
        
        _viewModel = new TrainingSessionDetailViewModel(trainingSession);
        _viewModel.RequestBack += OnRequestBack;
        BindingContext = _viewModel;
        
        // Hide the Shell tab bar when this page is displayed
        Shell.SetTabBarIsVisible(this, false);
        
        // Debug: Print training session information
        System.Diagnostics.Debug.WriteLine($"?? TrainingSessionDetailPage initialized for: {trainingSession.DisplayName}");
        System.Diagnostics.Debug.WriteLine($"?? Training Session ID: {trainingSession.Id}");
        System.Diagnostics.Debug.WriteLine($"?? Exercise count: {trainingSession.Exercises?.Count ?? 0}");
        
        if (trainingSession.Exercises != null && trainingSession.Exercises.Any())
        {
            foreach (var exercise in trainingSession.Exercises)
            {
                System.Diagnostics.Debug.WriteLine($"?? Exercise: {exercise.Name} (Type: {exercise.ExerciseType})");
                System.Diagnostics.Debug.WriteLine($"??   - ExerciseSets count: {exercise.ExerciseSets?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"??   - RunningSets count: {exercise.RunningSets?.Count ?? 0}");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("?? No exercises found in training session");
        }
    }

    private async void OnRequestBack(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    #region Simple Event Handlers

    private void OnExerciseEntryFocused(object? sender, FocusEventArgs e)
    {
        if (BindingContext is TrainingSessionDetailViewModel viewModel)
        {
            viewModel.ExerciseEntryFocusedCommand.Execute(null);
        }
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        // Entry automatically loses focus, no additional code needed
    }

    private void OnTimeEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue))
        {
            // Validate seconds input to not exceed 59
            if (entry.Placeholder == "00" && entry.Parent is Grid parentGrid)
            {
                // Check if this is a seconds field by looking for surrounding time separators
                var gridChildren = parentGrid.Children.ToList();
                var entryIndex = gridChildren.IndexOf(entry);
                
                // If there's a separator before this entry, it might be seconds
                if (entryIndex > 0 && gridChildren[entryIndex - 1] is Label label && (label.Text == ":" || label.Text == "."))
                {
                    // This appears to be a seconds field, validate it
                    if (int.TryParse(e.NewTextValue, out int seconds) && seconds > 59)
                    {
                        entry.Text = "59";
                        return;
                    }
                }
            }

            // Auto-navigate when 2 digits are entered
            if (e.NewTextValue.Length >= 2)
            {
                // Find the next entry in the same container
                if (entry.Parent is Grid grid)
                {
                    var currentIndex = grid.Children.IndexOf(entry);
                    
                    // Look for the next Entry control
                    for (int i = currentIndex + 1; i < grid.Children.Count; i++)
                    {
                        if (grid.Children[i] is Entry nextEntry)
                        {
                            nextEntry.Focus();
                            break;
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Page Lifecycle

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTabBarIsVisible(this, false);
        
        // Debug: Check if exercises are still available when page appears
        if (_viewModel?.TrainingSession?.Exercises != null)
        {
            System.Diagnostics.Debug.WriteLine($"?? OnAppearing - Exercise count: {_viewModel.TrainingSession.Exercises.Count}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.SetTabBarIsVisible(this, true);
        
        // Clean up event handlers
        if (_viewModel != null)
        {
            _viewModel.RequestBack -= OnRequestBack;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PopAsync();
        });
        return true;
    }

    #endregion
}