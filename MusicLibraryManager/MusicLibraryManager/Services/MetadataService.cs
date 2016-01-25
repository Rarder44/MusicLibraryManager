using ExtendCSharp;
using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ExtendCSharp.Extension;

namespace MusicLibraryManager.Services
{
    public static class MetadataService
    {
        public static void IncorporaMetadata(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp, EndIncorporaMetadata OnEndIncorporaMetadata = null, IncorporaMetadataNodeProcessed OnIncorporaMetadataNodeProcessed = null, MD5BlockTransformEventHandler OnProgressChangedSingleMD5 = null, bool Async=true)
        {

            Thread t = new Thread(() =>
              {
                  IncorporaMetadataRicorsivo(nodo, mfsp, OnIncorporaMetadataNodeProcessed, OnProgressChangedSingleMD5);
                  if (OnEndIncorporaMetadata != null)
                      OnEndIncorporaMetadata();
              });
            t.Start();

            if (!Async)
                t.Join();            
        }
        private static void IncorporaMetadataRicorsivo(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp, IncorporaMetadataNodeProcessed OnIncorporaMetadataNodeProcessed = null,MD5BlockTransformEventHandler OnProgressChangedSingleMD5=null,bool Async=true)
        {
            foreach (FileSystemNodePlus<MyAddittionalData> n in nodo.GetAllNode())
            {
                if (n.Type == FileSystemNodePlusType.Directory)
                    IncorporaMetadataRicorsivo(n, mfsp);
                else if (n.Type == FileSystemNodePlusType.File)
                    IncorporaMetadata(n, mfsp, OnIncorporaMetadataNodeProcessed, OnProgressChangedSingleMD5, Async); 
            }
        }


        public static void IncorporaMetadata(FileSystemNodePlus<MyAddittionalData> nodo,MyFileSystemPlus mfsp, IncorporaMetadataNodeProcessed OnIncorporaMetadataNodeProcessed = null, MD5BlockTransformEventHandler OnProgressChangedSingleMD5 = null, bool Async = true)
        {
            if (nodo.Type == FileSystemNodePlusType.File)
            {
                String p = mfsp.GetFullPath(nodo);
                if (SystemService.FileExist(p))
                {
                    if (nodo.AddittionalData == null)
                        nodo.AddittionalData = new MyAddittionalData();

                    if (nodo.AddittionalData.Metadata == null)
                        nodo.AddittionalData.Metadata = new FFmpegMetadata();

                    nodo.AddittionalData.Metadata = FFmpeg.GetMetadata(p);

                    SystemService.GetMD5(p, OnProgressChangedSingleMD5, (byte[] Hash) =>
                    {
                        if(Hash != null)
                            nodo.AddittionalData.MD5 = Hash.ToHex(true);
                    }, Async);

                    if(OnIncorporaMetadataNodeProcessed!=null)
                        OnIncorporaMetadataNodeProcessed(nodo, IncorporaMetadataError.nul);
                }
                else
                    if (OnIncorporaMetadataNodeProcessed != null)
                        OnIncorporaMetadataNodeProcessed(nodo, IncorporaMetadataError.FileNonTrovato);
            }



        }

    }

    public delegate void EndIncorporaMetadata();
    public delegate void IncorporaMetadataNodeProcessed(FileSystemNodePlus<MyAddittionalData> nodo, IncorporaMetadataError Err);

    public enum IncorporaMetadataError
    {
        nul,
        FileNonTrovato
    }
}
