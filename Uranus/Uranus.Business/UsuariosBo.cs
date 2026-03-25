using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class UsuariosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Usuarios usuario)
        {
            using (var context = new UranusEntities())
            {
                context.Usuarios.Add(usuario);
                context.SaveChanges();

                return usuario.ID;
            }
        }

        public static void Salvar(Usuarios usuario)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(usuario).State = EntityState.Modified;

                if (String.IsNullOrEmpty(usuario.Nome))
                {
                    context.Entry(usuario).Property(x => x.Nome).IsModified = false;
                }

                if (String.IsNullOrEmpty(usuario.Login))
                {
                    context.Entry(usuario).Property(x => x.Login).IsModified = false;
                }

                if (String.IsNullOrEmpty(usuario.Senha))
                {
                    context.Entry(usuario).Property(x => x.Senha).IsModified = false;
                }

                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var usuario = context.Usuarios.Find(id);
                    if (usuario == null)
                        return "99";
                    usuario.Bloqueio = true;
                    context.Entry(usuario).Property(x => x.Bloqueio).IsModified = true;
                    context.SaveChanges();
                }
                return "00";
            }
            catch (Exception ex)
            {
                String error = "99";
                String message = ex.InnerException?.ToString() ?? ex.Message;
                if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    error = "98";
                return error;
            }
        }

        //public static string Excluir(Int32 id)
        //{
        //    try
        //    {
        //        using (var context = new UranusEntities())
        //        {
        //            var usuario = context.Usuarios.Find(id);
        //            context.Usuarios.Attach(usuario);
        //            context.Usuarios.Remove(usuario);
        //            context.SaveChanges();
        //        }

        //        return "00";
        //    }
        //    catch (Exception ex)
        //    {
        //        String error = "99";
        //        String message = ex.InnerException.ToString();

        //        if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
        //            error = "98";

        //        return error;
        //    }
        //}

        public static List<Usuarios> Listar(string search) 
        {
            if (String.IsNullOrEmpty(search.Trim()))
            {
                using (var context = new UranusEntities())
                {
                    var query = from d in context.Usuarios
                                where d.Senha != null
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
            else
            {
                using (var context = new UranusEntities())
                {
                    var query = from d in context.Usuarios
                                where d.Nome.Contains(search)
                                where d.Senha != null
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<Usuarios> ListarTodos()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios
                            orderby d.Nome ascending
                            select d;

                return query.ToList();
            }
        }

        public static Array ListarArray()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios
                            where d.Bloqueio == false
                            where d.ID > 8
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static Int32 Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios
                            where d.Bloqueio == false
                            select d;

                return query.Count();
            }
        }

        public static Usuarios Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                Login = d.Login,
                                Bloqueio = d.Bloqueio,
                                Nivel = d.Nivel
                            };

                return query.ToArray();
            }
        }

        public static Usuarios Validar(String Login, String Senha)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios.Include("Profissionais")
                                                      .Include("Profissionais.Pessoas")
                            where d.Login == Login
                            where d.Senha == Senha
                            where d.Bloqueio == false
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Usuarios BuscarUsuarioPorLogin(String Login)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Usuarios.Include("Profissionais")
                                                      .Include("Profissionais.Pessoas")
                            where d.Login == Login
                            where d.Bloqueio == false
                            select d;

                return query.FirstOrDefault();
            }
        }

        // Manter o método com nome antigo para compatibilidade
        public static Usuarios BuscarUusarioPorLogin(String Login)
        {
            return BuscarUsuarioPorLogin(Login);
        }

        public static String Buscar(Int64 Id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var query = from d in context.Usuarios.Include("Profissionais")
                                                          .Include("Profissionais.Pessoas")
                                                          .Include("Profissionais.Pessoas.Email")
                                where d.ID == Id
                                where d.Profissionais.FirstOrDefault().Pessoas.Email.FirstOrDefault().Ativo == true
                                where d.Profissionais.FirstOrDefault().Pessoas.Email.FirstOrDefault().Principal == true
                                select d;

                    var email = query?.FirstOrDefault()?.Profissionais?.FirstOrDefault().Pessoas?.Email?.FirstOrDefault()?.Email1;

                    return email;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Ativar(Int32 id)
        {
            using (var context = new UranusEntities())
            {
                var usuario = context.Usuarios.Find(id);
                if (usuario != null)
                {
                    usuario.Bloqueio = false;
                    context.Entry(usuario).Property(x => x.Bloqueio).IsModified = true;
                    context.SaveChanges();
                }
            }
        }
    }
}