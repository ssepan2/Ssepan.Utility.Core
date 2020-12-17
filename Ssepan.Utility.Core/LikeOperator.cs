using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;

namespace Ssepan.Utility
{
    /// <summary>
    /// http://www.devexpertise.com/category/net/linq/
    /// </summary>
    public static class LikeOperator
    {
        /// <summary>
        /// Ex: var results = (from v in values where v.Like("*a*a*") select v);
        /// </summary>
        /// <param name="value"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static bool Like(this String value, String term)
        {
            Boolean returnValue = default(bool);
            Regex regex = default(Regex);

            try
            {
                regex = new Regex(String.Format("^{0}$", term.Replace("*", ".*")), RegexOptions.IgnoreCase);
                returnValue = regex.IsMatch(value ?? string.Empty);
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
                throw;
            }
            return returnValue;
        }

        /// <summary>
        /// Ex: var results = values.Like("*a*a*");
        /// </summary>
        /// <param name="source"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<String> Like(this IEnumerable<String> source, String expression)
        {
            IEnumerable<String> returnValue = default(IEnumerable<String>);

            try
            {
                returnValue = (from s in source where s.Like(expression) select s);
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
                throw;
            }
            return returnValue;
        }
    }
}
