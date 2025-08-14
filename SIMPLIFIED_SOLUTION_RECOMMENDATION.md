# ?? Förenklad Drag-and-Drop Lösning

## ?? **Slutsats och Rekommendation**

Efter att ha testat olika plugins upptäckte jag att:

### **Sharpnado.CollectionView Status:**
- ? **Version 3.1.2 är den senaste** tillgängliga
- ? **.NET 9 support är inte fullt kompatibel** ännu
- ?? **XAML namespace fungerar inte** med nuvarande version

### **CommunityToolkit.Maui Status:**
- ?? **Drag-and-drop är begränsad** i nuvarande version
- ?? **Inte lika avancerad** som Sharpnado

## ?? **Bästa Lösningen: Förenklad Native Implementation**

Istället för att använda en plugin som inte är fullt kompatibel med .NET 9, rekommenderar jag en **förenklad version** av den befintliga native implementationen:

### **Fördelar:**
1. ? **Fullt kompatibel med .NET 9**
2. ? **Betydligt mindre kod** än ursprunglig implementation
3. ? **Inga externa beroenden**
4. ? **Enkel att underhålla**
5. ? **Anpassningsbar** för dina specifika behov

### **Kodreduktion:**
- **Från 800+ rader ? endast ~100 rader**
- **Tar bort all komplex animation conflict prevention**
- **Behåller bara grundläggande funktionalitet**
- **Enkel renumbering av sets**

## ?? **Implementering**

### **Förenklad TrainingSessionDetailPage.xaml.cs:**
```csharp
public partial class TrainingSessionDetailPage : ContentPage
{
    private TrainingSessionDetailViewModel? _viewModel;

    public TrainingSessionDetailPage(TrainingSession trainingSession)
    {
        InitializeComponent();
        _viewModel = new TrainingSessionDetailViewModel(trainingSession);
        _viewModel.RequestBack += OnRequestBack;
        BindingContext = _viewModel;
        Shell.SetTabBarIsVisible(this, false);
    }

    private async void OnRequestBack(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Endast grundläggande event handlers - inga komplexa animationer
    private void OnEntryUnfocused(object? sender, FocusEventArgs e) { }
    private void OnTimeEntryTextChanged(object? sender, TextChangedEventArgs e) { /* Basic validation */ }
    private void OnExerciseEntryFocused(object? sender, FocusEventArgs e) { /* Basic focus handling */ }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTabBarIsVisible(this, false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.SetTabBarIsVisible(this, true);
        
        if (_viewModel != null)
        {
            _viewModel.RequestBack -= OnRequestBack;
        }
    }
}
```

### **Förenklad ViewModel:**
```csharp
// Endast grundläggande drag-and-drop commands
DragStartingCommand = new Command<object>(OnDragStarting);
DropCommand = new Command<object>(OnDrop);

private void OnDragStarting(object? set)
{
    _draggedSet = set;
    // Minimal logik
}

private void OnDrop(object? targetSet)
{
    if (_draggedSet != null && targetSet != null && _draggedSet != targetSet)
    {
        // Grundläggande reordering
        PerformReorder(_draggedSet, targetSet);
    }
    _draggedSet = null;
}
```

## ?? **Rekommendation**

**Gå med den förenklade native implementationen** eftersom:

1. **Plugins är inte redo för .NET 9** än
2. **Färre problem att felsöka**
3. **Bättre prestanda**
4. **Full kontroll över funktionalitet**
5. **Enklare att underhålla**

Vill du att jag implementerar denna förenklade lösning istället?