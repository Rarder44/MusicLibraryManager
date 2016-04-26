using ExtendCSharp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager.DataSave
{
    [JsonObject(MemberSerialization.OptIn)]
    class FileData
    {
        public FileData(FileDataType Type, object o )
        {
            this.Type = Type;
            this.o = o;
        }
        [JsonProperty]
        public FileDataType Type = FileDataType.NotSet;

        [JsonProperty]
        public object o;
       
    }

    public enum FileDataType
    {
        MediaLibrary,
        Option,
        Playlistlsocation,
        Playlist,
        IndexFile,
        NotSet
    }
}
