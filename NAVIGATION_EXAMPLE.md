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

> **Note on namespaces:** most existing pages under `Pages/<Feature>/` use the flat
> `AutoLotManager.Desktop.Pages` namespace rather than a per-folder one. This example deliberately
> uses the per-folder namespace `AutoLotManager.Desktop.Pages.Reports`, which is why Step 5 adds a
> `using` for it. Either is fine — just keep the `x:Class` attribute, the code-behind `namespace`,
> and the `using` in `NavigationConfiguration.cs` consistent with each other.

## Step 2: Add the New Files to `AutoLotManager.Desktop.csproj`

> ### ⚠️ Required — this is the step that is easy to miss
>
> `AutoLotManager.Desktop` is a **legacy, non-SDK-style project**. Its project file starts with
> `<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">` — no
> `Sdk="..."` attribute — which means **MSBuild does not glob source files for it**. Every single
> `.cs` and `.xaml` file is listed by hand in an `<ItemGroup>`.
>
> Skip this step and the two files you just created are **not part of the build**, even though they
> are in the right folder and Visual Studio will happily open and edit them. (Solution Explorer will
> not even show them unless *Show All Files* is on.) None of the resulting errors point at the
> csproj:
>
> - **Neither entry added** - `ReportsHomePage` does not exist, so the `typeof(ReportsHomePage)` you
>   add in Step 5 fails with `CS0246: The type or namespace name 'ReportsHomePage' could not be
>   found`, while the file sits right there on disk.
> - **`<Compile>` added but not `<Page>`** - the XAML is never compiled, so `InitializeComponent()`
>   is never generated and `ReportsHomePage.xaml.cs` fails with `CS0103: The name
>   'InitializeComponent' does not exist in the current context`.
> - **`<Page>` added but not `<Compile>`** - the genuinely silent case. The XAML compiler emits its
>   generated partial class on its own, so the solution builds and navigation works, but your
>   code-behind never runs: `InitializeComponent()` is never called and the page renders **blank**,
>   with no error, no warning, and nothing in the Debug output.
>
> Treat this as part of "create the View", not as optional boilerplate.

Open `AutoLotManager.Desktop/AutoLotManager.Desktop.csproj`, find the `<ItemGroup>` that already
contains the other `Pages\...` entries (near `<Page Include="Pages\Settings\SettingsHomePage.xaml">`),
and add:

```xml
<Page Include="Pages\Reports\ReportsHomePage.xaml">
  <Generator>MSBuild:Compile</Generator>
  <SubType>Designer</SubType>
</Page>
<Compile Include="Pages\Reports\ReportsHomePage.xaml.cs">
  <DependentUpon>ReportsHomePage.xaml</DependentUpon>
  <SubType>Code</SubType>
</Compile>
```

Details that matter:

- `Include` paths use **backslashes** and are relative to the `.csproj` file.
- `<DependentUpon>` is the bare XAML file name (`ReportsHomePage.xaml`), **not** a path — it is what
  nests the code-behind under the XAML in Solution Explorer.
- `<Generator>MSBuild:Compile</Generator>` on the `<Page>` item is what runs the XAML compiler and
  generates the `InitializeComponent()` partial that the code-behind calls.

**The ViewModel from Step 3 does *not* need a csproj entry.** `AutoLotManager.ViewModel` is an
SDK-style project (`<Project Sdk="Microsoft.NET.Sdk">`), and SDK-style projects include `**/*.cs`
automatically. That asymmetry between the two projects is exactly what makes this trap confusing:
the file you drop into `AutoLotManager.ViewModel` compiles with no further action, while the file
you drop into `AutoLotManager.Desktop` is invisible until you list it.

If you add the page through Visual Studio's *Add > New Item > Page (WPF)* dialog, VS writes both
entries for you. Creating the files any other way — by hand, by copying an existing page, or from a
non-VS editor — means adding them yourself.

## Step 3: Create the ViewModel

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

## Step 4: Register the ViewModel in Bootstrapper

In `AutoLotManager.Desktop/Startup/Bootstrapper.cs`, add:

```csharp
// Add this using statement at the top
using AutoLotManager.ViewModel.Pages.Reports;

// In the Bootstrap() method, add this line with the other RegisterType calls:
builder.RegisterType<ReportsHomePageViewModel>().AsSelf();
```

This registration is what allows the view model to take constructor dependencies. It is not
strictly required — if the type is not in the container, `NavigationService` constructs it with
`Activator.CreateInstance()` instead — but register it unless you have a reason not to.

## Step 5: Register the Page in NavigationConfiguration

In `AutoLotManager.Desktop/Navigation/NavigationConfiguration.cs`, add:

```csharp
// Add these using statements at the top
using AutoLotManager.Desktop.Pages.Reports;
using AutoLotManager.ViewModel.Pages.Reports;

// In the RegisterPages() method, add:
navigationService.RegisterPage("Reports", typeof(ReportsHomePage), typeof(ReportsHomePageViewModel));
```

## Step 6: Add the Menu Item

In `AutoLotManager.Desktop/MainWindow.xaml`, add the menu item to the `<mah:HamburgerMenu.ItemsSource>` section:

```xml
<mah:HamburgerMenuGlyphItem Glyph="&#xE7C3;" Label="Reports"/>
```

The `Label` is the page key passed to `NavigateTo`, so it must match the string used in Step 5.
Matching is case-insensitive.

## That's it!

Now when you click the "Reports" menu item in the hamburger menu:
1. `MainWindow.hmcLeftMenu_ItemClick` reads the clicked item's `Label` and passes it to
   `INavigationService.NavigateTo`
2. The NavigationService looks up "Reports" (case-insensitively) in its registrations
3. It creates an instance of `ReportsHomePage` with `Activator.CreateInstance()` — pages are never
   resolved from the container, so they need a public parameterless constructor
4. It resolves `ReportsHomePageViewModel` from the DI container (or, if the type is not registered
   there, constructs it directly with `Activator.CreateInstance()`)
5. It sets the view model as the page's DataContext
6. `MainWindow` assigns the returned page to `frameContent.Content`, displaying it in the main frame

If the key is not registered, `NavigateTo` throws `InvalidOperationException`;
`MainWindow.xaml.cs` catches it and writes the message to Debug output, so a misregistered page
fails silently from the user's point of view.

## Benefits of This Approach

- **No hardcoded navigation logic in the UI** - `MainWindow` never names a page type; it just
  forwards the menu `Label` to the navigation service
- **Dependency Injection** - View models can have dependencies injected
- **Consistent naming** - Following the `Xxx` / `XxxHomePage` / `XxxHomePageViewModel` pattern keeps
  registrations predictable (the mapping itself is still explicit — types are not inferred from the
  key; see issue #38)
- **Easy to maintain** - All navigation configuration in one place
- **Validated** - `RegisterPage` takes `System.Type` arguments, so the page type is checked at
  **run time** (it must derive from `System.Windows.Controls.Page`, otherwise `ArgumentException`).
  Using `typeof(...)` guarantees the types exist at compile time, but nothing checks at compile time
  that they are a `Page` or a view model.
