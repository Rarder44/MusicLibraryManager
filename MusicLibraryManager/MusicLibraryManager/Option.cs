using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendCSharp;


namespace MusicLibraryManager
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Option
    {
        ChangedVar cv = ChangedVar.nul;
        public ChangedVar ChangedVar { get { return cv; } }

        [JsonProperty]
        String _PathFFmpeg =null;
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
        String _PathMetaflac = "metaflac.exe";
        public String PathMetaflac
        {
            get { return _PathMetaflac; }
            set
            {
                if (_PathMetaflac == value)
                    return;
                _PathMetaflac = value;
                cv |= ChangedVar.PathMetaflac;
            }
        }


        [JsonProperty]
        String _PathMedia = null;
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

        [JsonProperty]
        ListPlus<String> _Extensions  = null;
        public ListPlus<String> Extensions
        {
            get { return _Extensions; }
            set
            {
                if (_Extensions == value)
                    return;
                _Extensions = value;
                cv |= ChangedVar.Extensions;
            }
        }



        [JsonProperty]
        bool _IndexFileFormCloseFormAutomatically = false;
        public bool IndexFileFormCloseFormAutomatically
        {
            get { return _IndexFileFormCloseFormAutomatically; }
            set
            {
                if (_IndexFileFormCloseFormAutomatically == value)
                    return;
                _IndexFileFormCloseFormAutomatically = value;
                cv |= ChangedVar.IndexFileFormCloseFormAutomatically;
            }
        }




        public FileSystemPlusLoadOption LoadMediaOption()
        {
            FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            if (Extensions != null)
                foreach (string s in Extensions)
                    lo.RestrictExtension.AddToLower(s);

            return lo;
        }
        public  Option()
        {
            _Extensions = new ListPlus<String>();
            _Extensions.OnAdd += (object sender, EventArgs e) => { cv |= ChangedVar.Extensions; };
            _Extensions.OnRemove += (object sender, EventArgs e) => { cv |= ChangedVar.Extensions; };
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
        Extensions=4,
        IndexFileFormCloseFormAutomatically = 8,
        PathMetaflac=16

    }
}
