using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class IniciaisPeriodosBO : IRequiresSessionState
    {
        public static List<TemplatesIniciaisAposentadoriaPeriodos> Listar(Int32 IdTemplate)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.TemplatesIniciaisAposentadoriaPeriodos
                            where d.IDTemplate == IdTemplate
                            select d;

                return query.ToList();
            }
        }
    }
}