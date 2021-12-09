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
        static string application = @"Battle Cats Save Editor.exe";
        static void Main()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            File.Delete(@"Battle Cats save editor.exe");

            string lines = "";
            bool skip = false;
            try
            {
                string data = (string)MakeRequest(true, WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt"));
                lines = data.Trim('\n');
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

                MakeRequest(false, WebRequest.Create(link));
                
            }
            System.Diagnostics.Process.Start(@"Battle Cats Save Editor.exe");

        }
        static object MakeRequest(bool isString, WebRequest request)
        {
            request.Headers.Add("time-stamp", DateTime.Now.Ticks.ToString());
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                if (isString)
                {
                    string responseFromServer = reader.ReadToEnd();
                    return responseFromServer;
                }
                else
                {
                    using (Stream s = File.Create(application))
                    {
                        dataStream.CopyTo(s);
                    }
                    return dataStream;
                }
            }
        }

    }
}
