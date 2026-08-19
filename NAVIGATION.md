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
- This label is used as the *page key* and maps to a View (e.g., `InventoryHomePage`)
- And optionally a ViewModel (e.g., `InventoryHomePageViewModel`)

The convention is a naming *guideline*, not an inference mechanism. Every mapping is written out
explicitly in `NavigationConfiguration.RegisterPages()` — the View and ViewModel types are **not**
derived from the key by naming convention, so a page you forget to register is simply not
navigable. (Adding convention-based inference is tracked by issue #38.)

Page keys are matched **case-insensitively** (`NavigationService` stores registrations in a
`Dictionary<string, ...>` built with `StringComparer.OrdinalIgnoreCase`), so a menu `Label` of
"inventory" resolves the page registered as "Inventory".

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

### 2. Add the View to `AutoLotManager.Desktop.csproj`

> **Do not skip this step.** `AutoLotManager.Desktop` is a **legacy, non-SDK-style project**: its
> project file opens with `<Project ToolsVersion="15.0" xmlns="...">` and has **no `Sdk="..."`
> attribute**, which means **it does not glob source files**. Every `.cs` and `.xaml` file in the
> project is listed by hand inside an `<ItemGroup>`, and a file that is not listed is not part of
> the build — even though it is sitting in the right folder on disk, and even though Visual Studio
> will happily open and edit it. (Solution Explorer does not show it at all unless *Show All Files*
> is turned on, which is what makes this so disorienting.) The failure never points at the csproj:
>
> - **Neither entry added** - the page type does not exist, so the `typeof(MyFeaturePage)` in
>   step 5 fails with `CS0246: The type or namespace name 'MyFeaturePage' could not be found`,
>   while the file is plainly there in the folder.
> - **`<Compile>` added but not `<Page>`** - the XAML is never compiled, so no
>   `InitializeComponent()` is generated and the code-behind fails with
>   `CS0103: The name 'InitializeComponent' does not exist in the current context`.
> - **`<Page>` added but not `<Compile>`** - this is the genuinely silent one. The XAML compiler
>   still emits its generated partial class, so everything builds and the page navigates fine, but
>   your code-behind never runs: `InitializeComponent()` is never called and the page renders
>   **blank**, with no error anywhere.

Open `AutoLotManager.Desktop/AutoLotManager.Desktop.csproj` and add two entries to the `<ItemGroup>`
that already holds the other `Pages\...` items:

```xml
<Page Include="Pages\MyFeature\MyFeaturePage.xaml">
  <Generator>MSBuild:Compile</Generator>
  <SubType>Designer</SubType>
</Page>
<Compile Include="Pages\MyFeature\MyFeaturePage.xaml.cs">
  <DependentUpon>MyFeaturePage.xaml</DependentUpon>
  <SubType>Code</SubType>
</Compile>
```

Paths use backslashes and are relative to the project file. `<DependentUpon>` is just the XAML file
name, with no path.

**The ViewModel needs no csproj entry.** `AutoLotManager.ViewModel` *is* SDK-style
(`<Project Sdk="Microsoft.NET.Sdk">`), and SDK-style projects include `**/*.cs` automatically. That
asymmetry is what makes this trap confusing: dropping a file into the ViewModel project just works,
dropping one into the Desktop project does not.

Visual Studio's *Add > New Item* dialog writes these entries for you. If you create the files any
other way — by hand, from a template, or in a different editor — you must add them yourself.

### 3. Create the ViewModel (Optional)

If your page needs a view model, create it in `AutoLotManager.ViewModel/Pages/`:

```csharp
// Example: Pages/MyFeature/MyFeatureViewModel.cs
public class MyFeatureViewModel : ViewModelBase
{
    // Your view model implementation
}
```

### 4. Register the View Model in Bootstrapper

If you created a view model, register it with the dependency injection container in
`Bootstrapper.Bootstrap()`:

```csharp
builder.RegisterType<MyFeatureViewModel>().AsSelf();
```

Registration is what lets the view model take constructor dependencies. It is not strictly
required: if the type is not registered in the container, `NavigationService` falls back to
`Activator.CreateInstance()`, which needs a public parameterless constructor. Register it unless you
have a reason not to.

Note that the **Page** itself is never resolved from the container — `NavigationService` always
creates it with `Activator.CreateInstance()`, so a page class must have a public parameterless
constructor.

### 5. Register the Page in NavigationConfiguration

Add your page registration in `NavigationConfiguration.RegisterPages()`:

```csharp
// Without view model:
navigationService.RegisterPage("MyFeature", typeof(MyFeaturePage));

// With view model:
navigationService.RegisterPage("MyFeature", typeof(MyFeaturePage), typeof(MyFeatureViewModel));
```

### 6. Add Menu Item to MainWindow.xaml

Add a new `HamburgerMenuGlyphItem` to the `ItemsSource` or `OptionsItemsSource` in `MainWindow.xaml`:

```xml
<mah:HamburgerMenuGlyphItem Glyph="&#xE11B;" Label="MyFeature"/>
```

The `Label` property must match the key you used in `NavigationConfiguration.RegisterPage()`
(matching ignores case).

## Example

Here's a complete example for adding an "Inventory" page:

1. **View**: `AutoLotManager.Desktop/Pages/Inventory/InventoryHomePage.xaml`
2. **Entries in `AutoLotManager.Desktop.csproj`** (required — the Desktop project does not glob):
   ```xml
   <Page Include="Pages\Inventory\InventoryHomePage.xaml">
     <Generator>MSBuild:Compile</Generator>
     <SubType>Designer</SubType>
   </Page>
   <Compile Include="Pages\Inventory\InventoryHomePage.xaml.cs">
     <DependentUpon>InventoryHomePage.xaml</DependentUpon>
     <SubType>Code</SubType>
   </Compile>
   ```
3. **ViewModel**: `AutoLotManager.ViewModel/Pages/Inventory/InventoryHomePageViewModel.cs`
   (no csproj entry — that project is SDK-style)
4. **Registration in Bootstrapper**:
   ```csharp
   builder.RegisterType<InventoryHomePageViewModel>().AsSelf();
   ```
5. **Registration in NavigationConfiguration**:
   ```csharp
   navigationService.RegisterPage("Inventory", typeof(InventoryHomePage), typeof(InventoryHomePageViewModel));
   ```
6. **Menu Item in MainWindow.xaml**:
   ```xml
   <mah:HamburgerMenuGlyphItem Glyph="&#xE77B;" Label="Inventory"/>
   ```

## Benefits

- **Consistent naming**: A predictable `Label` -> `XxxPage` / `XxxPageViewModel` naming pattern
  (the mapping itself is still written out explicitly — see [Convention](#convention))
- **Centralized**: All navigation mappings in one place (`NavigationConfiguration`)
- **Dependency Injection**: View models are resolved from the DI container
- **Maintainable**: Easy to see all available pages and their mappings
- **Extensible**: Simple to extend with additional features (navigation guards, history, etc.)

## Technical Details

### NavigationService

The `NavigationService` is responsible for:
- Storing page registrations (page key -> page type + view model type), keyed case-insensitively
- Creating page instances when navigation is requested, via `Activator.CreateInstance()` — pages are
  never resolved from the container, so they need a public parameterless constructor
- Resolving and setting view models from the DI container
- Setting the page's DataContext to the view model

### Dependency Resolution

When a view model is registered:
1. The NavigationService checks whether the type is registered in the Autofac container
2. If it is, the container resolves it; if it is not, an instance is created directly using `Activator.CreateInstance()`
3. The resolved/created view model is set as the page's DataContext

Note that step 2 branches on registration, not on success: if `Resolve` itself throws, the
exception propagates rather than falling back to `Activator.CreateInstance()`.

### Error Handling

All of the following are validated at **run time**, not compile time — `RegisterPage` takes
`System.Type` arguments, so the compiler cannot check that what you pass is a `Page`.

`RegisterPage` validates:
- The page key is not null (`ArgumentNullException`) and not empty or whitespace
  (`ArgumentException`)
- The page type is not null (`ArgumentNullException`)
- The page type derives from `System.Windows.Controls.Page` (`ArgumentException`)

`NavigateTo` validates:
- The page key is not null (`ArgumentNullException`) and not empty or whitespace
  (`ArgumentException`)
- The key has been registered (`InvalidOperationException`)

The view model type is not validated at all.

If navigation fails, the NavigationService throws. Logging is the caller's responsibility, and it
is **selective**: `hmcLeftMenu_ItemClick` in `MainWindow.xaml.cs` catches `InvalidOperationException`
and `ArgumentException` (which covers `ArgumentNullException`) and writes them to Debug output.
Anything else propagates and takes the app down — most plausibly the `MissingMethodException` you
get from `Activator.CreateInstance` when a registered page or view model has no public parameterless
constructor.
