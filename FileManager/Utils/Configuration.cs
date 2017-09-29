using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Utils
{
    public static class Configuration
    {
        public const string name = "libFileManager.dll";
        public const int startX = 10, startY = 40, maxPerLine = 10,startXMiddle = 210, startYMiddle = 70;
        public const int file = 0, directory = 1, raccourci = 2;
        public static Dictionary<int, string> types = new Dictionary<int, string>()
        {
            {0,"Volume" },
            {1,"Dossier" },
            {4,"Fichier" }

        };
    }
}
