using SprintTrack.ViewModels;
using SprintTrack.Models;
using System.Collections.Concurrent;
using DragEventArgs = SprintTrack.ViewModels.DragEventArgs;
using MauiDragEventArgs = Microsoft.Maui.Controls.DragEventArgs;

namespace SprintTrack;

public partial class TrainingSessionDetailPage : ContentPage
{
    private TrainingSessionDetailViewModel? _viewModel;
    
    // Enhanced animation state tracking for CollectionView compatibility
    private readonly ConcurrentDictionary<object, WeakReference<Frame>> _frameCache = new();
    private readonly Dictionary<object, (double OriginalTranslationY, bool IsAnimating)> _itemStates = new();
    private object? _draggedItem;
    private readonly Dictionary<object, Frame> _activeFrames = new();
    
    private const uint AnimationDuration = 250;
    private const double LiftScale = 1.05;
    private const double TranslateDistance = 60;

    public TrainingSessionDetailPage(TrainingSession trainingSession)
    {
        InitializeComponent();
        
        _viewModel = new TrainingSessionDetailViewModel(trainingSession);
        
        // Subscribe to drag and drop events for animations
        _viewModel.DragStarted += OnDragStarted;
        _viewModel.DragCompleted += OnDragCompleted;
        _viewModel.DragOver += OnViewModelDragOver;
        _viewModel.DragLeave += OnViewModelDragLeave;
        
        _viewModel.RequestBack += OnRequestBack;
        BindingContext = _viewModel;
        
        // Hide the Shell tab bar when this page is displayed
        Shell.SetTabBarIsVisible(this, false);
    }

    private async void OnRequestBack(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    #region Drag and Drop Animation Event Handlers

    private async void OnDragStarted(object? sender, DragEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"?? Drag started for item: Set {GetSetNumber(e.DraggedItem)}");
        
        if (e.DraggedItem == null) return;

        _draggedItem = e.DraggedItem;
        
        // Try to get the frame from our active frames registry
        if (_activeFrames.TryGetValue(e.DraggedItem, out var frame))
        {
            System.Diagnostics.Debug.WriteLine($"? Found frame for dragged item in active registry");
            
            // Store original state
            if (!_itemStates.ContainsKey(e.DraggedItem))
            {
                _itemStates[e.DraggedItem] = (frame.TranslationY, false);
            }
            
            // Animate the lift effect
            await Task.WhenAll(
                frame.ScaleTo(LiftScale, AnimationDuration, Easing.CubicOut),
                AnimateBackgroundColor(frame, Color.FromRgba(0.9f, 0.95f, 1.0f, 1.0f), AnimationDuration)
            );
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"? Could not find frame for dragged item in active registry");
        }
    }

    private async void OnDragCompleted(object? sender, DragEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"?? Drag completed, resetting all animations");
        
        var resetTasks = new List<Task>();

        // Reset all animated frames
        foreach (var kvp in _activeFrames.ToList())
        {
            var item = kvp.Key;
            var frame = kvp.Value;
            
            // Reset dragged item
            if (item == _draggedItem)
            {
                resetTasks.Add(frame.ScaleTo(1.0, AnimationDuration, Easing.CubicOut));
                resetTasks.Add(AnimateBackgroundColor(frame, Colors.White, AnimationDuration));
            }
            
            // Reset translated items
            if (_itemStates.TryGetValue(item, out var state) && state.IsAnimating)
            {
                resetTasks.Add(frame.TranslateTo(0, state.OriginalTranslationY, AnimationDuration, Easing.CubicOut));
                _itemStates[item] = (state.OriginalTranslationY, false);
            }
        }

        if (resetTasks.Any())
        {
            await Task.WhenAll(resetTasks);
        }

        // Clear drag state
        _draggedItem = null;
        
        // Clean up completed animations from state tracking
        var itemsToRemove = _itemStates.Where(kvp => !kvp.Value.IsAnimating).Select(kvp => kvp.Key).ToList();
        foreach (var item in itemsToRemove)
        {
            _itemStates.Remove(item);
        }
    }

    private async void OnViewModelDragOver(object? sender, DragOverEventArgs e)
    {
        if (e.TargetItem == null || e.DraggedItem == null || e.TargetItem == e.DraggedItem) return;

        System.Diagnostics.Debug.WriteLine($"?? Drag over target: Set {GetSetNumber(e.TargetItem)}");

        // Check if we have the target frame in our active registry
        if (_activeFrames.TryGetValue(e.TargetItem, out var targetFrame))
        {
            System.Diagnostics.Debug.WriteLine($"? Found target frame in active registry, animating translation");
            
            // Store original state if not already stored
            if (!_itemStates.ContainsKey(e.TargetItem))
            {
                _itemStates[e.TargetItem] = (targetFrame.TranslationY, false);
            }

            // Only animate if not already animating
            var currentState = _itemStates[e.TargetItem];
            if (!currentState.IsAnimating)
            {
                _itemStates[e.TargetItem] = (currentState.OriginalTranslationY, true);
                
                // Determine direction and animate
                var translateDirection = DetermineTranslateDirection(e.DraggedItem, e.TargetItem);
                var translateY = currentState.OriginalTranslationY + (translateDirection * TranslateDistance);

                // Animate the translation
                await targetFrame.TranslateTo(0, translateY, AnimationDuration, Easing.CubicOut);
                
                System.Diagnostics.Debug.WriteLine($"? Animated target frame to Y: {translateY}");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"? Could not find target frame for item: Set {GetSetNumber(e.TargetItem)}");
        }
    }

    private async void OnViewModelDragLeave(object? sender, DragEventArgs e)
    {
        if (e.TargetItem == null) return;

        System.Diagnostics.Debug.WriteLine($"?? Drag leave target: Set {GetSetNumber(e.TargetItem)}");

        // Check if we have the target frame and it's currently animated
        if (_activeFrames.TryGetValue(e.TargetItem, out var targetFrame) && 
            _itemStates.TryGetValue(e.TargetItem, out var state) && 
            state.IsAnimating)
        {
            System.Diagnostics.Debug.WriteLine($"? Resetting target frame to original position");
            
            // Animate back to original position
            await targetFrame.TranslateTo(0, state.OriginalTranslationY, AnimationDuration, Easing.CubicOut);
            _itemStates[e.TargetItem] = (state.OriginalTranslationY, false);
        }
    }

    #endregion

    #region XAML Event Handlers - Frame Registration

    private void OnFrameLoaded(object? sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext != null)
        {
            RegisterFrame(frame.BindingContext, frame);
        }
    }

    private void OnDragOver(object? sender, MauiDragEventArgs e)
    {
        if (sender is DropGestureRecognizer dropRecognizer && 
            dropRecognizer.Parent is Frame frame &&
            frame.BindingContext != null)
        {
            // Register this frame in our active frames registry
            RegisterFrame(frame.BindingContext, frame);
            
            if (_viewModel?.DragOverCommand?.CanExecute(null) == true)
            {
                var draggedItem = GetCurrentDraggedItem();
                if (draggedItem != null)
                {
                    var args = new DragOverEventArgs
                    {
                        DraggedItem = draggedItem,
                        TargetItem = frame.BindingContext,
                        IsComingFromAbove = DetermineTranslateDirection(draggedItem, frame.BindingContext) > 0
                    };
                    
                    _viewModel.DragOverCommand.Execute(args);
                }
            }
        }
    }

    private void OnDragLeave(object? sender, MauiDragEventArgs e)
    {
        if (sender is DropGestureRecognizer dropRecognizer && 
            dropRecognizer.Parent is Frame frame &&
            frame.BindingContext != null &&
            _viewModel?.DragLeaveCommand?.CanExecute(null) == true)
        {
            _viewModel.DragLeaveCommand.Execute(frame.BindingContext);
        }
    }

    #endregion

    #region Frame Registration System

    private void RegisterFrame(object item, Frame frame)
    {
        if (item != null)
        {
            _activeFrames[item] = frame;
            
            // Also register for drag start if this is the dragged item
            if (item == GetCurrentDraggedItem())
            {
                _draggedItem = item;
            }
            
            System.Diagnostics.Debug.WriteLine($"?? Registered frame for Set {GetSetNumber(item)}");
        }
    }

    private void UnregisterFrame(object item)
    {
        if (item != null && _activeFrames.ContainsKey(item))
        {
            _activeFrames.Remove(item);
            System.Diagnostics.Debug.WriteLine($"??? Unregistered frame for Set {GetSetNumber(item)}");
        }
    }

    #endregion

    #region Helper Methods

    private object? GetCurrentDraggedItem()
    {
        return _viewModel?.GetType()
            .GetField("_draggedSet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_viewModel);
    }

    private int DetermineTranslateDirection(object draggedItem, object targetItem)
    {
        var draggedSetNumber = GetSetNumber(draggedItem);
        var targetSetNumber = GetSetNumber(targetItem);

        if (draggedSetNumber < targetSetNumber)
        {
            return 1; // Dragged item is above target, move target down
        }
        else if (draggedSetNumber > targetSetNumber)
        {
            return -1; // Dragged item is below target, move target up
        }
        
        return 1; // Default to moving down
    }

    private int GetSetNumber(object? item)
    {
        return item switch
        {
            ExerciseSet exerciseSet => exerciseSet.SetNumber,
            RunningSet runningSet => runningSet.SetNumber,
            _ => 0
        };
    }

    private async Task AnimateBackgroundColor(Frame frame, Color targetColor, uint duration)
    {
        var originalColor = frame.BackgroundColor;
        
        var animation = new Animation(
            v => frame.BackgroundColor = Color.FromRgba(
                originalColor.Red + (targetColor.Red - originalColor.Red) * (float)v,
                originalColor.Green + (targetColor.Green - originalColor.Green) * (float)v,
                originalColor.Blue + (targetColor.Blue - originalColor.Blue) * (float)v,
                originalColor.Alpha + (targetColor.Alpha - originalColor.Alpha) * (float)v
            ),
            0, 1
        );

        animation.Commit(frame, "BackgroundColorAnimation", 16, duration, Easing.CubicOut);
        await Task.Delay((int)duration);
    }

    #endregion

    #region Existing Event Handlers

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
        Shell.SetTabBarIsVisible(this, false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.SetTabBarIsVisible(this, true);
        
        // Clean up event handlers and caches
        if (_viewModel != null)
        {
            _viewModel.DragStarted -= OnDragStarted;
            _viewModel.DragCompleted -= OnDragCompleted;
            _viewModel.DragOver -= OnViewModelDragOver;
            _viewModel.DragLeave -= OnViewModelDragLeave;
        }
        
        _frameCache.Clear();
        _itemStates.Clear();
        _activeFrames.Clear();
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