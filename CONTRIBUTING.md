# Contributing to AutoLotManagerWPF

This document covers how to build the solution, the conventions the code already
follows, and the one non-obvious trap that will silently break your change if you
miss it (see [The legacy csproj trap](#the-legacy-csproj-trap) — read that section
even if you skip everything else).

---

## Table of contents

- [Prerequisites](#prerequisites)
- [Build and test](#build-and-test)
- [Solution layout](#solution-layout)
- [The legacy csproj trap](#the-legacy-csproj-trap)
- [Adding a page](#adding-a-page)
- [MVVM boundaries](#mvvm-boundaries)
- [Coding style](#coding-style)
- [Commit and PR flow](#commit-and-pr-flow)

---

## Prerequisites

This solution is **Windows-only**. It contains a WPF application targeting
.NET Framework 4.7.2; neither `dotnet build` nor Mono can build it. You need:

- **Windows** (Windows 10/11, or `windows-latest` in CI)
- **Visual Studio 2019 or later** with the *.NET desktop development* workload,
  or **Build Tools for Visual Studio** plus MSBuild
- **.NET Framework 4.7.2 Developer Pack** (the targeting pack — the runtime alone
  is not enough)
- **NuGet CLI** (`nuget.exe`) on `PATH`

The `dotnet` CLI cannot build this solution end to end: it cannot build the legacy
WPF project, and the test project references it. Use MSBuild.

---

## Build and test

From the repository root:

```powershell
nuget restore AutoLotManager.sln
msbuild AutoLotManager.sln /p:Configuration=Release /m
```

`nuget restore` at the **solution** level is required, and `msbuild -t:restore` is
not a substitute. The solution mixes two package management styles:

| Project | Style | Packages |
| --- | --- | --- |
| `AutoLotManager.Core` | SDK-style, `netstandard2.0` | `PackageReference` |
| `AutoLotManager.ViewModel` | SDK-style, `netstandard2.0` | `PackageReference` |
| `AutoLotManager.Desktop` | **legacy csproj**, `net472`, WPF | `packages.config` |
| `AutoLotManager.Tests` | SDK-style, `net472`, NUnit | `PackageReference` |

`msbuild -t:restore` only understands `PackageReference`, so it silently skips the
Desktop project's `packages.config` dependencies and the build then fails on
missing assemblies. The NuGet CLI handles both.

Do **not** pass `/p:Platform`. The solution platform is `Any CPU` while the
projects use `AnyCPU`; forcing the platform at solution level trips that mismatch.

### Tests

Tests live in `AutoLotManager.Tests` (NUnit, `net472`). Run them from Visual
Studio's Test Explorer, or from the command line against the already-built
assembly:

```powershell
vstest.console.exe AutoLotManager.Tests\bin\Release\net472\AutoLotManager.Tests.dll
```

`dotnet test` will not work here: it wants to build the test project itself, and
that pulls in the legacy WPF Desktop project, which the `dotnet` CLI cannot build.

### CI

`.github/workflows/build.yml` runs on every pull request against `master`. It
builds both **Debug** and **Release** on a Windows runner and runs the NUnit suite
in each configuration. Both jobs must be green before a PR is merged. If you are
working on a machine without a .NET toolchain, CI is your only verifier — push
early and let it check your work.

---

## Solution layout

```
AutoLotManager.Core/          Domain models (Car). netstandard2.0.
AutoLotManager.ViewModel/     ViewModelBase, MainWindowViewModel, Pages/*.
                              netstandard2.0 — deliberately UI-framework-free.
AutoLotManager.Desktop/       The WPF application. net472, legacy csproj.
  MainWindow.xaml(.cs)        Shell window with the MahApps hamburger menu.
  Pages/                      Views (XAML + code-behind).
  Navigation/                 INavigationService, NavigationService,
                              NavigationConfiguration.
  Startup/Bootstrapper.cs     Autofac container composition root.
AutoLotManager.Tests/         NUnit tests. net472.
```

Dependency direction is one-way: `Desktop` → `ViewModel` → `Core`. Nothing in
`ViewModel` or `Core` may reference `Desktop`.

---

## The legacy csproj trap

**`AutoLotManager.Desktop` is a legacy (non-SDK) project. It does not glob files.**

Its project file opens with:

```xml
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
```

There is no `Sdk="Microsoft.NET.Sdk"` attribute, and therefore no implicit
`**/*.cs` include. Every single source and XAML file in that project is listed by
hand inside an `<ItemGroup>`.

The consequence, and the reason this section exists:

> A new file that you create in `AutoLotManager.Desktop` but do not add to
> `AutoLotManager.Desktop.csproj` **is silently not compiled into the
> application**. There is no error and no warning. The file simply is not part of
> the build. Your new page will not exist at run time, your new class will not
> resolve, and the failure looks like a mysterious runtime problem rather than a
> build problem.

Visual Studio adds these entries for you when you use *Add → New Item* on the
project. If you create files any other way — a text editor, a script, an agent,
`git apply` of a patch — you must edit the csproj yourself.

### What to add

A plain C# file needs one entry:

```xml
<Compile Include="Navigation\MyNewService.cs" />
```

A **XAML page needs two** entries, and both are required — the `<Page>` entry is
what generates the `InitializeComponent` partial, and the `<Compile>` entry is
what compiles the code-behind that calls it:

```xml
<Page Include="Pages\Reports\ReportsHomePage.xaml">
  <Generator>MSBuild:Compile</Generator>
  <SubType>Designer</SubType>
</Page>
<Compile Include="Pages\Reports\ReportsHomePage.xaml.cs">
  <DependentUpon>ReportsHomePage.xaml</DependentUpon>
</Compile>
```

Note the paths use **backslashes**, matching every other entry in the file.

Both belong in the one `<ItemGroup>` that already holds the application's pages —
the group that opens with `<ApplicationDefinition Include="App.xaml">`. Inside it
the `<Compile>` entries for page code-behind are listed together (roughly in path
order) and the `<Page>` entries are listed together after them, so put each of your
two items with its own kind rather than side by side. MSBuild does not care, but
the diff stays readable.

### The other three projects

`AutoLotManager.Core`, `AutoLotManager.ViewModel`, and `AutoLotManager.Tests` are
all SDK-style (`<Project Sdk="Microsoft.NET.Sdk">`) and glob their sources
automatically. **Adding a file to any of those three requires no project edit at
all.** Only `AutoLotManager.Desktop` has this problem.

### Checklist before you push

If your change adds files under `AutoLotManager.Desktop/`, confirm the diff also
touches `AutoLotManager.Desktop/AutoLotManager.Desktop.csproj`. If it does not,
you have almost certainly hit this trap.

---

## Adding a page

`NAVIGATION.md` and `NAVIGATION_EXAMPLE.md` walk through the navigation
infrastructure. Their steps are correct but **incomplete** — they predate this
document and omit the csproj registration (step 2 below). The full sequence, using
a hypothetical "Reports" page:

1. **Create the View** in `AutoLotManager.Desktop/Pages/Reports/` —
   `ReportsHomePage.xaml` and `ReportsHomePage.xaml.cs`. The code-behind is a
   `partial class` deriving from `Page` whose constructor does nothing but call
   `InitializeComponent()`. Its namespace must match the XAML's `x:Class`.

   > Existing pages declare `namespace AutoLotManager.Desktop.Pages` regardless of
   > which subfolder they sit in, while the ViewModels do use folder-matching
   > namespaces (`AutoLotManager.ViewModel.Pages.Inventory`). Either is acceptable
   > for a new page; just keep `x:Class` and the code-behind namespace in sync.

2. **Register both files in `AutoLotManager.Desktop.csproj`** — the `<Page>` and
   `<Compile>` entries shown in
   [The legacy csproj trap](#the-legacy-csproj-trap). Skip this and the page never
   compiles.

3. **Create the ViewModel** in `AutoLotManager.ViewModel/Pages/Reports/` deriving
   from `ViewModelBase`. No csproj edit needed — that project globs.

4. **Register the ViewModel** in `AutoLotManager.Desktop/Startup/Bootstrapper.cs`,
   alongside the other registrations:

   ```csharp
   builder.RegisterType<ReportsHomePageViewModel>().AsSelf();
   ```

   A ViewModel that is not registered still works — `NavigationService` falls back
   to `Activator.CreateInstance` — but only if it has a parameterless constructor.
   Register it, or constructor injection will not be available to it.

5. **Register the page** in
   `AutoLotManager.Desktop/Navigation/NavigationConfiguration.cs`:

   ```csharp
   navigationService.RegisterPage("Reports", typeof(ReportsHomePage), typeof(ReportsHomePageViewModel));
   ```

6. **Add the hamburger menu item** in `AutoLotManager.Desktop/MainWindow.xaml`,
   inside `<mah:HamburgerMenu.ItemsSource>`:

   ```xml
   <mah:HamburgerMenuGlyphItem Glyph="&#xE7C3;" Label="Reports"/>
   ```

   **The `Label` is the navigation key.** `MainWindow.xaml.cs` reads
   `menuItem.Label` and passes it straight to `NavigationService.NavigateTo`, so it
   must match the string passed to `RegisterPage`. The lookup is case-insensitive,
   but keep the two identical anyway.

   > **Use `ItemsSource`, not `OptionsItemsSource`.** Only `ItemClick` is wired up
   > (`MainWindow.xaml`), and the handler reads `SelectedItem` — options items land
   > in `SelectedOptionsItem` and never reach the handler at all. An item placed in
   > `OptionsItemsSource` is therefore dead: clicking it does nothing. The existing
   > "About" entry is exactly this case today.

Steps 5 and 6 are two halves of the same fact. Change one, change the other.

### When navigation appears to do nothing

`hmcLeftMenu_ItemClick` catches `InvalidOperationException` and `ArgumentException`
and only writes them to `Debug.WriteLine` — there is a `TODO` about surfacing them
to the user. So a `Label` that does not match any `RegisterPage` key produces a
click that visibly does nothing, with the real message
(`Page 'X' is not registered`) visible only in the debugger's Output window. If a
new menu item does nothing when clicked, check the Output window first, then check
for a `Label`/`RegisterPage` typo, then check that the item is under `ItemsSource`.

---

## MVVM boundaries

These two rules are the architecture. Please do not erode them.

### 1. No business logic in code-behind

A `*.xaml.cs` file should contain a constructor calling `InitializeComponent()`
and, at most, view-only plumbing that genuinely cannot be expressed in XAML.
State, decisions, data access, and formatting belong in the ViewModel, reached
through bindings and commands.

`MainWindow.xaml.cs` is the one place that legitimately touches navigation, because
translating a MahApps `HamburgerMenuGlyphItem` selection into a `Page` instance is
inherently a view concern.

### 2. ViewModels stay free of WPF and UI dependencies

`AutoLotManager.ViewModel` targets **`netstandard2.0` on purpose**. That target is
the enforcement mechanism: the WPF assemblies are not reachable from
`netstandard2.0`, so the compiler stops a `using System.Windows;` from ever landing
there.

Concretely, in `AutoLotManager.ViewModel`:

- No WPF types: no `Page`, no `Window`, no `Dispatcher`, no `MessageBox`, no
  `Visibility`, no `Brush`, no `DependencyObject`.
- `System.Windows.Input.ICommand` is the exception, and it is fine — despite the
  namespace it lives in `netstandard2.0`, not in WPF. The existing ViewModels
  import it for their Prism `DelegateCommand`-backed command properties. Expose
  behaviour to the view as an `ICommand`; that is the intended seam.
- No WPF-only NuGet packages. Adding one that has no `netstandard2.0` assets makes
  NuGet fall back to .NET Framework assets and warn `NU1701` on every build — this
  already happened once with the Syncfusion WPF packages; see the comment in
  `AutoLotManager.ViewModel.csproj` and **issue #39** for the cleanup.
- Notify changes through `ViewModelBase.OnPropertyChanged()`, which uses
  `[CallerMemberName]` — call it with no argument from the property setter.

If a ViewModel appears to need a UI service (a dialog, navigation, a file picker),
define an interface for it and have the Desktop project supply the implementation
through Autofac. Do not reach for the WPF type.

---

## Coding style

Formatting is codified in [`.editorconfig`](.editorconfig) at the repository root;
Visual Studio, VS Code, and Rider all honour it, so in practice you should not have
to think about most of it. The rules there are set to *suggestion* severity — they
guide the IDE, they do not fail the build.

The conventions, in prose:

- **4-space indentation**, spaces not tabs (the `.sln` is the exception; Visual
  Studio owns its formatting).
- **Allman braces** — the opening brace goes on its own line, for types, methods,
  properties, and control flow alike.
- **Always brace**, even a single-statement `if`. The guard clauses in
  `NavigationService` show the house style. (One unbraced `return` survives in
  `MainWindow.xaml.cs`; it predates this document.)
- **Naming**: `PascalCase` for types, methods, properties, events, and constants;
  `_camelCase` for private fields; `camelCase` for parameters and locals;
  `I`-prefixed interfaces.
- **`var`** where the right-hand side already names the type
  (`var builder = new ContainerBuilder();`), the explicit type otherwise.
- **Block bodies** over expression-bodied members, including property accessors:
  `get { return _value; }` is the existing style.
- **`using` directives** go outside the namespace. The existing files put project
  and third-party namespaces *before* `System.*`; that ordering is preserved
  rather than churned, so do not reorder an existing file's usings as a drive-by.
- **File encoding**: UTF-8 throughout. The repo is mixed on the byte-order mark —
  Visual Studio wrote a BOM, later hand-added files have none — so `.editorconfig`
  deliberately leaves `charset` unset for `.cs` and lets each file keep what it
  has. Do not "fix" a file's BOM as a drive-by; it makes the whole file look
  changed.
- **Line endings**: CRLF in the working tree. `.gitattributes` sets `* text=auto`,
  so git normalises to LF in the repository — you do not need to do anything.
- **XML doc comments** (`/// <summary>`) on public types and members that are not
  self-evident. Explain *why*, not *what*.
- One public type per file; the file name matches the type.

---

## Commit and PR flow

1. **Branch off `master`.** Do not commit to `master` directly. If you do not have
   push access to this repository, fork it first and branch in your fork.

   ```powershell
   git fetch origin master
   git checkout -b feature/my-change origin/master
   ```

2. **Keep commits focused.** One logical change per commit; do not mix a refactor
   with a behaviour change.

3. **Write clear commit messages.** A short imperative subject line (roughly 50
   characters, no trailing period), a blank line, then a body explaining the
   *why* if it is not obvious. Reference the issue: `Fixes #26`.

4. **Open a pull request against `master`.** Describe what changed and how you
   verified it. If the change is user-visible in the WPF app, a screenshot helps.

5. **CI must be green.** Both the *Build Debug* and *Build Release* jobs must
   succeed. A red build will not be merged.

6. **Before you push, check:**
   - [ ] New files under `AutoLotManager.Desktop/` are registered in its csproj.
   - [ ] New pages are registered in `NavigationConfiguration.cs` *and* have a
         matching `Label` in `MainWindow.xaml`.
   - [ ] No WPF types leaked into `AutoLotManager.ViewModel` or
         `AutoLotManager.Core`.
   - [ ] No business logic added to a `*.xaml.cs`.
   - [ ] Tests added or updated for new behaviour in `Core`, `ViewModel`, or
         `Navigation`.
   - [ ] No `bin/`, `obj/`, `packages/`, or `.vs/` artefacts in the diff.
