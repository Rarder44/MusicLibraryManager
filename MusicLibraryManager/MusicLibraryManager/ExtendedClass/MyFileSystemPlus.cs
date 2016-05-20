using ExtendCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    public class MyFileSystemPlus : FileSystemPlus<MyAddittionalData>
    {

        public MyFileSystemPlus() : base()
        {

        }
        public MyFileSystemPlus(String RootPath, FileSystemPlusLoadOption option = null) : base(RootPath,option)
        {

        }
        public void RimuoviFileSelezionati()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.File && n.AddittionalData.Selezionato;  }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
        }
        public void RimuoviCertelleSelezionate()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.Directory && n.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
        }
        


        public void RimuoviFileNonSelezionati()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.File && !n.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
        }
        public void RimuoviCertelleNonSelezionate()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.Directory && !n.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
        }

        public void RimuoviCartelleVuote()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.Directory && n.ChildCount==0; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
        }
        public void RimuoviCertelleVuoteSelezionate()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.Directory && n.ChildCount == 0 && n.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
        }
        public void RimuoviCertelleVuoteNonSelezionate()
        {
            Root.Remove((FileSystemNodePlus<MyAddittionalData> n) => { return n.Type == FileSystemNodePlusType.Directory && n.ChildCount == 0 && !n.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
        }




        public FileSystemNodePlus<MyAddittionalData> FindFirst(Func<FileSystemNodePlus<MyAddittionalData>,bool> Condizione)
        {
            if (Condizione(Root))
                return Root;

            IEnumerable<FileSystemNodePlus<MyAddittionalData>> t= Root.Flatten();
            if (t.Count() == 0)
                return null;

            return t.Where(x => Condizione(x)).First();
        }
        public FileSystemNodePlus<MyAddittionalData>[] FindAll(Func<FileSystemNodePlus<MyAddittionalData>, bool> Condizione)
        {
            return Root.Flatten().Where(x => Condizione(x)).ToArray();
        }
        public MyFileSystemPlus FindPreserveTree(Func<FileSystemNodePlus<MyAddittionalData>, bool> Condizione, FileSystemNodePlusControlType QuandoControllare)
        {
           MyFileSystemPlus t = this.Clone();
           t.Root.Remove((x) => { return !Condizione(x); }, FileSystemNodePlusLevelType.AllNode, QuandoControllare);
           return t;
        }




        public void DeselectAll()
        {
            Root.Flatten().ToList().ForEach(x => { x.AddittionalData.Selezionato = false; });
        }


        public MyFileSystemPlus Clone()
        {
            MyFileSystemPlus t=new MyFileSystemPlus();
            t._Root = base._Root.Clone();
            t._RootRealPath = base._RootRealPath;
            return t;
        }

    }
}
