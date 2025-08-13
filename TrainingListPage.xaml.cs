using SprintTrack.ViewModels;
using SprintTrack.Models;

namespace SprintTrack
{
    public partial class TrainingListPage : ContentPage
    {
        private TrainingListViewModel _viewModel;

        public TrainingListPage(TrainingListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private async void OnRequestNavigateToSession(object? sender, TrainingSession session)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Navigating to session: {session.DisplayName}");
                var detailPage = new TrainingSessionDetailPage(session);
                await Navigation.PushAsync(detailPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                // Optionally show user-friendly error message
                await DisplayAlert("Error", "Could not open training session details.", "OK");
            }
        }

        // Public method that can be called directly if event system fails
        public async Task NavigateToSession(TrainingSession session)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Direct navigation to session: {session.DisplayName}");
                var detailPage = new TrainingSessionDetailPage(session);
                await Navigation.PushAsync(detailPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Direct navigation error: {ex.Message}");
                await DisplayAlert("Error", "Could not open training session details.", "OK");
            }
        }

        // Public method to reinitialize event handlers (can be called from XAML if needed)
        public void ReinitializeEventHandlers()
        {
            if (_viewModel != null)
            {
                // Completely disconnect and reconnect
                _viewModel.RequestNavigateToSession -= OnRequestNavigateToSession;
                _viewModel.RequestNavigateToSession += OnRequestNavigateToSession;
                
                System.Diagnostics.Debug.WriteLine("Event handlers reinitialized");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Always ensure event handler is connected when page appears
            if (_viewModel != null)
            {
                // Remove any existing handler first to avoid duplicates
                _viewModel.RequestNavigateToSession -= OnRequestNavigateToSession;
                // Add the handler
                _viewModel.RequestNavigateToSession += OnRequestNavigateToSession;
                
                // Refresh the filtered sessions when page appears
                _viewModel.SearchCommand.Execute(null);
                
                System.Diagnostics.Debug.WriteLine($"TrainingListPage OnAppearing - Event handler reconnected. Has handlers: {_viewModel.HasNavigationHandlers}");
                
                // Force a small delay to ensure everything is properly initialized
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    System.Diagnostics.Debug.WriteLine($"Post-delay check - Has handlers: {_viewModel.HasNavigationHandlers}");
                });
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Don't disconnect the event handler when page disappears
            // This was causing the problem - keep it connected for when we return
            System.Diagnostics.Debug.WriteLine("TrainingListPage OnDisappearing - Event handler kept connected");
        }

        // Clean up only when the page is being destroyed, not just hidden
        ~TrainingListPage()
        {
            if (_viewModel != null)
            {
                _viewModel.RequestNavigateToSession -= OnRequestNavigateToSession;
            }
        }
    }
}