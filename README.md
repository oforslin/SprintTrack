# SprintTrack

A comprehensive training tracking application built with .NET MAUI, designed to help athletes and fitness enthusiasts track their workout sessions, exercises, and performance metrics.

## Features

### ??? Exercise Management
- **Multiple Exercise Types**: Support for strength training, running, sprinting, and sled sprint exercises
- **Detailed Set Tracking**: Track reps, weight, time, and distance for each set
- **Warmup Set Support**: Mark sets as warmup with visual indicators
- **Exercise Library**: Pre-populated exercise database with search functionality

### ?? Training Session Planning
- **Calendar Integration**: Visual calendar for planning and tracking workouts
- **Session Details**: Comprehensive training session information including:
  - Date and duration
  - Training type and description
  - Intensity rating (1-10 scale)
  - Exercise lists with detailed metrics

### ?? Exercise Types Supported
- **Strength Training**: Sets, reps, and weight tracking
- **Running**: Duration and distance tracking with time format (HH:MM:SS)
- **Sprinting**: Precise timing with seconds and hundredths (SS.HH)
- **Sled Sprint**: Combined timing, distance, and weight tracking

### ?? User Experience
- **Intuitive Interface**: Clean, modern UI optimized for mobile and desktop
- **Quick Entry**: Fast exercise and set entry with predefined exercise templates
- **Visual Feedback**: Color-coded warmup sets and exercise type indicators
- **Responsive Design**: Works seamlessly across iOS, Android, Windows, and macOS

## Technology Stack

- **.NET 9**: Latest .NET framework for cross-platform development
- **.NET MAUI**: Cross-platform UI framework for native mobile and desktop apps
- **MVVM Pattern**: Clean separation of concerns with ViewModels
- **Data Binding**: Reactive UI updates with automatic change notifications
- **Custom Converters**: Specialized data conversion for UI display

## Platform Support

- ? **Android** (API 21+)
- ? **iOS** (15.0+)
- ? **macOS** (Catalyst 15.0+)
- ? **Windows** (10.0.17763.0+)

## Getting Started

### Prerequisites
- Visual Studio 2022 17.8+ or Visual Studio Code
- .NET 9 SDK
- Platform-specific workloads for your target platforms

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/SprintTrack.git
   cd SprintTrack
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run on your preferred platform:
   ```bash
   # Windows
   dotnet run --framework net9.0-windows10.0.19041.0
   
   # Android (with emulator/device connected)
   dotnet run --framework net9.0-android
   
   # iOS (Mac only, with simulator/device)
   dotnet run --framework net9.0-ios
   ```

## Project Structure

```
SprintTrack/
??? Models/                 # Data models
?   ??? Exercise.cs        # Exercise and set models
?   ??? TrainingSession.cs # Training session model
??? ViewModels/            # MVVM ViewModels
?   ??? CalendarViewModel.cs
?   ??? TrainingSessionDetailViewModel.cs
?   ??? TrainingListViewModel.cs
??? Views/                 # XAML pages
?   ??? MainPage.xaml     # Calendar view
?   ??? TrainingListPage.xaml
?   ??? TrainingSessionDetailPage.xaml
??? Services/              # Business logic services
?   ??? TrainingDataService.cs
??? Converters/            # Value converters for UI
?   ??? ValueConverters.cs
??? Behaviors/             # Input validation behaviors
??? Resources/             # App resources (styles, images, etc.)
```

## Usage

### Creating a Training Session
1. Navigate to the calendar view
2. Tap on any date to select it
3. Click "Add Session" to create a new training session
4. Fill in session details (type, duration, intensity)
5. Save the session

### Adding Exercises
1. Open a training session
2. Click "Add Exercise" 
3. Search and select from the exercise library or create custom exercises
4. Configure exercise-specific settings (sets, reps, weight, etc.)
5. Add the exercise to your session

### Tracking Sets
- Use the + and - buttons for quick rep/weight adjustments
- Tap set numbers to toggle warmup status
- Enter time values directly for running/sprint exercises
- Remove sets using the × button

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Guidelines
- Follow MVVM patterns
- Use proper data binding
- Maintain platform compatibility
- Add unit tests for new features
- Update documentation

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Roadmap

- [ ] Data persistence and cloud sync
- [ ] Performance analytics and charts
- [ ] Exercise progress tracking
- [ ] Custom workout templates
- [ ] Social sharing features
- [ ] Wearable device integration

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/yourusername/SprintTrack/issues) on GitHub.

---

**SprintTrack** - Track your training, achieve your goals! ??