using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DX12Editor.Serializers
{
    internal static class Serializer
    {
        private static XmlWriterSettings _settings;


        static Serializer()
        {
            _settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t"
            };
        }

        public static void ToFile<T>(T instance, string path)
        {
            try
            {
                using var fs = XmlWriter.Create(path, _settings);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, instance);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public static T FromFile<T>(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(fs);
                return instance;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return default(T);
            }
        }
    }

}
