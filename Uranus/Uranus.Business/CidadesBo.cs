using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.SessionState;
using System.Text;
using System.Linq;

namespace Uranus.Business
{
    public class CidadesBo : IRequiresSessionState
    {
        public class Cidades
        {
            public string sigla { get; set; }
            public string cidade { get; set; }

        }

        public static Array ListarArray(string path, string uf)
        {
            Array cidades;

            using (StreamReader r = new StreamReader(path, Encoding.Default, true))
            {
                string result = r.ReadToEnd();

                cidades = JsonConvert.DeserializeObject<List<Cidades>>(result).Where(c => c.sigla.ToUpper().Contains(uf)).ToArray();
            }

            return cidades;
        }
    }
}