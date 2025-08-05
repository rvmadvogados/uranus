using System.Collections.Generic;
using System.Web;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Suite
{
    public static class Sessao
    {
        public static Usuarios Usuario
        {
            get
            {
                return (Usuarios)HttpContext.Current.Session["Usuario"];
            }
            set
            {
                HttpContext.Current.Session["Usuario"] = value;
            }
        }

        public static string Aplicativo
        {
            get
            {
                return HttpContext.Current.Session["Aplicativo"].ToString();
            }
            set
            {
                HttpContext.Current.Session["Aplicativo"] = value;
            }
        }

        public static Connected Conectado
        {
            get
            {
                return (Connected)HttpContext.Current.Session["Conectados"];
            }
            set
            {
                HttpContext.Current.Session["Conectados"] = value;
            }
        }

        public static List<Settings> Setting
        {
            get
            {
                return (List<Settings>)HttpContext.Current.Session["Setting"];
            }
            set
            {
                HttpContext.Current.Session["Setting"] = value;
            }
        }

        public static string ProcessRowIndex
        {
            get
            {
                return (string)HttpContext.Current.Session["ProcessRowIndex"];
            }
            set
            {
                HttpContext.Current.Session["ProcessRowIndex"] = value;
            }
        }

        public static string ProcessNumber
        {
            get
            {
                return (string)HttpContext.Current.Session["ProcessNumber"];
            }
            set
            {
                HttpContext.Current.Session["ProcessNumber"] = value;
            }
        }

        public static string ClientName
        {
            get
            {
                return (string)HttpContext.Current.Session["ClientName"];
            }
            set
            {
                HttpContext.Current.Session["ClientName"] = value;
            }
        }

        public static string AreaType
        {
            get
            {
                return (string)HttpContext.Current.Session["AreaType"];
            }
            set
            {
                HttpContext.Current.Session["AreaType"] = value;
            }
        }

        public static string ProcessStatus
        {
            get
            {
                return (string)HttpContext.Current.Session["ProcessStatus"];
            }
            set
            {
                HttpContext.Current.Session["ProcessStatus"] = value;
            }
        }

        public static string Judgment
        {
            get
            {
                return (string)HttpContext.Current.Session["Judgment"];
            }
            set
            {
                HttpContext.Current.Session["Judgment"] = value;
            }
        }

        public static string URLParameters
        {
            get
            {
                return (string)HttpContext.Current.Session["URLParameters"];
            }
            set
            {
                HttpContext.Current.Session["URLParameters"] = value;
            }
        }

        public static string FeriadosRecesso
        {
            get
            {
                return (string)HttpContext.Current.Session["FeriadosRecesso"];
            }
            set
            {
                HttpContext.Current.Session["FeriadosRecesso"] = value;
            }
        }

    }
}