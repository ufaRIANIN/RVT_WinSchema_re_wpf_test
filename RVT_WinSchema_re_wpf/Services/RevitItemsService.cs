using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RVT_WinSchema_re_wpf.Services
{
    public interface IRevitItemsService : IService
    {
        UIApplication GetUIApplication();
        Application GetApplication();
        Document GetDocument();
        UIDocument GetUIDocument();
    }

    public class RevitItemsService : IRevitItemsService
    {
        public RevitItemsService(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _app = uiapp.Application;
            _uidoc = uiapp.ActiveUIDocument;
            _doc = uiapp.ActiveUIDocument.Document;
        }

        private UIApplication _uiapp;
        private Application _app;
        private Document _doc;
        private UIDocument _uidoc;
        public UIApplication GetUIApplication()
        {
            return _uiapp;
        }

        public Application GetApplication()
        {
            return _app;
        }

        public Document GetDocument()
        {
            return _doc;
        }

        public UIDocument GetUIDocument()
        {
            return _uidoc;
        }
    }
}
