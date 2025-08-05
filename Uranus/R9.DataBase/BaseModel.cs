using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R9.DataBase
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Browsable(false)]
        public DataContext Context { get; set; }

        [Browsable(false)]
        internal List<string> PKNames
        {
            get
            {
                return GetAllPKColumnName();
            }
        }

        [Browsable(false)]
        internal List<string> IdentityNames
        {
            get
            {
                return GetAllPKColumnName();
            }
        }

        [Browsable(false)]
        internal List<object> PKValues
        {
            get
            {
                return GetAllPKValues();
            }
        }

        [Browsable(false)]
        public bool HasID
        {
            get
            {
                foreach (var v in GetAllPKValues())
                {
                    if (v != null)
                    {
                        if (v is int)
                        {
                            if ((int)v != 0)
                            {
                                return true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (v is long)
                        {
                            if ((long)v != 0)
                            {
                                return true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (v is decimal)
                        {
                            if ((decimal)v != 0)
                            {
                                return true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

        }

        [Browsable(false)]
        public Dictionary<string, object> Keys
        {
            get
            {
                Dictionary<string, object> keys = new Dictionary<string, object>();

                PropertyInfo[] properties = GetProps();

                foreach (var prop in properties)
                {
                    if (prop.GetCustomAttributes(false).Where(a => a is PK).FirstOrDefault() != null)
                    {
                        keys.Add(prop.Name, prop.GetValue(this));
                    }
                }

                return keys;
            }
        }

        
        public List<PropertyInfo> GetAutoLinks()
        {
            List<PropertyInfo> keys = new List<PropertyInfo>();

            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is Autolink).FirstOrDefault() != null)
                { 
                    keys.Add(prop);
                }
            }

            return keys;
        }

        private List<string> GetAllPKColumnName()
        {
            List<string> keys = new List<string>();

            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is PK).FirstOrDefault() != null)
                {
                    keys.Add(prop.Name);
                }
            }

            return keys;
        }

        private List<PropertyInfo> GetFKs()
        {
            List<PropertyInfo> keys = new List<PropertyInfo>();

            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is FK).FirstOrDefault() != null)
                {
                    keys.Add(prop);
                }
            }

            return keys;
        }

        private List<string> GetAllIdentityColumnsNames()
        {
            List<string> keys = new List<string>();

            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is Identity).FirstOrDefault() != null)
                {
                    keys.Add(prop.Name);
                }
            }

            return keys;
        }

        private List<object> GetAllPKValues()
        {
            List<object> values = new List<object>();

            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is PK).FirstOrDefault() != null)
                {
                    var value = prop.GetValue(this);

                    values.Add(value);
                }
            }

            return values;
        }

        public class UIProperty
        {
            public string UIName { get; set; }
            public string SQLName { get; set; }

            public UIProperty(string uIName, string sQLName)
            {
                UIName = uIName;
                SQLName = sQLName;
            }
        }

        public List<UIProperty> GetUIProperties()
        {
            List<UIProperty> values = new List<UIProperty>();

            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                var customAttr = prop.GetCustomAttributes(false).Where(a => a is DataElement && ((a as DataElement).Friendly)).FirstOrDefault();

                if (customAttr != null)
                {
                    var SQLName = prop.Name;
                    var UIName = (customAttr as DataElement).FriendlyName;

                    values.Add(new UIProperty(UIName, SQLName));
                }
            }

            return values;
        }

        internal void ResetKeys()
        {
            var props = GetProps();

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is PK).FirstOrDefault() != null)
                {
                    prop.SetValue(this, null, null);
                }
            }
        }

        private PropertyInfo[] GetProps()
        {
            Dictionary<string, object> _ParamValues = new Dictionary<string, object>();

            PropertyInfo[] properties = this.GetType().GetProperties();

            return properties;
        }

        internal object GetID()
        {
            PropertyInfo[] properties = GetProps();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is AutoLinkAnchor).FirstOrDefault() != null)
                {
                    return prop.GetValue(this);
                }
            }

            return 0;
        }

        public virtual void ResetLists()
        {
            

        }

        public static string AddSpaces(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

    }
}
