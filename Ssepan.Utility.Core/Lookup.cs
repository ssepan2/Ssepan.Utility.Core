using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Ssepan.Utility
{
    public class Lookup
    {
        public Lookup()
        {
        }

        public Lookup
        (
            String value,
            String text
        ) :
            this()
        {
            Value = value;
            Text = text;
        }

        private String _Value = default(String);
        public String Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private String _Text = default(String);
        public String Text
        {
            get { return _Text; }
            set { _Text = value; }
        }


        public static List<Lookup> GetEnumLookup<TEnum>(Boolean addNotSelectedItem = false)
        {
            List<Lookup> returnValue = default(List<Lookup>);

            try
            {
                returnValue = (from Enum value in Enum.GetValues(typeof(TEnum))
                               select new Lookup(value.ToString(), value.ToString())).ToList();
                if (addNotSelectedItem)
                {
                    returnValue.Insert(0, new Lookup(String.Empty, "(Not Selected)"));
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
            return returnValue;
        }
    }
}
