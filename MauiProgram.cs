using Microsoft.Extensions.Logging;
using SprintTrack.ViewModels;
using SprintTrack.Services;
using Sharpnado.CollectionView;

namespace SprintTrack
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSharpnadoCollectionView(loggerEnable: false)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            builder.Services.AddSingleton<ITrainingDataService, TrainingDataService>();
            
            // Register ViewModels and Pages
            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddSingleton<TrainingListViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<TrainingListPage>();
            // Note: TrainingSessionDetailPage is not registered as it requires a TrainingSession parameter
            // It should be created manually: new TrainingSessionDetailPage(trainingSession)

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
