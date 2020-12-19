using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    class Updater
    {
        static void Main(string[] args)
        {
            string folderName = @"newversion.txt";
            File.Delete(@"Battle Cats save editor.exe");
            string application = @"Battle Cats Save Editor.exe";

            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt", folderName);

            string[] lines = System.IO.File.ReadAllLines(@"newversion.txt");
           
            string link = "https://github.com/fieryhenry/Battle-Cats-Save-File-Editor/releases/download/" + lines[0] + "/Battle.Cats.save.editor.exe";
            Console.WriteLine(link);
            webClient.DownloadFile(link, application);
            System.Diagnostics.Process.Start(@"Battle Cats Save Editor.exe");

        }

    }
}
