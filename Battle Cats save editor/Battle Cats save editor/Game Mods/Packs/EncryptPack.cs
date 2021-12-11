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
            string listFile = "";
            string[] files = Directory.GetFiles(path);
            listFile += files.Length + "\n";
            int previous = 0;
            Console.WriteLine("Making .list file, this may take a minute depending on how many files are used...");
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                int FileLength = File.ReadAllBytes(files[i]).Length;
                listFile += fileName;
                listFile += "," + previous + "," + FileLength + "\n";
                previous += FileLength;
                //ColouredText($"&Created list line: &{i}& for &{fileName}& - &{i}&/&{files.Length}&\n");

            }
            Console.WriteLine("Done");
            List<byte> ls = Encoding.ASCII.GetBytes(listFile).ToList();
            int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
            rem *= 16;
            rem -= ls.Count;
            for (int i = 0; i < rem && rem != 16; i++)
            {
                ls.Add((byte)rem);
            }
            if (!Directory.Exists("CompFiles/"))
            {
                Directory.CreateDirectory("CompFiles/");
            }
            Console.WriteLine("List file successfully created");
            File.WriteAllBytes(@"CompFiles/" + name + ".list", ls.ToArray());

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
                int FileLen = File.ReadAllBytes(files[i]).Length;
                if (name.Contains("Server"))
                {
                    byte[] IV2 = new byte[16];
                    byte[] Key2 = Encoding.ASCII.GetBytes("89a0f99078419c28");

                    using Aes aesAlg2 = Aes.Create();
                    aesAlg2.Key = Key2;
                    aesAlg2.IV = IV2;
                    aesAlg2.Padding = PaddingMode.None;
                    aesAlg2.Mode = CipherMode.ECB;

                    ICryptoTransform encryptor2 = aesAlg2.CreateEncryptor(aesAlg2.Key, aesAlg2.IV);

                    using MemoryStream ms = new(bytef);
                    using CryptoStream cs = new(ms, encryptor2, CryptoStreamMode.Read);

                    cs.Read(bytef, 0, FileLen);

                    //Console.WriteLine($"Decrypted: {Path.GetFileName(files[i])} {i}/{files.Length}");
                }
                else if (name.Contains("Local"))
                {
                    if (!name.Contains("ImageData"))
                    {
                        byte[] IV2 = new byte[16];
                        byte[] Key2 = new byte[16];

                        if (ver.ToLower() == "yes")
                        {
                            byte[] ivtemp = { 0x40, 0xb2, 0x13, 0x1a, 0x9f, 0x38, 0x8a, 0xd4, 0xe5, 0x00, 0x2a, 0x98, 0x11, 0x8f, 0x61, 0x28 };
                            byte[] keytemp = { 0xd7, 0x54, 0x86, 0x8d, 0xe8, 0x9d, 0x71, 0x7f, 0xa9, 0xe7, 0xb0, 0x6d, 0xa4, 0x5a, 0xe9, 0xe3 };
                            Key = keytemp;
                            IV = ivtemp;
                        }
                        else
                        {
                            byte[] leytemp = { 0x0a, 0xd3, 0x9e, 0x4a, 0xea, 0xf5, 0x5a, 0xa7, 0x17, 0xfe, 0xb1, 0x82, 0x5e, 0xde, 0xf5, 0x21 };
                            byte[] ivtemp = { 0xd1, 0xd7, 0xe7, 0x08, 0x09, 0x19, 0x41, 0xd9, 0x0c, 0xdf, 0x8a, 0xa5, 0xf3, 0x0b, 0xb0, 0xc2 };
                            Key = leytemp;
                            IV = ivtemp;
                        }

                        using Aes aesAlg2 = Aes.Create();
                        aesAlg2.Key = Key;
                        aesAlg2.IV = IV;
                        aesAlg2.Padding = PaddingMode.None;
                        aesAlg2.Mode = CipherMode.CBC;

                        ICryptoTransform encryptor2 = aesAlg2.CreateEncryptor(aesAlg2.Key, aesAlg2.IV);

                        using MemoryStream ms = new(bytef);
                        using CryptoStream cs = new(ms, encryptor2, CryptoStreamMode.Read);

                        cs.Read(bytef, 0, FileLen);
                    }

                    //Console.WriteLine($"Decrypted: {Path.GetFileName(files[i])} {i}/{files.Length}");
                }
                bytef.CopyTo(PackBytes, preIndex);
                preIndex += bytef.Length;
            }
            File.WriteAllBytes(@"CompFiles/" + name + ".pack", PackBytes);
            Console.WriteLine("Done\nThe .list and .pack file can be found in " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/CompFiles/");
        }

    }
}
