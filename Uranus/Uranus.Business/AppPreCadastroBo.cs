using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AppPreCadastroBo : IRequiresSessionState
    {
        public static void Salvar(PreCadastro cadastro)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(cadastro).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static PreCadastro Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PreCadastro
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<PreCadastro> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PreCadastro
                            where d.IdCliente == null
                            select d;

                return query.ToList();
            }
        }
    }
}