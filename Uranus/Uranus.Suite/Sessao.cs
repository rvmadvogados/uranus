using System.Collections.Generic;
using System.Web;
using Uranus.Domain;
using Uranus.Domain.Entities;
using Uranus.Suite.ViewModels;

namespace Uranus.Suite
{
    public static class Sessao
    {
        public static bool IsSessionValid
        {
            get
            {
                return HttpContext.Current?.Session != null
                    && HttpContext.Current.Session["Usuario"] != null;
            }
        }

        public static Usuarios Usuario
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["Usuario"] as Usuarios;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Usuario"] = value;
            }
        }

        public static string Aplicativo
        {
            get
            {
                if (HttpContext.Current?.Session == null) return "Suite";
                var aplicativo = HttpContext.Current.Session["Aplicativo"];
                return aplicativo != null ? aplicativo.ToString() : "Suite";
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Aplicativo"] = value;
            }
        }

        public static Connected Conectado
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["Conectados"] as Connected;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Conectados"] = value;
            }
        }

        public static List<Settings> Setting
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["Setting"] as List<Settings>;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Setting"] = value;
            }
        }

        public static string ProcessRowIndex
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["ProcessRowIndex"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["ProcessRowIndex"] = value;
            }
        }

        public static string ProcessNumber
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["ProcessNumber"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["ProcessNumber"] = value;
            }
        }

        public static string ClientName
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["ClientName"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["ClientName"] = value;
            }
        }

        public static string AreaType
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["AreaType"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["AreaType"] = value;
            }
        }

        public static string ProcessStatus
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["ProcessStatus"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["ProcessStatus"] = value;
            }
        }

        public static string Judgment
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["Judgment"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Judgment"] = value;
            }
        }

        public static string URLParameters
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["URLParameters"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["URLParameters"] = value;
            }
        }

        public static string FeriadosRecesso
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["FeriadosRecesso"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["FeriadosRecesso"] = value;
            }
        }

        public static string Token
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["Token"] as string;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Token"] = value;
            }
        }

        public static List<UsuarioClaimDTO> Claims
        {
            get
            {
                if (HttpContext.Current?.Session == null) return null;
                return HttpContext.Current.Session["Claims"] as List<UsuarioClaimDTO>;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                    HttpContext.Current.Session["Claims"] = value;
            }
        }
    }
}