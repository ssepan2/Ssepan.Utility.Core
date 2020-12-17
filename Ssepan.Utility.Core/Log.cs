//#define USEEVENTLOG

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Ssepan.Utility
{
    public static class Log
    {
        private const String LOG_FILENAME = "Application.log";

        private static String _LogPath = System.Windows.Forms.Application.CommonAppDataPath;
        public static String LogPath
        {
            get { return _LogPath; }
            set { _LogPath = value; }
        }

        /// <summary>
        /// Write log entry.
        /// General-purpose version that can be used for any situation.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="logFilename"></param>
        /// <param name="append"></param>
        public static void Write
        (
            String message,
            EventLogEntryType type = EventLogEntryType.Error,
            String logFilename = "",
            Boolean append = true
        )
        {
#if USEEVENTLOG
            EventLog.WriteEntry
            (
                message,
                type
            );
#else
            String LogFilename = default(String);
            if (String.IsNullOrEmpty(logFilename))
            {
                LogFilename = @Path.Combine(LogPath, LOG_FILENAME);
            }
            else
            {
                LogFilename = @Path.Combine(LogPath, logFilename);
            }

            using (TextWriter textWriter = new StreamWriter(LogFilename, append))
            {
                textWriter.WriteLine(String.Format("{0}\t{1}\t{2}", DateTime.Now, type.ToString(), message));
            }
#endif
        }

        /// <summary>
        /// Writes a message to the Application event log.
        /// Special-purpose version that is designed for use with exceptions.
        /// exception: an exception that we want to write to log
        /// ex: an exception that may occur when we try to write exception to log
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="currentMethod"></param>
        /// <param name="entryType"></param>
        /// <param name="logFilename"></param>
        /// <param name="append"></param>
        public static void Write
        (
            Exception exception,
            MethodBase currentMethod,
            EventLogEntryType entryType,
            String logFilename = "",
            Boolean append = true
        )
        {
            try
            {
                Log.Write
                (
                    Log.FormatEntry(Log.Build(exception, currentMethod), currentMethod.DeclaringType.FullName, currentMethod.Name),
                    entryType,
                    logFilename,
                    append
                );
            }
            catch (Exception ex)
            {
                //this will appear in the UI
                throw new Exception(String.Format("Unable to write to log. \n Reason: {0} \n Message: {1}", ex.Message, exception.Message), exception);
            }
        }

        /// <summary>
        /// Formats entry header, using explicitly passed values.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static String FormatEntry(String message, String className, String methodName)
        {
            String returnValue = String.Empty;

            try
            {
                //FormatEntry entry header
                returnValue = String.Format("Location: Logged\nClass: {0}\nMember: {1}\n\n", className, methodName);

                //Append message
                returnValue += message;

                //truncate to 1st 32K characters
                returnValue = (returnValue.Substring(0, Math.Min(returnValue.Length, 32000)));
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to format entry header.", ex);
            }
            
            return returnValue;
        }

        /// <summary>
        /// Formats entry for layout of nested messages, using explicitly passed values.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static String FormatEntryDetail(String message)
        {
            String returnValue = String.Empty;

            try
            {
                //Format entry detail
                returnValue = String.Format("{0}\nMember: {1}\n\n", message);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to format entry detail.", ex);
            }
            
            return returnValue;
        }

        /// <summary>
        /// Formats message with detail information describing the error, using explicitly passed values.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stackFrameInfo"></param>
        /// <param name="innerMessage"></param>
        /// <returns></returns>
        public static String FormatMessage(String message, String stackFrameInfo, String innerMessage)
        {
            String returnValue = String.Empty;

            try
            {
                //Format entry detail
                returnValue = String.Format("[{0}Message: {1}\n\n{2}\n]\n", stackFrameInfo, message, innerMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to format message.", ex);
            }
            
            return returnValue;
        }
 
        /// <summary>
        /// Builds message from passed exception and any inner-exceptions.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="currentMethod"></param>
        /// <returns></returns>
        public static String Build(Exception exception, MethodBase currentMethod)
        {
            String returnValue = String.Empty;
            String exceptionMessage = String.Empty;
            String innerExceptionMessage = String.Empty;

            try
            {
                if (exception != null)
                {
                    if (exception.InnerException != null)
                    { 
                        //Build inner message.
                        innerExceptionMessage = Build(exception.InnerException, currentMethod);
                    }

                    //Build outer message
                    exceptionMessage = FormatMessage(exception.Message, Log.GetStackFrameInfo(exception), innerExceptionMessage);
                }

                returnValue = exceptionMessage;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to build message.", ex);
            }
            
            return returnValue;
        }

        /// <summary>
        /// Gets line number, method, and class of error from stack trace.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static String GetStackFrameInfo(Exception exception)
        {
            String returnValue = String.Empty;
            StackTrace stackTrace = default(StackTrace);
            String lineNumber = String.Empty;
            String methodName = String.Empty;
            String className = String.Empty;
            StackFrame stackFrame = default(StackFrame);
            StackFrame[] stackFrames;
            String location = String.Empty;

            try
            {
                if (exception.StackTrace == null)
                {
                    returnValue += String.Format("\n Location: {0} \n Class: {1} \n Member: {2} \n Line: {3} \n ", "n/a", "n/a", "n/a", "n/a");
                }
                else
                {
                    stackTrace = new StackTrace(exception, true);

                    stackFrames = stackTrace.GetFrames();
                    for (Int32 i = stackFrames.GetLowerBound(0); i <= stackFrames.GetUpperBound(0); i++)
                    {
                        switch (i)
                        {
                            case 0:
                                location = "Thrown";
                                break;
                            case 1:
                                location = "Caught";
                                break;
                            default:
                                location = "n/a";
                                break;
                        }

                        stackFrame = stackFrames[i];

                        lineNumber = stackFrame.GetFileLineNumber().ToString();
                        methodName = stackFrame.GetMethod().Name;
                        if (stackFrame.GetMethod().DeclaringType == null)
                        {
                            className = "n/a";
                        }
                        else
                        {
                            className = stackFrame.GetMethod().DeclaringType.FullName;
                        }

                        returnValue += String.Format("\nLocation: {0}\nClass: {1}\nMember: {2}\nLine: {3}\n", location, className, methodName, lineNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get line number.", ex);
            }

            return returnValue;
        }
   }
}
