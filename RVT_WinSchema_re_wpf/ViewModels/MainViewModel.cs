using RVT_WinSchema_re_wpf.Models;
using RVT_WinSchema_re_wpf.Helpers;
using RVT_WinSchema_re_wpf.Services;
using Autodesk.Revit.UI;
using System.Windows.Input;

namespace RVT_WinSchema_re_wpf.ViewModels
{
    public class MainViewModel : BaseModel
    {
        private bool _calculateArea = true;
        public bool CalculateArea
        {
            get => _calculateArea;
            set { _calculateArea = value; NotifyPropertyChanged(); }
        }

        private bool _includeGaps = true;
        public bool IncludeGaps
        {
            get => _includeGaps;
            set { _includeGaps = value; NotifyPropertyChanged(); }
        }

        public ICommand GenerateSchemaCommand { get; }

        private EventRegHandler _eventHandler;
        private ExternalEvent _externalEvent;

        public MainViewModel()
        {
            // Создаём обработчик
            _eventHandler = new EventRegHandler();

            // Создаём ExternalEvent для Revit API
            _externalEvent = ExternalEvent.Create(_eventHandler);

            // Команда кнопки
            GenerateSchemaCommand = new RelayCommand(RaiseEvent);
        }

        private void RaiseEvent()
        {
            // Передаём настройки
            _eventHandler.Settings = new WindowSchemaSettings
            {
                CalculateArea = this.CalculateArea,
                IncludeGaps = this.IncludeGaps
            };

            // Запускаем обработчик через ExternalEvent
            _externalEvent.Raise();
        }
    }
}