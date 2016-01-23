using ExtendCSharp.Interfaces;
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
        public String Titolo, Artista, Album, NumeroTraccia, Genere;
        public String MD5;

        public MyAddittionalData()
        {
            Selezionato = false;
            Titolo = "";
            Artista = "";
            Album = "";
            NumeroTraccia = "";
            Genere = "";
        }

        public object Clone()
        {
            MyAddittionalData m = new MyAddittionalData();
            m.Selezionato = Selezionato;
            m.Titolo = Titolo;
            m.Artista = Artista;
            m.Album = Album;
            m.NumeroTraccia = NumeroTraccia;
            m.Genere = Genere;
            return m;
        }
    }
}
