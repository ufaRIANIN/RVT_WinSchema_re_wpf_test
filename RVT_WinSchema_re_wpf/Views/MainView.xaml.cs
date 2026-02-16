using RVT_WinSchema_re_wpf.Helpers;
using System.Windows;

namespace RVT_WinSchema_re_wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            DataContext = MainViewModelHelper.MainViewModel;
        }
    }
}
