using System.Data.Entity;
using System;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AppGerarTokenBo : IRequiresSessionState
    {
        public static AppConfiguracao Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = context.Database.SqlQuery<AppConfiguracao>("SELECT * FROM AppConfiguracao");

                return query.FirstOrDefault();
            }
        }

        public static Int32 Inserir(AppConfiguracao configuracao)
        {
            using (var context = new UranusEntities())
            {
                context.AppConfiguracao.Add(configuracao);
                context.SaveChanges();

                return configuracao.Id;
            }
        }

        public static void Salvar(AppConfiguracao configuracao)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(configuracao).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

    }
}