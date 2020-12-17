using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Xml;
using System.Text;
using System.Xml.Serialization;
//using Compat.Runtime.Serialization;try this port if nothing else works

namespace Ssepan.Utility
{

    public enum SerializationFormat
    {
        Xml,
        DataContract
    }

    /// <summary>
    /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// 
    /// Provides a method for performing a deep copy of an object.
    /// Binary Serialization is used to perform the copy.
    /// 
    /// Note: based on article and code above, but used my XML File I/O logic
    /// to replace the use of the IFormatter because it had problems with serializing 
    /// some objects that my File I/O had no problem converting. The new code just uses 
    /// XmlSerializer and MemoryStream.
    /// </summary>

    public static class ObjectHelper
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="TStruct">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source, SerializationFormat serializeAs = SerializationFormat.Xml)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            #region old
            //Original code; has problems serializing some objects (i.e. - EquatableBindingListOfT).
            //IFormatter formatter = new BinaryFormatter();
            //Stream stream = new MemoryStream();
            //using (stream)
            //{
            //    formatter.Serialize(stream, source);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return (TStruct)formatter.Deserialize(stream);
            //}
            #endregion old
            //TODO:try datacontract serializer instead, to handle complex type and avoid circular references.--SJS
            #region new
            switch (serializeAs)
            {
                case SerializationFormat.DataContract:
                    {
                        #region datacontract
                        //DataContract Serializer of type Settings
                        return CloneUsingDataContract<T>(source);
                        #endregion datacontract

                        break;
                    }
                case SerializationFormat.Xml:
                default:
                    {
                        #region xml
                        return CloneUsingXml<T>(source);
                        #endregion xml

                        break;
                    }
            }
            #endregion new
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// 
        /// <returns></returns>
        private static T CloneUsingDataContract<T>(T source)
        {
            DataContractSerializerSettings settings = default(DataContractSerializerSettings);
            settings = new DataContractSerializerSettings();

            settings.MaxItemsInObjectGraph = Int32.MaxValue;
            settings.IgnoreExtensionDataObject = false;
            settings.PreserveObjectReferences = true;

            DataContractSerializer ds =
                //new NetDataContractSerializer
                new DataContractSerializer
                (
                    typeof(T),
                    settings
                    //null, 
                    //Int32.MaxValue, 
                    //false, 
                    //true /* preserve object refs */, 
                    //null
                );
            //DataContractSerializer ds = new DataContractSerializer(typeof(T));
            Stream stream = new MemoryStream();
            using (stream)
            {
                ds.WriteObject(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)ds.ReadObject(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static T CloneUsingXml<T>(T source)
        {
            //XML Serializer of type Settings
            XmlSerializer xs = new XmlSerializer(typeof(T));
            Stream stream = new MemoryStream();
            using (stream)
            {
                xs.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)xs.Deserialize(stream);
            }
        }

        /// <summary>
        /// Cast method - thanks to type inference when calling methods it 
        /// is possible to cast object to type without knowing the type name
        /// See: http://tomasp.net/blog/cannot-return-anonymous-type-from-method.aspx
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Cast<T>(object obj, T type)
        {
            return (T)obj;
        }

        /// <summary>
        /// Return a list of property names using Reflection.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static List<String> GetPropertyNames<TEntity>()
        {
            List<String> returnValue  =
                (from pi in typeof(TEntity).GetProperties(/*BindingFlags.Public*/)
                 select pi.Name).ToList<String>();
            
            return returnValue;
        }

        /// <summary>
        /// Gets the value of a property discovered by name on an entity.
        /// Also returns the Type on a reference property
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tEntity">Entity of type TEntity, in which we are discovering a property</param>
        /// <param name="propertyName">Name of property to discover</param>
        /// <param name="propertyType">Type of discovered property</param>
        /// <returns>Value of discovered property</returns>
        public static Object GetValueFromPropertyByPropertyName<TEntity>(TEntity tEntity, String propertyName, ref Type propertyType)
        {
            // get propertyinfo of named property of entity (compact)
            PropertyInfo propertyInfo =
                (from pi in typeof(TEntity).GetProperties(/*BindingFlags.Public*/)
                 where pi.Name == propertyName
                 select pi).ToList<PropertyInfo>().SingleOrDefault();

            // get propertyinfo of named property of entity (expanded)
            ////PropertyInfo[] propertyInfoArray = typeof(TEntity).GetProperties(/*BindingFlags.Public*/);
            //List<PropertyInfo> propertyInfoList = (from pi in propertyInfoArray
            //                                       where pi.Name == propertyName
            //                                       select pi).ToList<PropertyInfo>();
            //PropertyInfo propertyInfo = 
            //    propertyInfoList.SingleOrDefault();

            //get property type
            propertyType = propertyInfo.PropertyType;

            //get value from propertyinfo and entity reference
            return propertyInfo.GetValue(tEntity, null);

            ////get value by gettign methodinfo and invoking with entity reference
            //MethodInfo methodInfo = propertyInfo.GetGetMethod();
            //return methodInfo.Invoke(t, null);
        }

        /// <summary>
        /// Gets the Type of a property discovered by name on an entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        //// <param name="tEntity">Entity of type TEntity, in which we are discovering a property</param>
        /// <param name="propertyName">Name of property to discover</param>
        /// <returns>Type of discovered property</returns>
        public static Type GetTypeFromPropertyByPropertyName<TEntity>(/*TEntity tEntity, */String propertyName)
        {
            // get propertyinfo of named property of entity (compact)
            PropertyInfo propertyInfo =
                (from pi in typeof(TEntity).GetProperties(/*BindingFlags.Public*/)
                 where pi.Name == propertyName
                 select pi).ToList<PropertyInfo>().SingleOrDefault();

            // get propertyinfo of named property of entity (expanded)
            //PropertyInfo[] propertyInfoArray = typeof(TEntity).GetProperties(/*BindingFlags.Public*/);
            //List<PropertyInfo> propertyInfoList = (from pi in propertyInfoArray
            //                                       where pi.Name == propertyName
            //                                       select pi).ToList<PropertyInfo>();
            //PropertyInfo propertyInfo =
            //    propertyInfoList.SingleOrDefault();

            //get property type
            Type propertyType = propertyInfo.PropertyType;

            //get value from propertyinfo and entity reference
            return propertyType;
        }

        /// <summary>
        /// By SamAgain (MSFT CSG) 
        /// on http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/740489bd-7906-42ed-a1d3-e20273fbcc9c/
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="extendedType"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            List<MethodInfo> extension_methods = new List<MethodInfo>();

            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsDefined(typeof(ExtensionAttribute), false))
                {
                    foreach (MethodInfo mi in t.GetMethods())
                    {
                        if (mi.IsDefined(typeof(ExtensionAttribute), false))
                        {
                            if (mi.GetParameters()[0].ParameterType == extendedType)
                                extension_methods.Add(mi);
                        }
                    }
                }
            }
            return extension_methods;
        }

        /// <summary>
        /// Read struct from stream.
        /// Use only with stream containing value types.
        /// Found in 
        ///  http://bchavez.bitarmory.com/archive/2012/08/27/modify-visual-studio-2012-dark-and-light-themes.aspx
        /// and 
        ///  http://stackoverflow.com/questions/4159184/c-read-structures-from-binary-file
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static TStruct ReadStruct<TStruct>
        (
            this Stream stream
        )
            where TStruct : struct
        {
            Int32 size = Marshal.SizeOf(typeof(TStruct));

            Byte[] buffer = new Byte[size];
            stream.Read(buffer, 0, size);
            GCHandle pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            TStruct structure =
                (TStruct)Marshal.PtrToStructure
                (
                    pinnedBuffer.AddrOfPinnedObject(),
                    typeof(TStruct)
                );
            pinnedBuffer.Free();
            return structure;
        }

        /// <summary>
        /// Based on http://www.codeproject.com/Articles/308536/How-to-copy-event-handlers-from-one-control-to-ano?msg=4822390#xx4822390xx
        /// by http://www.codeproject.com/script/Membership/View.aspx?mid=3837373
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="eventName"></param>
        public static void CopyEvents
        <
            TSource, 
            TDestination
        >
        (
            TSource source, 
            TDestination destination,
            String eventName = "events"
        )
            where TSource : class//, new()
            where TDestination : class//, new()
        {
            try
            {
                FieldInfo sourceFieldInfo = typeof(TSource).GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo destinationFieldInfo = typeof(TDestination).GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (sourceFieldInfo != null)
                {
                    Object eventHandlerList = sourceFieldInfo.GetValue(source);
                    //sourceFieldInfo.SetValue(destination, eventHandlerList);//Note:using source field info for dest caused error finding field name.
                    destinationFieldInfo.SetValue(destination, eventHandlerList);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
        }
    }
}


