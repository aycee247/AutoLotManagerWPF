using AutoLotManager.ViewModel;
using MahApps.Metro.Controls;
using System.Web.UI.WebControls;
using System.Windows;

namespace AutoLotManager.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel _vm;
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
            
        }
    }
}
