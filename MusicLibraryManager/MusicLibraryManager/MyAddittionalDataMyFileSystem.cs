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
        public long Size;

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
            m.MD5 = MD5;
            m.Size = Size;
            return m;
        }
        public MyAddittionalData CloneClass()
        {
            return (MyAddittionalData)(this as ICloneablePlus).Clone();
        }
    }
}
