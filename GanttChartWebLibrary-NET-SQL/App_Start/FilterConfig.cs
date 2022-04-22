using System.Web;
using System.Web.Mvc;

namespace GanttChartWebLibrary_NET_SQL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
