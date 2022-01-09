using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class EncryptPack
    {
        public static void EncryptData(string key)
        {
            Console.WriteLine("Enter name of .pack file name to be outputed, e.g DataLocal, ImapServer (don't include .pack) + capatalisation must be correct");
            string name = Console.ReadLine();

            FolderBrowserDialog fd = new()
            {
                SelectedPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\game_files",
                Description = "Select a folder of game files to be encrypted into .pack and .list files",
                ShowNewFolderButton = false
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select a folder of game content!");
                Editor.Options();
            }
            string path = fd.SelectedPath;
            string[] files = Directory.GetFiles(path);
            string listFile = files.Length + "\n";
            Console.WriteLine("Making .list file, this may take a minute depending on how many files are used...");
            int previous = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);

                FileInfo fi = new (files[i]);
                int FileLength = (int)fi.Length;

                listFile += $"{fileName},{previous},{FileLength}\n";
                previous += FileLength;
            }
            Console.WriteLine("Done");
            byte[] list_bytes = FileHandler.AddExtraBytes("", false, Encoding.ASCII.GetBytes(listFile));
            if (!Directory.Exists("CompFiles/"))
            {
                Directory.CreateDirectory("CompFiles/");
            }
            Console.WriteLine("List file successfully created");
            File.WriteAllBytes(@"CompFiles/" + name + ".list", list_bytes);

            using var stream = new FileStream(@"CompFiles/" + name + ".list", FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] IV = new byte[16];
            byte[] Key = Encoding.ASCII.GetBytes(key);


            string result = name + ".list";

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Padding = PaddingMode.None;
            aesAlg.Mode = CipherMode.ECB;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new(allData);
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Read);

            byte[] bytess = new byte[length];

            csEncrypt.Read(bytess, 0, length);
            stream.Close();

            File.WriteAllBytes(@"CompFiles/" + result, bytess);
            byte[] PackBytes = new byte[previous];
            int preIndex = 0;
            Console.WriteLine("Are you using jp 10.8 and up?(yes/no)");
            string ver = Console.ReadLine();
            Console.WriteLine("Making .pack file, this may take a minute depending on how many files are used...");
            for (int i = 0; i < files.Length; i++)
            {
                byte[] bytef = File.ReadAllBytes(files[i]);
                if (name.Contains("Server"))
                {
                    byte[] IV2 = new byte[16];
                    byte[] Key2 = Encoding.ASCII.GetBytes("89a0f99078419c28");

                    using Aes aesAlg2 = Aes.Create();
                    aesAlg2.Key = Key2;
                    aesAlg2.IV = IV2;
                    aesAlg2.Padding = PaddingMode.None;
                    aesAlg2.Mode = CipherMode.ECB;

                    bytef = EncryptAES(files[i], aesAlg2);

                    //Console.WriteLine($"Decrypted: {Path.GetFileName(files[i])} {i}/{files.Length}");
                }
                else if (name.Contains("Local"))
                {
                    if (!name.Contains("ImageData"))
                    {
                        if (ver.ToLower() == "yes")
                        {
                            Key = StringToByteArray("d754868de89d717fa9e7b06da45ae9e3");
                            IV = StringToByteArray("40b2131a9f388ad4e5002a98118f6128");
                        }
                        else
                        {
                            Key = StringToByteArray("0ad39e4aeaf55aa717feb1825edef521");
                            IV = StringToByteArray("d1d7e708091941d90cdf8aa5f30bb0c2");
                        }

                        using Aes aesAlg2 = Aes.Create();
                        aesAlg2.Key = Key;
                        aesAlg2.IV = IV;
                        aesAlg2.Padding = PaddingMode.None;
                        aesAlg2.Mode = CipherMode.CBC;

                        bytef = EncryptAES(files[i], aesAlg2);
                    }

                    //Console.WriteLine($"Decrypted: {Path.GetFileName(files[i])} {i}/{files.Length}");
                }
                bytef.CopyTo(PackBytes, preIndex);
                preIndex += bytef.Length;
            }
            File.WriteAllBytes(@"CompFiles/" + name + ".pack", PackBytes);
            Console.WriteLine("Done\nThe .list and .pack file can be found in " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/CompFiles/");
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }
        public static byte[] EncryptAES(string path, Aes aes)
        {
            byte[] bytef = File.ReadAllBytes(path);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream ms = new(bytef);
            using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Read);
            cs.Read(bytef, 0, bytef.Length);

            return bytef;
        }

    }
}
