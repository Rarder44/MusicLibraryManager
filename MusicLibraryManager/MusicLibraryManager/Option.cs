using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Option
    {
        ChangedVar cv = ChangedVar.nul;


        [JsonProperty]
        public String _PathFFmpeg =null;
        public String PathFFmpeg
        {
            get { return _PathFFmpeg; }
            set
            {
                if (_PathFFmpeg == value)
                    return;
                _PathFFmpeg = value;
                 cv |= ChangedVar.PathFFmpeg;
            }
        }


        [JsonProperty]
        public String _PathMedia = null;
        public String PathMedia
        {
            get { return _PathMedia; }
            set
            {
                if (_PathMedia == value)
                    return;
                _PathMedia = value;
                cv |= ChangedVar.PathMedia;
            }
        }

        public void SomethingChenged()
        {
            if (OnSomethingChenged != null)
                OnSomethingChenged(cv);
            cv = ChangedVar.nul;
        }

        public delegate void SomethingChanged(ChangedVar VarChanged);
        public event SomethingChanged OnSomethingChenged;


    }


    public enum ChangedVar
    {
        nul=0,
        PathFFmpeg=1,
        PathMedia=2,

    }
}
