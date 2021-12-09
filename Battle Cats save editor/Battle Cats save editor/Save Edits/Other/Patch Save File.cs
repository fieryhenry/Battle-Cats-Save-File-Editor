using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class PatchSaveFile
    {
        public static string patchSaveFile(string choice, string path)
        {
            string name = Path.GetFileName(path);
            if (name.EndsWith(".pack") || name.EndsWith(".list") || name.EndsWith(".so") || name.EndsWith(".csv"))
            {
                return "";
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] toBeUsed = new byte[allData.Length - 32];
            for (int i = 0; i < allData.Length - 32; i++)
                toBeUsed[i] = allData[i];
            byte[] bytes = Encoding.ASCII.GetBytes("battlecats");
            if (choice != "jp")
            {
                bytes = Encoding.ASCII.GetBytes("battlecats" + choice);
            }
            int test = 32 - bytes.Length;

            byte[] Usable = new byte[allData.Length - test];
            bytes.CopyTo(Usable, 0);
            toBeUsed.CopyTo(Usable, bytes.Length);


            var md5 = MD5.Create();

            byte[] Data = new byte[16];
            Data = md5.ComputeHash(Usable);

            string hex = Editor.ByteArrayToString(Data);
            Console.WriteLine("Data patched");

            string EncyptedHex = Editor.ByteArrayToString(Data);

            hex = hex.ToLower();

            byte[] stuffs = Encoding.ASCII.GetBytes(hex);

            stream.Position = allData.Length - 32;
            stream.Write(stuffs, 0, stuffs.Length);
            return choice;
        }

    }
}
