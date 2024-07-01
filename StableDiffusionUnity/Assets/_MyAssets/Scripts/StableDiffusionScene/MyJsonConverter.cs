using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

public static class MyJsonConverter
{
    /// <summary>
    /// Deserialize from Json string
    /// </summary>
    public static T Deserialize<T>(string body)
    {
        using (var stream = new MemoryStream())
        using (var writer = new StreamWriter(stream))
        {
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
        }
    }

    /// <summary>
    /// Serialize an object to Json
    /// </summary>
    public static string Serialize<T>(T item)
    {
        using(MemoryStream ms = new MemoryStream())
        {
            new DataContractJsonSerializer(typeof(T)).WriteObject(ms, item);
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}
