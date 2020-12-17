using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Ssepan.Utility
{
    public static class Configuration
    {
        /// <summary>
        /// Generic method to read a value that must also be parsed
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="connectionStringName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static Boolean ReadValue<T>(String settingName, out T setting) where T: struct
        {
            Boolean returnValue = default(Boolean);
            setting = default(T);

            try
            {

                Object settingObject = null;

                settingObject = ConfigurationManager.AppSettings[settingName];
                if (settingObject == null)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString was not found: {0}", settingName));
                }
                if (settingObject.ToString() == String.Empty)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString was empty: {0}", settingName));
                }
                //Note:unable to access TryParse() without reflection within generic class--SJS
                if (!Parsing.TryParseGeneric<T>(settingObject.ToString(), out setting))
                {
                    throw new ApplicationException(String.Format("Configuration connectionString '{0}' was incorrectly formatted: {1}", settingName, settingObject.ToString()));
                }

                returnValue = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to read a String value that does not need to be parsed
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static Boolean ReadString(String settingName, out String setting)
        {
            Boolean returnValue = default(Boolean);
            setting = default(String);

            try
            {

                Object settingObject = null;

                settingObject = ConfigurationManager.AppSettings[settingName];
                if (settingObject == null)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString was not found: {0}\r\nThis condition may not be an error if the connectionString is optional.\r\nThis warning may be unrelated to subsequent messages.", settingName));
                }
                if (settingObject.ToString() == String.Empty)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString was empty: {0}", settingName));
                }
                setting = settingObject.ToString();

                returnValue = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to specifically read a connection String value
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static Boolean ReadConnectionString
        (
            String connectionStringName, 
            out String connectionString,
            out String providerName
        )
        {
            Boolean returnValue = default(Boolean);
            Object settingObject = default(Object);
            connectionString = default(String);
            providerName = default(String);

            try
            {
                settingObject = ConfigurationManager.ConnectionStrings[connectionStringName];
                if (settingObject == null)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString (connection string) was not found: {0}", connectionStringName));
                }

                if (((ConnectionStringSettings)settingObject).ConnectionString == String.Empty)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString (connection string) was empty: {0}", connectionStringName));
                }
                connectionString = ((ConnectionStringSettings)settingObject).ConnectionString;

                if (((ConnectionStringSettings)settingObject).ProviderName == String.Empty)
                {
                    throw new ApplicationException(String.Format("Configuration connectionString (provider name) was empty: {0}", connectionStringName));
                }
                providerName = ((ConnectionStringSettings)settingObject).ProviderName;

                returnValue = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
            return returnValue;
        }
    }
}

