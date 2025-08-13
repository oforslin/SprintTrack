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
    private readonly Dictionary<object, (double OriginalTranslationY, bool IsAnimating, double OriginalOpacity, Color OriginalBackgroundColor, double OriginalScale)> _itemStates = new();
    private object? _draggedItem;
    private readonly Dictionary<object, Frame> _activeFrames = new();
    
    // Collection of all CollectionViews that need cleanup
    private readonly List<CollectionView> _collectionViews = new();
    
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
        
        // Find and register all CollectionViews for cleanup purposes
        RegisterCollectionViews();
    }

    private async void OnRequestBack(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    #region CollectionView Management

    private void RegisterCollectionViews()
    {
        // Find all CollectionViews in the page for cleanup purposes
        _collectionViews.Clear();
        FindCollectionViewsRecursive(this);
        
        System.Diagnostics.Debug.WriteLine($"?? Registered {_collectionViews.Count} CollectionViews for cleanup");
    }

    private void FindCollectionViewsRecursive(Element element)
    {
        if (element is CollectionView collectionView)
        {
            _collectionViews.Add(collectionView);
        }
        
        if (element is Layout layout)
        {
            foreach (var child in layout.Children.OfType<Element>())
            {
                FindCollectionViewsRecursive(child);
            }
        }
        else if (element is ContentView contentView && contentView.Content != null)
        {
            FindCollectionViewsRecursive(contentView.Content);
        }
        else if (element is ScrollView scrollView && scrollView.Content != null)
        {
            FindCollectionViewsRecursive(scrollView.Content);
        }
    }

    private async Task ForceCollectionViewRefresh()
    {
        System.Diagnostics.Debug.WriteLine($"?? Forcing CollectionView refresh for {_collectionViews.Count} views");
        
        foreach (var collectionView in _collectionViews)
        {
            try
            {
                // Force layout update
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    collectionView.IsVisible = false;
                    await Task.Delay(1);
                    collectionView.IsVisible = true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error refreshing CollectionView: {ex.Message}");
            }
        }
    }

    #endregion

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
            
            // Store original state with all visual properties
            if (!_itemStates.ContainsKey(e.DraggedItem))
            {
                _itemStates[e.DraggedItem] = (
                    frame.TranslationY, 
                    false, 
                    frame.Opacity,
                    frame.BackgroundColor,
                    frame.Scale
                );
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
        System.Diagnostics.Debug.WriteLine($"?? Drag completed, resetting all animations and forcing UI refresh");
        
        try
        {
            // Comprehensive cleanup of ALL visual states
            await PerformComprehensiveCleanup();
            
            // Force CollectionView refresh to ensure UI synchronization
            await ForceCollectionViewRefresh();
            
            System.Diagnostics.Debug.WriteLine($"? Comprehensive cleanup completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error during comprehensive cleanup: {ex.Message}");
        }
    }

    private async Task PerformComprehensiveCleanup()
    {
        var resetTasks = new List<Task>();

        // Reset ALL frames to their original state, not just animated ones
        foreach (var kvp in _activeFrames.ToList())
        {
            var item = kvp.Key;
            var frame = kvp.Value;
            
            try
            {
                // Get original state or use defaults
                var originalState = _itemStates.TryGetValue(item, out var state) ? state : (0, false, 1, Colors.White, 1);
                
                // Reset ALL visual properties regardless of animation state
                frame.Opacity = originalState.OriginalOpacity;
                resetTasks.Add(frame.ScaleTo(originalState.OriginalScale, AnimationDuration / 2, Easing.CubicOut));
                resetTasks.Add(frame.TranslateTo(0, originalState.OriginalTranslationY, AnimationDuration / 2, Easing.CubicOut));
                resetTasks.Add(AnimateBackgroundColor(frame, originalState.OriginalBackgroundColor, AnimationDuration / 2));
                
                System.Diagnostics.Debug.WriteLine($"?? Reset frame for Set {GetSetNumber(item)} - Opacity: {originalState.OriginalOpacity}, Scale: {originalState.OriginalScale}, TranslationY: {originalState.OriginalTranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error resetting frame for item {GetSetNumber(item)}: {ex.Message}");
            }
        }

        // Wait for all reset animations to complete
        if (resetTasks.Any())
        {
            await Task.WhenAll(resetTasks);
        }

        // Clear all state tracking
        _draggedItem = null;
        _itemStates.Clear();
        
        System.Diagnostics.Debug.WriteLine($"?? Cleared all animation states and dragged item reference");
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
                _itemStates[e.TargetItem] = (
                    targetFrame.TranslationY, 
                    false, 
                    targetFrame.Opacity,
                    targetFrame.BackgroundColor,
                    targetFrame.Scale
                );
            }

            // Only animate if not already animating
            var currentState = _itemStates[e.TargetItem];
            if (!currentState.IsAnimating)
            {
                _itemStates[e.TargetItem] = (
                    currentState.OriginalTranslationY, 
                    true, 
                    currentState.OriginalOpacity,
                    currentState.OriginalBackgroundColor,
                    currentState.OriginalScale
                );
                
                // Determine direction and animate (VERTICAL ONLY)
                var translateDirection = DetermineTranslateDirection(e.DraggedItem, e.TargetItem);
                var translateY = currentState.OriginalTranslationY + (translateDirection * TranslateDistance);

                // Animate the translation (constrained to vertical movement)
                await targetFrame.TranslateTo(0, translateY, AnimationDuration, Easing.CubicOut);
                
                System.Diagnostics.Debug.WriteLine($"? Animated target frame to Y: {translateY} (vertical only)");
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
            _itemStates[e.TargetItem] = (
                state.OriginalTranslationY, 
                false, 
                state.OriginalOpacity,
                state.OriginalBackgroundColor,
                state.OriginalScale
            );
        }
    }

    #endregion

    #region XAML Event Handlers - Frame Registration and Drag Control

    private void OnFrameLoaded(object? sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext != null)
        {
            RegisterFrame(frame.BindingContext, frame);
        }
    }

    private void OnDragStarting_DragStarting(object? sender, DragStartingEventArgs e)
    {
        if (sender is DragGestureRecognizer dragRecognizer && 
            dragRecognizer.Parent is Frame frame &&
            frame.BindingContext != null)
        {
            System.Diagnostics.Debug.WriteLine($"?? OnDragStarting_DragStarting: Hiding original frame for Set {GetSetNumber(frame.BindingContext)}");
            
            // Register the frame and store original state
            RegisterFrame(frame.BindingContext, frame);
            
            // Store comprehensive original state before any modifications
            if (!_itemStates.ContainsKey(frame.BindingContext))
            {
                _itemStates[frame.BindingContext] = (
                    frame.TranslationY, 
                    false, 
                    frame.Opacity,
                    frame.BackgroundColor,
                    frame.Scale
                );
            }
            
            // Hide the original frame by setting opacity to 0 (prevents ghosting)
            frame.Opacity = 0;
            
            // Store the frame reference for later restoration
            _draggedItem = frame.BindingContext;
            
            // VERTICAL-ONLY DRAG CONSTRAINT: Set drag data to constrain movement
            if (e.Data != null)
            {
                e.Data.Text = "vertical_drag_only";
            }
        }
    }

    private async void OnDragCompleted_DropCompleted(object? sender, DropCompletedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"?? OnDragCompleted_DropCompleted: Comprehensive restoration starting");
        
        try
        {
            // Primary restoration - restore the specific dragged frame
            if (sender is DragGestureRecognizer dragRecognizer && 
                dragRecognizer.Parent is Frame frame &&
                frame.BindingContext != null)
            {
                var originalState = _itemStates.TryGetValue(frame.BindingContext, out var state) ? 
                    state : (0, false, 1, Colors.White, 1);
                
                // Restore all visual properties
                frame.Opacity = originalState.OriginalOpacity;
                frame.Scale = originalState.OriginalScale;
                frame.TranslationY = originalState.OriginalTranslationY;
                frame.BackgroundColor = originalState.OriginalBackgroundColor;
                
                System.Diagnostics.Debug.WriteLine($"? Primary restoration for Set {GetSetNumber(frame.BindingContext)}");
            }
            
            // Secondary restoration - ensure the stored dragged item is also restored
            if (_draggedItem != null && _activeFrames.TryGetValue(_draggedItem, out var draggedFrame))
            {
                var originalState = _itemStates.TryGetValue(_draggedItem, out var state) ? 
                    state : (0, false, 1, Colors.White, 1);
                
                draggedFrame.Opacity = originalState.OriginalOpacity;
                draggedFrame.Scale = originalState.OriginalScale;
                draggedFrame.TranslationY = originalState.OriginalTranslationY;
                draggedFrame.BackgroundColor = originalState.OriginalBackgroundColor;
                
                System.Diagnostics.Debug.WriteLine($"? Secondary restoration for stored dragged item Set {GetSetNumber(_draggedItem)}");
            }
            
            // Tertiary restoration - clean up any other frames that might be in a bad state
            await Task.Delay(50); // Small delay to ensure drop operations are complete
            await PerformComprehensiveCleanup();
            
            // Force CollectionView refresh to ensure UI synchronization
            await ForceCollectionViewRefresh();
            
            System.Diagnostics.Debug.WriteLine($"? Comprehensive restoration completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error during drop completion: {ex.Message}");
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
            
            // Store original state when registering
            if (!_itemStates.ContainsKey(item))
            {
                _itemStates[item] = (
                    frame.TranslationY, 
                    false, 
                    frame.Opacity,
                    frame.BackgroundColor,
                    frame.Scale
                );
            }
            
            System.Diagnostics.Debug.WriteLine($"?? Registered frame for Set {GetSetNumber(item)}");
        }
    }

    private void UnregisterFrame(object item)
    {
        if (item != null && _activeFrames.ContainsKey(item))
        {
            _activeFrames.Remove(item);
            _itemStates.Remove(item);
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
        
        // Re-register CollectionViews when page appears
        RegisterCollectionViews();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.SetTabBarIsVisible(this, true);
        
        // Perform final cleanup before leaving the page
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await PerformComprehensiveCleanup();
        });
        
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
        _collectionViews.Clear();
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