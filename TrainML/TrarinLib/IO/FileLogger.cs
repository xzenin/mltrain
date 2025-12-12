using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetTrain.IO
{
    public static class FileLogger
    {
        public static Action<string> WriteThere = null;
        public static void WriteLine(string message) { 
        
            Console.WriteLine(message);
            Debug.WriteLine(message);
            if (WriteThere != null)
            {
                try
                {
                    WriteThere(message);
                }
                catch { }
            }
        }

        public static string WriteToFileJson(string filename, object data)
        {
            // Save object to file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filename, jsonString);
            return jsonString;
        }
        public static T ReadFromFileJson<T>(string filename)
        {
            // Read object back
            string readJson = File.ReadAllText(filename);
            if(string.IsNullOrEmpty(readJson)) return default(T);
            T obj = JsonSerializer.Deserialize<T>(readJson);
            return obj;
        }
        public static string WriteToFileXml(string filename, object data)
        {
            // Save object to file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filename, jsonString);
            return jsonString;
        }
        public static T ReadFromFileXml<T>(string filename)
        {
            // Read object back
            string readJson = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(readJson)) return default(T);
            T obj = JsonSerializer.Deserialize<T>(readJson);
            return obj;
        }
        public static T DeserializeFromString<T>(string value)
        {
            T outObject;
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(value);
            outObject = (T)deserializer.Deserialize(stringReader);
            stringReader.Close();
            return outObject;
        }
        public static string SerializeFromString<T>(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, value);
            stringWriter.Close();
            return stringWriter.ToString();
        }
    }
}
