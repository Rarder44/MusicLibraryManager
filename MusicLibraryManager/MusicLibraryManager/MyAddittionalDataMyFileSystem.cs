using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    public class MyAddittionalDataMyFileSystem
    {
        public bool Selezionato;
        public String Titolo, Artista, Album, NumeroTraccia, Genere;
        public String MD5;

        public MyAddittionalDataMyFileSystem()
        {
            Selezionato = false;
            Titolo = "";
            Artista = "";
            Album = "";
            NumeroTraccia = "";
            Genere = "";
        }
    }
}
