using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class NewInquiryCode
    {
        public static void NewIQ(string path)
        {

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream2.Length;
            byte[] allData = new byte[length];
            stream2.Read(allData, 0, length);

            Console.WriteLine("What inquiry code do you want - this code must be set to an account code that actually lets you play without the save is used elsewhere bug");
            string iq = Console.ReadLine();
            byte[] bytes = Encoding.ASCII.GetBytes(iq);
            bool found = false;

            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2D && allData[i + 1] == 0x0 && allData[i + 2] == 0x0 && allData[i + 3] == 0x0 && allData[i + 4] == 0x2E)
                {
                    for (int j = 1900; j < 2108; j++)
                    {
                        if (allData[i - j] == 09)
                        {
                            stream2.Position = i - j + 4;
                            stream2.Write(bytes, 0, bytes.Length);
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                Editor.Error();
            }
            Console.WriteLine("Success\nYour new account code is now: " + iq + " This should remove that \"save is being used elsewhere\" bug and if your account is banned, this should get you unbanned");
        }
    }
}
