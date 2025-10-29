# Navigation Infrastructure

## Overview

This document describes the convention-based navigation infrastructure for the AutoLotManager WPF application.

## Architecture

The navigation infrastructure consists of three main components:

1. **INavigationService** - Interface for navigation operations
2. **NavigationService** - Implementation that handles page creation and view model binding
3. **NavigationConfiguration** - Centralized configuration for registering pages

## Convention

The navigation system follows a simple convention:

- Menu items in the hamburger menu have a `Label` (e.g., "Inventory")
- This label maps to a View (e.g., `InventoryHomePage`) 
- And optionally a ViewModel (e.g., `InventoryHomePageViewModel`)

## How to Add a New Page

To add a new navigable page to the application:

### 1. Create the View (XAML + Code-behind)

Create your page in the appropriate folder under `AutoLotManager.Desktop/Pages/`:

```csharp
// Example: Pages/MyFeature/MyFeaturePage.xaml.cs
public partial class MyFeaturePage : Page
{
    public MyFeaturePage()
    {
        InitializeComponent();
    }
}
```

### 2. Create the ViewModel (Optional)

If your page needs a view model, create it in `AutoLotManager.ViewModel/Pages/`:

```csharp
// Example: Pages/MyFeature/MyFeatureViewModel.cs
public class MyFeatureViewModel : ViewModelBase
{
    // Your view model implementation
}
```

### 3. Register the View Model in Bootstrapper

If you created a view model, register it with the dependency injection container in `Bootstrapper.cs`:

```csharp
builder.RegisterType<MyFeatureViewModel>().AsSelf();
```

### 4. Register the Page in NavigationConfiguration

Add your page registration in `NavigationConfiguration.RegisterPages()`:

```csharp
// Without view model:
navigationService.RegisterPage("MyFeature", typeof(MyFeaturePage));

// With view model:
navigationService.RegisterPage("MyFeature", typeof(MyFeaturePage), typeof(MyFeatureViewModel));
```

### 5. Add Menu Item to MainWindow.xaml

Add a new `HamburgerMenuGlyphItem` to the `ItemsSource` or `OptionsItemsSource` in `MainWindow.xaml`:

```xml
<mah:HamburgerMenuGlyphItem Glyph="&#xE11B;" Label="MyFeature"/>
```

The `Label` property must match the key you used in `NavigationConfiguration.RegisterPage()`.

## Example

Here's a complete example for adding an "Inventory" page:

1. **View**: `AutoLotManager.Desktop/Pages/Inventory/InventoryHomePage.xaml`
2. **ViewModel**: `AutoLotManager.ViewModel/Pages/Inventory/InventoryHomePageViewModel.cs`
3. **Registration in Bootstrapper**:
   ```csharp
   builder.RegisterType<InventoryHomePageViewModel>().AsSelf();
   ```
4. **Registration in NavigationConfiguration**:
   ```csharp
   navigationService.RegisterPage("Inventory", typeof(InventoryHomePage), typeof(InventoryHomePageViewModel));
   ```
5. **Menu Item in MainWindow.xaml**:
   ```xml
   <mah:HamburgerMenuGlyphItem Glyph="&#xE77B;" Label="Inventory"/>
   ```

## Benefits

- **Convention over Configuration**: Minimal code needed to add new pages
- **Centralized**: All navigation mappings in one place (`NavigationConfiguration`)
- **Dependency Injection**: View models are resolved from the DI container
- **Maintainable**: Easy to see all available pages and their mappings
- **Extensible**: Simple to extend with additional features (navigation guards, history, etc.)

## Technical Details

### NavigationService

The `NavigationService` is responsible for:
- Storing page registrations (page key -> page type + view model type)
- Creating page instances when navigation is requested
- Resolving and setting view models from the DI container
- Setting the page's DataContext to the view model

### Dependency Resolution

When a view model is registered:
1. The NavigationService first tries to resolve it from the Autofac container
2. If resolution fails, it creates an instance directly using `Activator.CreateInstance()`
3. The resolved/created view model is set as the page's DataContext

### Error Handling

The NavigationService validates:
- Page keys are not null or empty
- Page types are registered before navigation
- Page types derive from `System.Windows.Controls.Page`

If navigation fails, an exception is thrown and logged to Debug output.
