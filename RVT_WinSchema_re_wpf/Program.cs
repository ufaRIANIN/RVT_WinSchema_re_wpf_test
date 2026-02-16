using Autodesk.Revit.UI;
using Newtonsoft.Json;
using RVT_WinSchema_re_wpf.Helpers;
using RVT_WinSchema_re_wpf.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RVT_WinSchema_re_wpf
{
    internal static class Program
    {
        private static UIApplication uiapp;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(UIApplication uiapp, ExternalEvent _exEvent, List<string> ParametersInElements) //это точка входа в приложение, поэтому тут передаем все необходимые данные для работы
        {
            Program.uiapp = uiapp;
            try
            {
                string ExAddinCache_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                $"\\Autodesk\\Revit\\ExAdd_Cache\\{Assembly.GetExecutingAssembly().GetName().Name}";

                if (File.Exists(ExAddinCache_path + "//ParametersMapping.json"))
                {
                    Info.ParametersMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ExAddinCache_path + "//ParametersMapping.json"));
                }

                bool MappingError = false;
                foreach (var param in Info.ParametersMapping)
                {
                    if (!ParametersInElements.Contains(param.Value))
                        MappingError = true;
                }

                ParametersInElements.Sort();

                if (!MappingError)
                {
                    //запуск приложения

                    RegisterServices();
                    ConstructVewModel();

                    //поднятие события плагина
                    _exEvent.Raise();
                }
                else
                {
                    MessageBox.Show("Необходимо настроить маппинг параметров для данного плагина.", "Некорректный маппинг",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void RegisterServices()
        {
            ServiceLocator.Instance.RegisterSingle<IRevitItemsService, RevitItemsService>(new RevitItemsService(uiapp));
        }

        private static void ConstructVewModel() // Создание view models
        {
            // Создаём VM
            MainViewModelHelper.Constuct();

            //// Создаём WPF окно
            //var mainView = new Views.MainView
            //{
            //    DataContext = MainViewModelHelper.MainViewModel
            //};

            //// Показываем окно модально, если нужно
            //mainView.ShowDialog();
        }
    }
}
