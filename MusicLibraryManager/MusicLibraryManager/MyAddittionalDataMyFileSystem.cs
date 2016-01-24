using ExtendCSharp.Interfaces;
using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    public class MyAddittionalData:ICloneablePlus
    {
        public bool Selezionato;
        public FFmpegMetadata Metadata;
        public String MD5;

        public MyAddittionalData()
        {
            Selezionato = false;
            Metadata = new FFmpegMetadata();
        }

        public object Clone()
        {
            MyAddittionalData m = new MyAddittionalData();
            m.Selezionato = Selezionato;
            m.Metadata = Metadata.CloneClass();
            return m;
        }
        public MyAddittionalData CloneClass()
        {
            return (MyAddittionalData)(this as ICloneablePlus).Clone();
        }
    }
}
