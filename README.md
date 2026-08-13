# AutoLotManagerWPF

[![Build](https://github.com/aycee247/AutoLotManagerWPF/actions/workflows/build.yml/badge.svg)](https://github.com/aycee247/AutoLotManagerWPF/actions/workflows/build.yml)

Software to manage a fictional car lot using WPF, XAML, C#, Autofac, Prism, and MVVM.

![WPFdemoapp](https://user-images.githubusercontent.com/26072560/194612881-87cbe693-4536-44ff-ba6a-f3cf8a68587e.PNG)

## 📋 Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Documentation](#documentation)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

## ✨ Features

### Current Functionality

#### Core Features
- **Vehicle Management**: Manage car inventory with properties including VIN, Make, Model, Year, and Color
- **MVVM Architecture**: Clean separation of concerns using the Model-View-ViewModel pattern
- **Navigation System**: Convention-based navigation infrastructure with dependency injection
- **Modular Design**: Organized into separate projects for Core domain logic, ViewModels, and Desktop UI

#### User Interface
- **Modern WPF UI**: Built with Material Design and MahApps.Metro for a modern look and feel
- **Hamburger Menu Navigation**: Intuitive side navigation menu for accessing different modules
- **Multiple Pages/Views**:
  - **Home Page**: Main dashboard and landing page
  - **Inventory Management**: View, add, edit, and manage vehicle inventory
  - **Export Inventory**: Export inventory data to external formats
  - **Sales Management**: Track and manage vehicle sales
  - **Settings**: Application configuration and preferences
  - **About**: Application information and version details

#### Technical Features
- **Dependency Injection**: Full IoC container support using Autofac
- **Convention-Based Navigation**: Automatic page routing based on naming conventions
- **Extensible Architecture**: Easy to add new pages and features
- **View Model Resolution**: Automatic view model creation and binding via DI container

## 🛠️ Technology Stack

### Framework & Platform
- **.NET Framework 4.7.2**: Target framework for Windows desktop applications
- **WPF (Windows Presentation Foundation)**: Modern Windows desktop UI framework
- **XAML**: Declarative markup for UI design

### Libraries & Packages
- **Autofac**: Dependency injection and IoC container
- **Prism**: Framework for building loosely coupled, maintainable, and testable XAML applications
- **MahApps.Metro**: Toolkit for creating modern WPF applications
- **Bogus**: Library for generating fake data (used in development/testing)

### Architecture Patterns
- **MVVM (Model-View-ViewModel)**: Primary architectural pattern
- **Dependency Injection**: For loose coupling and testability
- **Repository Pattern**: For data access abstraction (ready for implementation)
- **Navigation Service**: Convention-based page navigation

## 🏗️ Architecture

### Solution Structure

The solution is organized into three main projects:

1. **AutoLotManager.Core**: Domain models and business logic
   - Contains the `Car` entity and core domain types
   - Framework-agnostic business logic

2. **AutoLotManager.ViewModel**: View Models and presentation logic
   - All ViewModel classes implementing `ViewModelBase`
   - Separated from UI concerns for testability
   - Page-specific ViewModels organized by feature

3. **AutoLotManager.Desktop**: WPF UI project
   - XAML views and code-behind
   - Navigation infrastructure (INavigationService, NavigationService)
   - Startup and bootstrapping logic
   - Material Design UI components

### Navigation Architecture

The application uses a custom convention-based navigation system:

- **INavigationService**: Interface defining navigation operations
- **NavigationService**: Implementation that handles page creation and ViewModel binding
- **NavigationConfiguration**: Centralized registration of all navigable pages
- **Automatic ViewModel Resolution**: ViewModels are resolved from the DI container and bound to views automatically

For detailed information, see [NAVIGATION.md](NAVIGATION.md) and [NAVIGATION_EXAMPLE.md](NAVIGATION_EXAMPLE.md).

## 🚀 Getting Started

### Prerequisites

- Windows Operating System (Windows 10 or later recommended)
- Visual Studio 2019 or later (Visual Studio 2022 fully supported)
- .NET Framework 4.7.2 SDK or later
- NuGet Package Manager (included with Visual Studio)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/aycee247/AutoLotManagerWPF.git
   cd AutoLotManagerWPF
   ```

2. **Restore NuGet packages**
   ```bash
   nuget restore AutoLotManager.sln
   ```
   Or restore via Visual Studio: Right-click solution → Restore NuGet Packages

3. **Build the solution**
   ```bash
   msbuild AutoLotManager.sln /p:Configuration=Release
   ```
   Or build via Visual Studio: Build → Build Solution (Ctrl+Shift+B)

4. **Run the application**
   - Set `AutoLotManager.Desktop` as the startup project
   - Press F5 to run in debug mode, or Ctrl+F5 to run without debugging

## 📁 Project Structure

```
AutoLotManagerWPF/
├── AutoLotManager.Core/              # Domain models and business logic
│   ├── Car.cs                        # Car entity model
│   └── AutoLotManager.Core.csproj
│
├── AutoLotManager.ViewModel/         # ViewModels and presentation logic
│   ├── ViewModelBase.cs              # Base class for all ViewModels
│   ├── MainWindowViewModel.cs        # Main window ViewModel
│   ├── MainHomePageViewModel.cs      # Home page ViewModel
│   ├── Pages/                        # Feature-specific ViewModels
│   │   └── Inventory/
│   │       ├── InventoryHomePageViewModel.cs
│   │       └── ExportInventoryListPageViewModel.cs
│   └── AutoLotManager.ViewModel.csproj
│
├── AutoLotManager.Desktop/           # WPF UI project
│   ├── App.xaml                      # Application entry point
│   ├── MainWindow.xaml               # Main window with hamburger menu
│   ├── Navigation/                   # Navigation infrastructure
│   │   ├── INavigationService.cs
│   │   ├── NavigationService.cs
│   │   └── NavigationConfiguration.cs
│   ├── Startup/                      # Application bootstrapping
│   │   └── Bootstrapper.cs           # DI container configuration
│   ├── Pages/                        # All page views
│   │   ├── MainHomePage.xaml
│   │   ├── About/
│   │   ├── Inventory/
│   │   ├── Sales/
│   │   └── Settings/
│   └── AutoLotManager.Desktop.csproj
│
├── README.md                         # This file
├── NAVIGATION.md                     # Navigation system documentation
├── NAVIGATION_EXAMPLE.md             # Step-by-step navigation guide
├── IMPLEMENTATION_SUMMARY.md         # Implementation details
└── AutoLotManager.sln                # Visual Studio solution file
```

## 📚 Documentation

Additional documentation is available in the following files:

- **[NAVIGATION.md](NAVIGATION.md)**: Comprehensive guide to the navigation infrastructure
- **[NAVIGATION_EXAMPLE.md](NAVIGATION_EXAMPLE.md)**: Step-by-step example of adding a new page
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)**: Detailed implementation notes and architectural decisions

### Adding a New Page

To add a new page to the application:

1. Create the Page (View) in `AutoLotManager.Desktop/Pages/`
2. Create the ViewModel in `AutoLotManager.ViewModel/Pages/`
3. Register the ViewModel in `Bootstrapper.cs`
4. Register the page in `NavigationConfiguration.cs`
5. Add a menu item in `MainWindow.xaml`

See [NAVIGATION_EXAMPLE.md](NAVIGATION_EXAMPLE.md) for a complete walkthrough.

## 🗺️ Roadmap

### Planned Enhancements

Each item below is tracked as a GitHub issue — follow the link for scope and implementation notes.

#### Core Functionality
- [ ] **Database Integration** ([#6](https://github.com/aycee247/AutoLotManagerWPF/issues/6)): Implement Entity Framework or Dapper for data persistence
- [ ] **Customer Management** ([#7](https://github.com/aycee247/AutoLotManagerWPF/issues/7)): Add customer entity and CRUD operations
- [ ] **Sales Transaction Processing** ([#8](https://github.com/aycee247/AutoLotManagerWPF/issues/8)): Complete sales workflow with pricing and payment tracking
- [ ] **Inventory Reporting** ([#9](https://github.com/aycee247/AutoLotManagerWPF/issues/9)): Generate reports on inventory status, sales trends, and analytics
- [ ] **Search & Filter** ([#10](https://github.com/aycee247/AutoLotManagerWPF/issues/10)): Advanced search capabilities across inventory and sales
- [ ] **Data Import/Export** ([#11](https://github.com/aycee247/AutoLotManagerWPF/issues/11)): Support for CSV, Excel, and JSON formats

#### User Interface
- [ ] **Dashboard with Analytics** ([#12](https://github.com/aycee247/AutoLotManagerWPF/issues/12)): Charts and graphs for sales and inventory metrics
- [ ] **Print Functionality** ([#13](https://github.com/aycee247/AutoLotManagerWPF/issues/13)): Print invoices, reports, and inventory lists
- [ ] **Multi-Language Support** ([#14](https://github.com/aycee247/AutoLotManagerWPF/issues/14)): Internationalization (i18n) for global use
- [ ] **Themes** ([#15](https://github.com/aycee247/AutoLotManagerWPF/issues/15)): Light/dark mode and customizable color schemes
- [ ] **Responsive Design** ([#16](https://github.com/aycee247/AutoLotManagerWPF/issues/16)): Better layout adaptation for different screen sizes

#### Technical Improvements
- [ ] **Unit Tests** ([#17](https://github.com/aycee247/AutoLotManagerWPF/issues/17)): Comprehensive test coverage for ViewModels and business logic
- [ ] **Integration Tests** ([#18](https://github.com/aycee247/AutoLotManagerWPF/issues/18)): End-to-end testing of key workflows
- [ ] **Logging Framework** ([#19](https://github.com/aycee247/AutoLotManagerWPF/issues/19)): Implement structured logging (Serilog, NLog)
- [ ] **Error Handling** ([#20](https://github.com/aycee247/AutoLotManagerWPF/issues/20)): Global exception handling and user-friendly error messages
- [ ] **Configuration Management** ([#21](https://github.com/aycee247/AutoLotManagerWPF/issues/21)): External configuration for app settings
- [ ] **API Integration** ([#22](https://github.com/aycee247/AutoLotManagerWPF/issues/22)): RESTful API for external system integration
- [ ] **Authentication & Authorization** ([#23](https://github.com/aycee247/AutoLotManagerWPF/issues/23)): User login and role-based access control

#### Developer Experience
- [ ] **CI/CD Pipeline** ([#24](https://github.com/aycee247/AutoLotManagerWPF/issues/24)): Automated build, test, and deployment
- [ ] **Code Documentation** ([#25](https://github.com/aycee247/AutoLotManagerWPF/issues/25)): XML documentation comments for all public APIs
- [ ] **Style Guide** ([#26](https://github.com/aycee247/AutoLotManagerWPF/issues/26)): Coding standards and conventions document
- [ ] **Docker Support** ([#27](https://github.com/aycee247/AutoLotManagerWPF/issues/27)): Containerization for easier development and deployment

### Ideas for Future Features

Speculative, not committed work — each is tracked as an issue for discussion.

- **Vehicle History Tracking** ([#28](https://github.com/aycee247/AutoLotManagerWPF/issues/28)): Maintenance records, service history, accident reports
- **Photo Management** ([#29](https://github.com/aycee247/AutoLotManagerWPF/issues/29)): Upload and display vehicle photos
- **Pricing Calculator** ([#30](https://github.com/aycee247/AutoLotManagerWPF/issues/30)): Automated pricing based on market data and vehicle condition
- **Email Notifications** ([#31](https://github.com/aycee247/AutoLotManagerWPF/issues/31)): Automated alerts for low inventory, pending sales, etc.
- **Barcode/QR Code Integration** ([#32](https://github.com/aycee247/AutoLotManagerWPF/issues/32)): Quick vehicle lookup and tracking
- **Mobile Companion App** ([#33](https://github.com/aycee247/AutoLotManagerWPF/issues/33)): MAUI-based mobile application
- **Cloud Sync** ([#34](https://github.com/aycee247/AutoLotManagerWPF/issues/34)): Multi-device synchronization via cloud storage
- **Audit Trail** ([#35](https://github.com/aycee247/AutoLotManagerWPF/issues/35)): Track all changes to inventory and sales data
- **Financial Dashboard** ([#36](https://github.com/aycee247/AutoLotManagerWPF/issues/36)): Integration with accounting systems
- **Customer Portal** ([#37](https://github.com/aycee247/AutoLotManagerWPF/issues/37)): Self-service portal for customers to view inventory

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit your changes** (`git commit -m 'Add some AmazingFeature'`)
4. **Push to the branch** (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

Please ensure your code:
- Follows the existing code style and conventions
- Includes appropriate documentation
- Works with the existing architecture
- Does not break existing functionality

## 📄 License

This project is provided as-is for educational and demonstration purposes.

---

**Note**: This is a demonstration/portfolio project showcasing WPF, MVVM, and modern C# development practices. The "Auto Lot" is fictional, and the software is designed to demonstrate technical capabilities rather than for production use.
