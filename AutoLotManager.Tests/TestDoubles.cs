using System.Windows.Controls;

namespace AutoLotManager.Tests
{
    /// <summary>
    /// A bare Page with no XAML. The application's real pages resolve MaterialDesign and
    /// MahApps resource dictionaries declared in App.xaml, so constructing one without a
    /// running Application fails for reasons that have nothing to do with NavigationService.
    /// These doubles keep the navigation tests about navigation.
    /// </summary>
    public class TestPage : Page
    {
    }

    public class AnotherTestPage : Page
    {
    }

    public class TestViewModel
    {
    }

    /// <summary>
    /// Not a Page — used to verify RegisterPage rejects types that cannot be navigated to.
    /// </summary>
    public class NotAPage
    {
    }
}
