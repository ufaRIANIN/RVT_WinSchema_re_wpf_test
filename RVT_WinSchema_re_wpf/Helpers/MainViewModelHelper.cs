using RVT_WinSchema_re_wpf.ViewModels;

namespace RVT_WinSchema_re_wpf.Helpers
{
    /// <summary>
    /// Класс для создания и хранения VM для будущей подвязки к View
    /// </summary>
    public static class MainViewModelHelper
    {
        public static MainViewModel MainViewModel { get; private set; }
        public static void Constuct()
        {
            MainViewModel = new MainViewModel();
        }
    }
}
