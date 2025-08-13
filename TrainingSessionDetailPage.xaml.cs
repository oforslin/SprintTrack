using SprintTrack.ViewModels;
using SprintTrack.Models;

namespace SprintTrack;

public partial class TrainingSessionDetailPage : ContentPage
{
    public TrainingSessionDetailPage(TrainingSession trainingSession)
    {
        InitializeComponent();
        var viewModel = new TrainingSessionDetailViewModel(trainingSession);
        viewModel.RequestBack += OnRequestBack;
        BindingContext = viewModel;
        
        // Hide the Shell tab bar when this page is displayed
        Shell.SetTabBarIsVisible(this, false);
    }

    private async void OnRequestBack(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnExerciseEntryFocused(object? sender, FocusEventArgs e)
    {
        if (BindingContext is TrainingSessionDetailViewModel viewModel)
        {
            viewModel.ExerciseEntryFocusedCommand.Execute(null);
        }
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        // This method handles when an Entry loses focus
        // The Entry will automatically lose focus, no additional code needed
        // You can add additional logic here if needed
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ensure tab bar is hidden when page appears
        Shell.SetTabBarIsVisible(this, false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Show tab bar again when leaving this page
        Shell.SetTabBarIsVisible(this, true);
    }

    protected override bool OnBackButtonPressed()
    {
        // Handle hardware back button on Android
        // This ensures proper navigation back to the previous page
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PopAsync();
        });
        return true; // Prevent default back behavior
    }
}