using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager.DataSave
{
    static class FileService
    {
        static public FileData ReadFile(String Path)
        {
            FileData fd= MySerializerWriter.Deserialize<FileData>(Path);
            if(fd!=null)
            {
                if(fd.o is Newtonsoft.Json.Linq.JObject)
                {

                    if (fd.Type == FileDataType.Option)
                        fd.o = (fd.o as Newtonsoft.Json.Linq.JObject).ToObject<Option>();
                    else if (fd.Type == FileDataType.MediaLibrary)
                        fd.o = (fd.o as Newtonsoft.Json.Linq.JObject).ToObject<MyFileSystemPlus>();
                    else if (fd.Type == FileDataType.Playlistlsocation)
                        fd.o = (fd.o as Newtonsoft.Json.Linq.JObject).ToObject<Playlistlsocation>();
                    else if (fd.Type == FileDataType.Playlist)
                        fd.o = (fd.o as Newtonsoft.Json.Linq.JObject).ToObject<Playlist>();
                    else if (fd.Type == FileDataType.IndexFile)
                        fd.o = (fd.o as Newtonsoft.Json.Linq.JObject).ToObject<IndexFile>();
                        
                }
            }
            return fd;
        }
        static public void WriteFile(String Path,object o,FileDataType Type)
        {
            MySerializerWriter.Serialize(new FileData(Type, o), Path);
        }

    }
}
