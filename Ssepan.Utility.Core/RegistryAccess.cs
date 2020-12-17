using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using Ssepan.Utility;

namespace Ssepan.Utility
{
    /// <summary>
    /// Provides access to the Registry for maintaining persistent storage.
    /// </summary>
    public class RegistryAccess
    {
        private static String _SubKeyName = default(String);
        public static String SubKeyName
        {
            get { return _SubKeyName; }
            set { _SubKeyName = value; }
        }

        private static String _ValueName = default(String);
        public static String ValueName
        {
            get { return _ValueName; }
            set { _ValueName = value; }
        }

        private static RegistryKey _ClassesRoot;
        public static RegistryKey ClassesRoot
        {
            get
            {
                if (_ClassesRoot == null)
                {
                    _ClassesRoot = Registry.ClassesRoot.OpenSubKey(SubKeyName, true);
                    if (_ClassesRoot == null)
                    {
                        _ClassesRoot = Registry.ClassesRoot.CreateSubKey(SubKeyName);
                    }
                }
                return _ClassesRoot;
            }
        }

        private static RegistryKey _CurrentConfig;
        public static RegistryKey CurrentConfig
        {
            get
            {
                if (_CurrentConfig == null)
                {
                    _CurrentConfig = Registry.CurrentConfig.OpenSubKey(SubKeyName, true);
                    if (_CurrentConfig == null)
                    {
                        _CurrentConfig = Registry.CurrentConfig.CreateSubKey(SubKeyName);
                    }
                }
                return _CurrentConfig;
            }
        }
        
        private static RegistryKey _currentUser;
        public static RegistryKey CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = Registry.CurrentUser.OpenSubKey(SubKeyName, true);
                    if (_currentUser == null)
                    {
                        _currentUser = Registry.CurrentUser.CreateSubKey(SubKeyName);
                    }
                }
                return _currentUser;
            }
        }

        private static RegistryKey _LocalMachine;
        public static RegistryKey LocalMachine
        {
            get
            {
                if (_LocalMachine == null)
                {
                    _LocalMachine = Registry.LocalMachine.OpenSubKey(SubKeyName, true);
                    if (_LocalMachine == null)
                    {
                        _LocalMachine = Registry.LocalMachine.CreateSubKey(SubKeyName);
                    }
                }
                return _LocalMachine;
            }
        }

        //TODO:Users, DynData, PerformanceData

        public static String GetValue(RegistryKey hive)
        {
            return (String)hive.GetValue(ValueName, null);
        }

        public static void SetValue(RegistryKey hive, String value)
        {
            hive.SetValue(ValueName, value);
        }

        /// <summary>
        /// Check registry for file association; if missing and allowed, registers file type.
        /// </summary>
        /// <param name="executablePath"></param>
        /// <param name="fileExtension"></param>//FileTypeExtension
        /// <returns></returns>
        public static Boolean ValidateFileAssociation(String executablePath, String fileExtension)
        {
            Boolean returnValue = default(Boolean);

            try
            {
                if (executablePath == null)
                {
                    returnValue = IsFileAssociation(System.Windows.Forms.Application.ExecutablePath, fileExtension, false);
                }
                else
                {
                    returnValue = IsFileAssociation(executablePath, fileExtension, false);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);

                throw;
            }

            return returnValue;
        }

        /// <summary>
        /// Get or set file association in reistry.
        /// </summary>
        /// <param name="executablePath"></param>
        /// <param name="fileExtension"></param>
        /// <param name="allowRegisterFileAssociation"></param>
        /// <returns></returns>
        public static Boolean IsFileAssociation(String executablePath, String fileExtension, Boolean allowRegisterFileAssociation)
        {
            Boolean returnValue = default(Boolean);

            try
            {
                // Create a registry key object to represent the HKEY_CLASSES_ROOT registry section
                RegistryKey registryKeyHKCR = Registry.ClassesRoot;

                // Attempt to retrieve the registry key for the .<fileExtension> file type
                RegistryKey registryKeyFileType = registryKeyHKCR.OpenSubKey("." + fileExtension);

                // Was the file type found?
                returnValue = (registryKeyFileType != null);
                if (!returnValue)
                {
                    // No, so register it (if the current settings allow)
                    if (allowRegisterFileAssociation)
                    {
                        // No, so register it
                        RegistryKey registryKeyNew;

                        // Create the registry key
                        registryKeyNew = registryKeyHKCR.CreateSubKey("." + fileExtension);

                        // Set the unique file type name
                        registryKeyNew.SetValue("", fileExtension + ".settings");

                        // Create the file type information key
                        RegistryKey registryKeyInfo = registryKeyHKCR.CreateSubKey(fileExtension + ".settings");

                        // Set the default value to the file type description
                        registryKeyInfo.SetValue("", fileExtension + " File");

                        // Create the shell key to contain all verbs
                        RegistryKey registryKeyShell = registryKeyInfo.CreateSubKey("shell");

                        // Create a subkey for the "Open" verb
                        RegistryKey registryKeyOpen = registryKeyShell.CreateSubKey("Open");

                        // Set the menu name against the key
                        registryKeyOpen.SetValue("", "&Open Document");

                        // Create and set the command string
                        registryKeyNew = registryKeyOpen.CreateSubKey("command");
                        registryKeyNew.SetValue("", executablePath + @" %1");

                        // Attempt (again) to retrieve the registry key for the .<fileExtension> file type
                        registryKeyFileType = registryKeyHKCR.OpenSubKey("." + fileExtension);
                        returnValue = (registryKeyFileType == null);
                    }

                }
                returnValue = true;
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
