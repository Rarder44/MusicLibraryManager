using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager.DataSave
{
    static class MySerializerWriter
    {
        //TODO: implementare una serializzazione piu efficace ( binary )
        static public void Serialize(Object o, String Path)
        {
            File.WriteAllText(Path, Json.Serialize(o));
        }
        static public  T Deserialize<T>(String Path)
        {
            if (File.Exists(Path))
                return Json.Deserialize<T>(File.ReadAllText(Path));

            return default(T);          
        }
    }
}
