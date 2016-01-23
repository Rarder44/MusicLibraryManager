using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Playlist
    {
        [JsonProperty]
        String Name;
        [JsonProperty]
        public MyFileSystemPlus FileSystem;

        [JsonIgnore]
        public String Path;

        public Playlist()
        {

        }
        public Playlist(String Path)
        {
            this.Path = Path;
        }
        public Playlist(String Path,String Nome)
        {
            Name = Nome;
            this.Path = Path;
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
