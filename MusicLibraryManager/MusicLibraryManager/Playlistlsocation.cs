using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    [JsonObject(MemberSerialization.OptIn)]
    class Playlistlsocation
    {
        [JsonProperty]
        public List<String> PathPlaylist = new List<string>();
    }
}
