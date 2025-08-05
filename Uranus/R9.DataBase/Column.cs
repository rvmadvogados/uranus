using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R9.DataBase
{
    public class DataElement : Attribute
    {
        public bool Friendly { get; set; }
        public string FriendlyName { get; set; }

        public DataElement()
        {

        }

        public DataElement(bool friendly, string friendlyName)
        {
            Friendly = friendly;
            FriendlyName = friendlyName;
        }
    }

    public class Column : DataElement
    {
        public int Start { get; private set; }
        public int Size { get; private set; }        

        public Column()
        {

        }

        public Column(bool friendly, string friendlyName)
        {
            Friendly = friendly;
            FriendlyName = friendlyName;
        }

        public Column(int Start, int Size)
        {
            this.Start = Start;
            this.Size = Size;
        }
    }

    public class FK : Column       
    {
        public Type ReferencedType;
        public string OrderBy;

        public FK(Type ReferencedType, string OrderBy = "")
        {
            this.ReferencedType = ReferencedType;
            this.OrderBy = OrderBy;
        }
    }

    public class PK : Column
    {
        
    }

    public class ID : PK, AutoLinkAnchor
    {

    }

    public class SK : Column, AutoLinkAnchor
    {

    }

    public class Identity : Column
    {

    }

    public interface AutoLinkAnchor
    {

    }

    public class Autolink : Attribute
    {
        public Type ReferencedType;
        public string OrderBy;

        public Autolink(Type ReferencedType, string OrderBy = "")
        {
            this.ReferencedType = ReferencedType;
            this.OrderBy = OrderBy;
        }
    }
}
