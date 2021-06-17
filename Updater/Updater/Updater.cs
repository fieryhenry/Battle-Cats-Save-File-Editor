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
        static void Main()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            File.Delete(@"Battle Cats save editor.exe");
            string application = @"Battle Cats Save Editor.exe";

            WebClient webClient = new WebClient();

            string lines = "";
            bool skip = false;
            try
            {
                lines = webClient.DownloadString("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt").Trim('\n');
            }
            catch (WebException)
            {
                Console.WriteLine("No internet connection to update\n");
                skip = true;
            }

            if (!skip)
            {
                string link = "https://github.com/fieryhenry/Battle-Cats-Save-File-Editor/releases/download/" + lines + "/Battle.Cats.save.editor.exe";
                Console.WriteLine(link + "\nUpdating program to newest version please wait");
                WebClient webClient2 = new WebClient();
                webClient2.DownloadFile(link, application);
            }

            System.Diagnostics.Process.Start(@"Battle Cats Save Editor.exe");

        }

    }
}
