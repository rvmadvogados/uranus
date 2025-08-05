using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R9.DataBase
{
    public class Table : Attribute
    {
        public string Name;
        public bool ReadOnly;
        public bool NoLock;

        public Table(string tableName, bool readOnly = false, bool NoLock = true)
        {
            this.Name = tableName;
            this.ReadOnly = readOnly;
            this.NoLock = NoLock;
        }
    }

    public class Query : Attribute
    {
        public string QueryText;
        public bool ReadOnly;
        public string OrderBy;

        public Query(string Query, string OrderBy)
        {
            this.QueryText = Query;
            this.ReadOnly = true;
            this.OrderBy = OrderBy;
        }
    }
}
