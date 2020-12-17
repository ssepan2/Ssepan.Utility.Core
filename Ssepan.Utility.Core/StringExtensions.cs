using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ssepan.Utility
{
    public static class StringExtensions
    {
        /// <summary>
        /// Perform string.Contains(value) with case-sensitivity explicitly on or off.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="caseInsensitive"></param>
        /// <returns></returns>
        public static Boolean Contains(this String source, String value, Boolean caseInsensitive)
        {

            if (caseInsensitive)
            {
                Int32 results = source.IndexOf(value, StringComparison.CurrentCultureIgnoreCase);
                return results == -1 ? false : true;
            }
            else
            {
                return source.Contains(value);
            }

        }
    }
}
