using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.SessionState;
using System.Text;

namespace Uranus.Business
{
    public class EstadosBo : IRequiresSessionState
    {
        public class Estados
        {
            public string nome { get; set; }
            public string sigla { get; set; }
        }

        public static Array ListarArray(string path)
        {
            Array estados;

            using (StreamReader r = new StreamReader(path, Encoding.Default, true))
            {
                string result = r.ReadToEnd();
                estados = JsonConvert.DeserializeObject<List<Estados>>(result).ToArray();
            }

            return estados;
        }
    }
}