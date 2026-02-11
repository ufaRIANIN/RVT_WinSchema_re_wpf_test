using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RVT_WinSchema_re_wpf.Models;
using RVT_WinSchema_re_wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RevitView = Autodesk.Revit.DB.View;


namespace RVT_WinSchema_re_wpf
{
    public class EventRegHandler : IExternalEventHandler
    {
        public bool EventRegistered { get; set; }

        public WindowSchemaSettings Settings { get; set; } = new WindowSchemaSettings();

        public void Execute(UIApplication uiapp)
        {
            try
            {
                var revitService = ServiceLocator.Instance.Single<IRevitItemsService>();
                UIDocument uidoc = revitService.GetUIDocument();
                Document doc = revitService.GetDocument();

                using (Transaction t = new Transaction(doc, "Схемы окон"))
                {
                    t.Start();

                    // 1️ Находим вид "Схемы окон"
                    RevitView windowSchemaView = new FilteredElementCollector(doc)
                        .OfClass(typeof(RevitView))
                        .Cast<RevitView>()
                        .FirstOrDefault(v => v.Name == "Схемы окон");

                    if (windowSchemaView == null)
                    {
                        TaskDialog.Show("Ошибка", "Вид 'Схемы окон' не найден");
                        t.RollBack();
                        return;
                    }

                    uidoc.ActiveView = windowSchemaView;

                    // 2️ Получаем все экземпляры окон
                    var windowInstances = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .ToList();

                    var windowTypeNames = new HashSet<string>(windowInstances.Select(w => w.Symbol.Name));

                    // 3️ Получаем все типы окон
                    var allWindowTypes = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .WhereElementIsElementType()
                        .Cast<FamilySymbol>()
                        .Where(ft => ft.IsActive && ft.FamilyName.Contains("Окно"))
                        .ToList();

                    // 4️ Отбираем только используемые типы
                    var windowTypes = allWindowTypes
                        .Where(ft => windowTypeNames.Contains(ft.Name))
                        .ToList();

                    // 5️ Получаем элементы легенды
                    var legendComponents = new FilteredElementCollector(doc, windowSchemaView.Id)
                        .OfCategory(BuiltInCategory.OST_LegendComponents)
                        .ToList();

                    var textNotes = new FilteredElementCollector(doc, windowSchemaView.Id)
                        .OfCategory(BuiltInCategory.OST_TextNotes)
                        .ToList();

                    var detailCurves = new FilteredElementCollector(doc, windowSchemaView.Id)
                        .OfClass(typeof(CurveElement))
                        .ToList();

                    // 6️ Удаляем старые примечания и линии
                    foreach (var tn in textNotes) doc.Delete(tn.Id);
                    foreach (var dc in detailCurves) doc.Delete(dc.Id);

                    // 7️ Удаляем лишние элементы легенды, оставляем один
                    if (legendComponents.Count > 1)
                    {
                        for (int i = 1; i < legendComponents.Count; i++)
                            doc.Delete(legendComponents[i].Id);
                    }

                    var firstLegend = legendComponents.FirstOrDefault();
                    if (firstLegend == null)
                    {
                        TaskDialog.Show("Ошибка", "Легенда не найдена");
                        t.RollBack();
                        return;
                    }

                    // 8️ Создаем группы для всех типов окон
                    double deltaX = 0;
                    foreach (var winType in windowTypes)
                    {
                        Group group = doc.Create.NewGroup(new List<ElementId> { firstLegend.Id });
                        LocationPoint loc = group.Location as LocationPoint;
                        XYZ newPoint = new XYZ(loc.Point.X + deltaX, loc.Point.Y, loc.Point.Z);
                        Group newGroup = doc.Create.PlaceGroup(newPoint, group.GroupType);
                        group.UngroupMembers();
                        newGroup.UngroupMembers();
                        deltaX += 7;
                    }

                    // 9️ Удаляем исходный элемент легенды
                    doc.Delete(firstLegend.Id);

                    // 10️ Получаем обновленные элементы легенды
                    var newLegendComponents = new FilteredElementCollector(doc, windowSchemaView.Id)
                        .OfCategory(BuiltInCategory.OST_LegendComponents)
                        .ToList();

                    // 11️ Заполняем легенду параметрами
                    for (int i = 0; i < newLegendComponents.Count; i++)
                    {
                        var comp = newLegendComponents[i];
                        comp.LookupParameter("Тип компонента").Set(windowTypes[i].Id);

                        // 12️ Вычисляем площадь, если включено
                        if (Settings.CalculateArea)
                        {
                            double width = windowTypes[i].LookupParameter("Примерная ширина").AsDouble();
                            double height = windowTypes[i].LookupParameter("Примерная высота").AsDouble();

                            // 13️ Добавляем монтажные зазоры, если включено
                            if (Settings.IncludeGaps)
                            {
                                width += UnitUtils.ConvertToInternalUnits(15, UnitTypeId.Millimeters);
                                height += UnitUtils.ConvertToInternalUnits(15, UnitTypeId.Millimeters);
                            }

                            string areaStr = $"S = {Math.Round((UnitUtils.ConvertFromInternalUnits(width, UnitTypeId.Millimeters) * UnitUtils.ConvertFromInternalUnits(height, UnitTypeId.Millimeters)) / 1000000, 2)} м²";

                            BoundingBoxXYZ bb = comp.get_BoundingBox(windowSchemaView);
                            XYZ squareLocation = new XYZ((bb.Min.X + bb.Max.X) / 2, bb.Min.Y + height + 0.6, 0);

                            TextNote.Create(doc, windowSchemaView.Id, squareLocation, areaStr, new TextNoteOptions(new ElementId(8047)));
                        }
                    }

                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }
        }

        public string GetName() => "EventRegHandler";
    }
}
