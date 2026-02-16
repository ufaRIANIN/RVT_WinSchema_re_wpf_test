using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RVT_WinSchema_re_wpf.Helpers;
using RVT_WinSchema_re_wpf.Models;
using RVT_WinSchema_re_wpf.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RVT_WinSchema_re_wpf.ViewModels
{
    public class MainViewModel : BaseModel
    {
        // Чекбоксы

        private bool _calculateArea = true;
        public bool CalculateArea
        {
            get => _calculateArea;
            set
            {
                _calculateArea = value;
                NotifyPropertyChanged();
            }
        }

        private bool _includeGaps = true;
        public bool IncludeGaps
        {
            get => _includeGaps;
            set
            {
                _includeGaps = value;
                NotifyPropertyChanged();
            }
        }


        // Список видов

        public ObservableCollection<RevitViewModel> Views { get; }
            = new ObservableCollection<RevitViewModel>();


        private RevitViewModel _selectedView;
        public RevitViewModel SelectedView
        {
            get => _selectedView;
            set
            {
                _selectedView = value;
                NotifyPropertyChanged();
            }
        }


        // Команда

        public ICommand GenerateSchemaCommand { get; }


        // Revit

        private EventRegHandler _eventHandler;
        private ExternalEvent _externalEvent;


        // Конструктор

        public MainViewModel()
        {
            // Обработчик
            _eventHandler = new EventRegHandler();

            // ExternalEvent
            _externalEvent = ExternalEvent.Create(_eventHandler);

            // Команда
            GenerateSchemaCommand = new RelayCommand(RaiseEvent);

            // Загружаем виды
            LoadViews();
        }


        //  Загрузка видов 

        private void LoadViews()
        {
            var service = ServiceLocator.Instance.Single<IRevitItemsService>();

            if (service == null)
                return;

            var doc = service.GetDocument();

            var views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => !v.IsTemplate)
                .OrderBy(v => v.Name);

            Views.Clear();

            foreach (var v in views)
            {
                Views.Add(new RevitViewModel
                {
                    Id = v.Id.IntegerValue,
                    Name = v.Name
                });
            }

            // По умолчанию выбираем первый
            SelectedView = Views.FirstOrDefault();
        }


        // Запуск

        private void RaiseEvent()
        {
            if (SelectedView == null)
                return;

            // Передаём настройки
            _eventHandler.Settings = new WindowSchemaSettings
            {
                CalculateArea = CalculateArea,
                IncludeGaps = IncludeGaps
            };

            // Передаём выбранный вид
            _eventHandler.SelectedViewId = SelectedView.Id;

            // Запуск
            _externalEvent.Raise();
        }
    }
}