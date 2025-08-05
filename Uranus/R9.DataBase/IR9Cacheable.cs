using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R9.DataBase
{
    public interface IR9Cacheable
    {
        DateTime ValidadeCache { get; }
        void AtualizarValidadeCache();
        int ID { get; set; }
    }
}
