using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace MITD.Services
{
    public static class ByteArraySerializer
    {
        public static T Clone<T>(T graph)
        {
            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(stream, graph);
            stream.Position = 0;
            return (T)serializer.ReadObject(stream);
        }

        public static string ToString<T>(T graph)
        {

            var stream = new MemoryStream();

            var serializer = new DataContractSerializer(typeof(T));

            serializer.WriteObject(stream, graph);

            stream.Position = 0;
            var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();

        }

        public static T ToObject<T>(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            MemoryStream stream = new MemoryStream(bytes);

            stream.Position = 0;

            var serializer = new DataContractSerializer(typeof(T));

            return (T)serializer.ReadObject(stream);

        }

    }


}