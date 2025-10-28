# Example: Adding a New "Reports" Page

This example shows how to add a new "Reports" page to the application using the navigation infrastructure.

## Step 1: Create the View

Create `AutoLotManager.Desktop/Pages/Reports/ReportsHomePage.xaml`:

```xml
<Page x:Class="AutoLotManager.Desktop.Pages.Reports.ReportsHomePage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
      mc:Ignorable="d" 
      d:DesignHeight="450" d:DesignWidth="800"
      Title="ReportsHomePage">

    <Grid>
        <TextBlock Text="{Binding PageTitle}" 
                   FontSize="24" 
                   HorizontalAlignment="Center" 
                   VerticalAlignment="Center"/>
    </Grid>
</Page>
```

Create `AutoLotManager.Desktop/Pages/Reports/ReportsHomePage.xaml.cs`:

```csharp
using System.Windows.Controls;

namespace AutoLotManager.Desktop.Pages.Reports
{
    /// <summary>
    /// Interaction logic for ReportsHomePage.xaml
    /// </summary>
    public partial class ReportsHomePage : Page
    {
        public ReportsHomePage()
        {
            InitializeComponent();
        }
    }
}
```

## Step 2: Create the ViewModel

Create `AutoLotManager.ViewModel/Pages/Reports/ReportsHomePageViewModel.cs`:

```csharp
namespace AutoLotManager.ViewModel.Pages.Reports
{
    public class ReportsHomePageViewModel : ViewModelBase
    {
        private string _pageTitle;

        public ReportsHomePageViewModel()
        {
            _pageTitle = "Reports";
        }

        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }
    }
}
```

## Step 3: Register the ViewModel in Bootstrapper

In `AutoLotManager.Desktop/Startup/Bootstrapper.cs`, add:

```csharp
// Add this using statement at the top
using AutoLotManager.ViewModel.Pages.Reports;

// In the Bootstrap() method, add this line with the other RegisterType calls:
builder.RegisterType<ReportsHomePageViewModel>().AsSelf();
```

## Step 4: Register the Page in NavigationConfiguration

In `AutoLotManager.Desktop/Navigation/NavigationConfiguration.cs`, add:

```csharp
// Add this using statement at the top
using AutoLotManager.Desktop.Pages.Reports;
using AutoLotManager.ViewModel.Pages.Reports;

// In the RegisterPages() method, add:
navigationService.RegisterPage("Reports", typeof(ReportsHomePage), typeof(ReportsHomePageViewModel));
```

## Step 5: Add the Menu Item

In `AutoLotManager.Desktop/MainWindow.xaml`, add the menu item to the `<mah:HamburgerMenu.ItemsSource>` section:

```xml
<mah:HamburgerMenuGlyphItem Glyph="&#xE7C3;" Label="Reports"/>
```

## That's it!

Now when you click the "Reports" menu item in the hamburger menu:
1. The NavigationService receives "Reports" as the page key
2. It creates an instance of `ReportsHomePage`
3. It resolves `ReportsHomePageViewModel` from the DI container
4. It sets the view model as the page's DataContext
5. The page is displayed in the main frame

## Benefits of This Approach

- **No hardcoded navigation logic** - Just register the page and it works
- **Dependency Injection** - View models can have dependencies injected
- **Convention-based** - Follow the pattern and everything just works
- **Easy to maintain** - All navigation configuration in one place
- **Type-safe** - Compile-time checking of page and view model types
