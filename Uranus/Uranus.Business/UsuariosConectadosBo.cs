using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class UsuariosConectadosBo : IRequiresSessionState
    {
        public static void Inserir(UsuariosConectados conectado)
        {
            using (var context = new UranusEntities())
            {
                context.UsuariosConectados.Add(conectado);
                context.SaveChanges();
            }
        }

        public static void Salvar(UsuariosConectados conectado)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(conectado).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static UsuariosConectados Consultar(String IP, Int32 IdUsuario, String SistemaOperacional, String Navegador)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.UsuariosConectados
                            where d.IP == IP
                            where d.IdUsuario == IdUsuario
                            where d.SistemaOperacional == SistemaOperacional
                            where d.Navegador == Navegador
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Int32 Consultar()
        {
            using (var context = new UranusEntities())
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddSeconds(-150);

                var query = from d in context.UsuariosConectados
                            where (d.DataHora >= startDate && d.DataHora < endDate)
                            select d;

                return query.Count();
            }
        }

        public static List<UsuariosLogados> ListarUsuariosConectados()
        {
            using (var context = new UranusEntities())
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddSeconds(-150);

                var query = (from d in context.UsuariosConectados.Include("Usuarios")
                             where (d.DataHora >= startDate && d.DataHora < endDate)
                             orderby d.Usuarios.Nome ascending
                             group d by d.Usuarios.Nome into g
                             select new
                             {
                                 Usuario = g.Key,
                                 Total = g.Count()
                             }).ToList();

                List<UsuariosLogados> logados = new List<UsuariosLogados>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    logados = JsonConvert.DeserializeObject<List<UsuariosLogados>>(jsonString);
                }

                return logados;
            }
        }

        public static List<SistemasOperacionais> ListarSistemasOperacionais()
        {
            using (var context = new UranusEntities())
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddSeconds(-150);

                var query = (from d in context.UsuariosConectados
                             where (d.DataHora >= startDate && d.DataHora < endDate)
                             orderby d.SistemaOperacional ascending
                             group d by d.SistemaOperacional into g
                             select new
                             {
                                 Nome = g.Key,
                                 Total = g.Count(),
                                 Percentual = ((g.Count() * 100) / (from p in context.UsuariosConectados
                                                                    where (p.DataHora >= startDate && p.DataHora < endDate)
                                                                    select p).Count())
                             }).Take(5).ToList();

                List<SistemasOperacionais> sistemas = new List<SistemasOperacionais>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    sistemas = JsonConvert.DeserializeObject<List<SistemasOperacionais>>(jsonString);
                }

                return sistemas;
            }
        }

        public static List<Navegadores> ListarNavegadores()
        {
            using (var context = new UranusEntities())
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddSeconds(-150);

                var query = (from d in context.UsuariosConectados
                             where (d.DataHora >= startDate && d.DataHora < endDate)
                             orderby d.Navegador ascending
                             group d by d.Navegador into g
                             select new
                             {
                                 Nome = g.Key,
                                 Total = g.Count(),
                                 Percentual = ((g.Count() * 100) / (from p in context.UsuariosConectados
                                                                    where (p.DataHora >= startDate && p.DataHora < endDate)
                                                                    select p).Count())
                             }).Take(5).ToList();

                List<Navegadores> navegadores = new List<Navegadores>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    navegadores = JsonConvert.DeserializeObject<List<Navegadores>>(jsonString);
                }

                return navegadores;
            }
        }
    }
}