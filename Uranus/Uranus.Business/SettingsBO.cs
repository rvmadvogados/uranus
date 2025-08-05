using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class SettingsBO : IRequiresSessionState
    {
        public static List<Settings> Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Settings
                            select d;

                return query.ToList();
            }
        }
    }
}