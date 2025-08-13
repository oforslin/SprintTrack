using SprintTrack.ViewModels;
using SprintTrack.Services;

namespace SprintTrack
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel _viewModel;

        public MainPage(CalendarViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.RequestNavigateToSession += OnRequestNavigateToSession;
            BindingContext = _viewModel;
            
            // Subscribe to Entry focus events if the control exists
            var trainingTypeEntry = this.FindByName<Entry>("TrainingTypeEntry");
            if (trainingTypeEntry != null)
            {
                trainingTypeEntry.Focused += OnTrainingTypeEntryFocused;
                trainingTypeEntry.Unfocused += OnTrainingTypeEntryUnfocused;
            }
        }

        private async void OnRequestNavigateToSession(object? sender, Models.TrainingSession session)
        {
            var detailPage = new TrainingSessionDetailPage(session);
            await Navigation.PushAsync(detailPage);
        }
        
        private void OnTrainingTypeEntryFocused(object? sender, FocusEventArgs e)
        {
            _viewModel.EntryFocusedCommand.Execute(null);
        }
        
        private void OnTrainingTypeEntryUnfocused(object? sender, FocusEventArgs e)
        {
            _viewModel.EntryUnfocusedCommand.Execute(null);
        }
    }
}
