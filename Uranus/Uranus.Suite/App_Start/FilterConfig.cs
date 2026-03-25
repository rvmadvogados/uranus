using System.Web.Mvc;
using Uranus.Suite.Filters;

namespace Uranus.Suite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}