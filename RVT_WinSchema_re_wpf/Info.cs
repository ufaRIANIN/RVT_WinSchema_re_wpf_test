using System.Collections.Generic;
using System.Drawing;

namespace RVT_WinSchema_re_wpf
{
    internal class Info
    {
        public static Dictionary<string, string> ParametersMapping = new Dictionary<string, string>
        {
            //{ "first", "first" },
            //{ "second", "second" }
        };
        //можно заполнять в удобном формате, например: { "УдобноеИменованиеПараметра", "ADSK_Этаж"}

        //public static List<string> NonSharedParameters = new List<string>()
        //{
        //    "first",
        //    "second"
        //}; // В будущем мы выпилим это свойство, все параметры должны доставаться через корневой в маппинге

        public static string Category()
        {
            return "Тестовые";
        }

        public static string Name()
        {
            return $"ТЕСТОВЫЕ ОКНА";
        }

        public static Bitmap[] Icon()
        {
            return new Bitmap[] {
                Properties.Resources.GN_Test_Light_32x32,
                Properties.Resources.GN_Test_Light_16x16
            }; //заменить иконки на нужные
        }

        public static Dictionary<string, string> UsedParameters()
        {
            return ParametersMapping;
        }

        //public static List<string> UsedNonSharedParameters()
        //{
        //    return NonSharedParameters;
        //}
    }
}
