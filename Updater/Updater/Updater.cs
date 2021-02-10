using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Updater
{
    class Updater
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            string folderName = @"newversion.txt";
            File.Delete(@"Battle Cats save editor.exe");
            string application = @"Battle Cats Save Editor.exe";

            HttpRequest webClient = new HttpRequest(@"newversion.txt", "https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt", "Updater");

            string[] lines = System.IO.File.ReadAllLines(@"newversion.txt");
           
            string link = "https://github.com/fieryhenry/Battle-Cats-Save-File-Editor/releases/download/" + lines[0] + "/Battle.Cats.save.editor.exe";
            Console.WriteLine(link + "\nUpdating program to newest version please wait");
            HttpRequest webClient2 = new HttpRequest(application, link, "Updater");

            System.Diagnostics.Process.Start(@"Battle Cats Save Editor.exe");

        }

    }
}
