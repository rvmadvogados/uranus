using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AuditoriaBo : IRequiresSessionState
    {
        public static void Inserir(Auditoria auditoria)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Auditoria.Add(auditoria);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

 //       public static List<View_AuditoriaListar> Listar(string data, string numeroProcesso, string modulo, string tipo, string acao, string usuario, string nome)
        public static List<Auditoria> Listar(string data, string numeroProcesso, string modulo, string tipo, string acao, string usuario, string nome)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                string[] datas;
                DateTime datainicial;
                DateTime datafinal;

                if (!String.IsNullOrEmpty(data.Trim()))
                {
                    datas = data.Replace(" ", "").Split('-');
                    datainicial = DateTime.Parse(datas[0]);
                    datafinal = DateTime.Parse(datas[1] + " 23:59:59");

//                    var query = from d in context.View_AuditoriaListar
                    var query = from d in context.Auditoria
                                where d.DataHora >= datainicial && d.DataHora <= datafinal
                                where d.Log.Contains(numeroProcesso)
                                where d.Log.Contains(nome)
                                where d.Modulo.Contains(modulo)
                                where d.Tipo.Contains(tipo)
                                where d.Acao.Contains(acao)
                                where d.Usuario.Contains(usuario)
                                orderby d.DataHora descending
                                select d;

                    return query.ToList();
                }
                else
                {
                    context.Database.CommandTimeout = 360;

//                    var query = from d in context.View_AuditoriaListar
                    var query = from d in context.Auditoria
                                where d.Modulo.Contains(modulo)
                                where d.Log.Contains(numeroProcesso)
                                where d.Log.Contains(nome)
                                where d.Tipo.Contains(tipo)
                                where d.Acao.Contains(acao)
                                where d.Usuario.Contains(usuario)
                                orderby d.DataHora descending
                                select d;

                    return query.Take(500).ToList();
                }
            }
        }


        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
//                var query = from d in context.View_AuditoriaConsultar
                var query = from d in context.Auditoria
                            where d.Id == Id
                            select new
                            {
                                DataHora = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataHora), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataHora), 2) + "/" + SqlFunctions.DatePart("year", d.DataHora).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataHora), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataHora), 2),
                                Modulo = d.Modulo,
                                Tipo = d.Tipo,
                                Acao = d.Acao,
                                Usuario = d.Usuario,
                                Log = d.Log
                            };

                return query.ToArray();
            }
        }
    }
}