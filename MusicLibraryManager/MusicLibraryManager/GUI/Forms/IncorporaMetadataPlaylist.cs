using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;


namespace MusicLibraryManager.GUI.Forms
{


    class IncorporaMetadataPlaylist : IncorporaMetadata 
    {
        Playlist pp;

        public IncorporaMetadataPlaylist() : base()
        {
            
        }
        public IncorporaMetadataPlaylist(Playlist pp) : base(pp.FileSystem,pp.FileSystem.Root)
        {
            this.pp = pp;
            base.OnIncorporaMetadataFormEnd += (MyFileSystemPlus mfsp, FileSystemNodePlus<MyAddittionalData> AlternativeNode, IncorporaMetadataFormResult Result) =>
            {
                if (OnIncorporaMetadataPlaylistFormEnd != null)
                    OnIncorporaMetadataPlaylistFormEnd(pp, Result);
            };
        }

       

        public delegate void IncorporaMetadataPlaylistFormEnd(Playlist p, IncorporaMetadataFormResult Result);
        public event IncorporaMetadataPlaylistFormEnd OnIncorporaMetadataPlaylistFormEnd;
    }
    
}
