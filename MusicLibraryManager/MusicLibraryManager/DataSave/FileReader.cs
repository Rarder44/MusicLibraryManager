using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager.DataSave
{
    static class FileReader
    {
        static public FileData ReadFile(String Path)
        {
            FileData fd= MySerializerWriter.Deserialize<FileData>(Path);
            if(fd!=null)
            {
                if(fd.o is Newtonsoft.Json.Linq.JObject)
                {
                    if (fd.Type == FileDataType.Option)
                        fd.o = Json.Deserialize<Option>(fd.o.ToString());
                    else if (fd.Type == FileDataType.MediaLibrary)
                        fd.o = Json.Deserialize<MyFileSystemPlus>(fd.o.ToString());
                    else if(fd.Type==FileDataType.Playlistlsocation)
                        fd.o = Json.Deserialize<Playlistlsocation>(fd.o.ToString());
                    else if (fd.Type == FileDataType.Playlist)
                        fd.o = Json.Deserialize<Playlist>(fd.o.ToString());
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
