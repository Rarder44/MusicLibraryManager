using ExtendCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    public class MyFileSystemPlus : FileSystemPlus<MyAddittionalDataMyFileSystem>
    {
        public MyFileSystemPlus(String RootPath, FileSystemPlusLoadOption option = null) : base(RootPath,option)
        {

        }
        public void RimuoviOggettiSelezionati()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalDataMyFileSystem> n) => { return n.Type == FileSystemNodePlusType.File && n.AddittionalData.Selezionato;  }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
        }
        public void RimuoviOggettiNonSelezionati()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalDataMyFileSystem> n) => { return n.Type == FileSystemNodePlusType.File && !n.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
        }
        public void CancellaCartelleVuote()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalDataMyFileSystem> n) => { return n.Type == FileSystemNodePlusType.Directory && n.ChildCount==0; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
        }

    }
}
