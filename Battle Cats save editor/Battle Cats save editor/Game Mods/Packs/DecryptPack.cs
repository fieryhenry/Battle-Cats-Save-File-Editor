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
    public class DecryptPack
    {
        public static byte[] DecryptData(Aes aesAlg, byte[] content)
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            if (!Directory.Exists("game_files/"))
            {
                Directory.CreateDirectory("game_files");
            }
            MemoryStream memory = new(content);

            using CryptoStream csDecrypt = new(memory, decryptor, CryptoStreamMode.Read);
            byte[] bytes = new byte[content.Length];
            csDecrypt.Read(bytes, 0, content.Length);

            return bytes;
        }
        public static void Decrypt(string key)
        {
            Console.WriteLine("Are you running game version jp 10.8 and up? (yes, no)?");
            string ver = Console.ReadLine();
            OpenFileDialog fd = new()
            {
                Multiselect = true,
                Filter = "files (*.pack; .list)|*.pack;*.list"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .pack and .list files");
                Editor.Options();
            }
            string[] paths = fd.FileNames;
            bool hasPack = false;
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".pack"))
                {
                    hasPack = true;
                }
            }
            if (paths.Length % 2 != 0)
            {
                Console.WriteLine("Please enter a .pack and a .list file\nPress enter to continue");
                Console.ReadLine();
                Editor.Options();
            }
            if (hasPack)
            {
                string[] lists = new string[paths.Length];
                string[] packs = new string[paths.Length];
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].EndsWith(".pack"))
                    {
                        packs[i] = paths[i];
                    }
                    else if (paths[i].EndsWith(".list"))
                    {
                        lists[i] = paths[i];
                    }
                }
                string[] listLines = new string[25000];
                string[] names = new string[listLines.Length];
                int[] startpos = new int[listLines.Length];
                int[] offset = new int[listLines.Length];
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].EndsWith(".list"))
                    {
                        using var stream = new FileStream(paths[i], FileMode.Open, FileAccess.ReadWrite);

                        int length = (int)stream.Length;
                        byte[] allData = new byte[length];
                        stream.Read(allData, 0, length);

                        byte[] IV = new byte[16];
                        byte[] Key = Encoding.ASCII.GetBytes(key);

                        string plaintext = null;

                        string result = Path.GetFileName(paths[i]);

                        using Aes aesAlg = Aes.Create();
                        aesAlg.Key = Key;
                        aesAlg.IV = IV;
                        aesAlg.Padding = PaddingMode.None;
                        aesAlg.Mode = CipherMode.ECB;

                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        using MemoryStream msDecrypt = new(allData);
                        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using StreamReader srDecrypt = new(csDecrypt);

                        plaintext = srDecrypt.ReadToEnd();
                        if (!Directory.Exists("Decomp_lists/"))
                        {
                            Directory.CreateDirectory("Decomp_lists/");
                        }
                        File.WriteAllText(@"decomp_lists/" + result, plaintext);
                        listLines = File.ReadAllLines(@"decomp_lists/" + result);
                        for (int j = 1; j < listLines.Length - 1; j++)
                        {
                            try
                            {
                                names[j] = listLines[j].Split(',').ElementAt(0);
                                startpos[j] = int.Parse(listLines[j].Split(',').ElementAt(1));
                                offset[j] = int.Parse(listLines[j].Split(',').ElementAt(2));
                            }
                            catch
                            {
                                startpos[j] = startpos[0];
                                offset[j] = offset[0];
                                names[j] = names[0];
                            }

                        }
                    }
                    else if (paths[i].EndsWith(".pack"))
                    {
                        using var stream = new FileStream(paths[i], FileMode.Open, FileAccess.ReadWrite);

                        int length = (int)stream.Length;
                        byte[] allData = new byte[length];
                        stream.Read(allData, 0, length);
                        if (paths[i].ToLower().Contains("server"))
                        {
                            for (int j = 1; j < listLines.Length - 1; j++)
                            {
                                byte[] content = new byte[offset[j]];
                                Array.Copy(allData, startpos[j], content, 0, offset[j]);

                                byte[] IV = new byte[16];
                                byte[] Key = Encoding.ASCII.GetBytes("89a0f99078419c28");

                                using Aes aesAlg = Aes.Create();
                                aesAlg.Key = Key;
                                aesAlg.IV = IV;
                                aesAlg.Padding = PaddingMode.None;
                                aesAlg.Mode = CipherMode.ECB;

                                byte[] bytes = DecryptData(aesAlg, content);

                                try
                                {
                                    Directory.CreateDirectory(@"game_files/" + Path.GetFileName(paths[i]));
                                    File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], bytes);
                                }
                                catch
                                {

                                }
                                float percentageF = ((float)j / (listLines.Length - 2)) * 100;
                                string percentage = percentageF.ToString("0.0");
                                float totalPercentageF = ((float)(i + 1) / 2) / (paths.Length / 2) * 100;
                                string totalPercentage = totalPercentageF.ToString("0.0");
                                Editor.ColouredText($"\r&Extracted: &{names[j]} &from &{Path.GetFileName(paths[i])} & {j}&/&{(listLines.Length - 2)}  &{percentage}%& - {(i + 1) / 2}/{paths.Length / 2}  &{totalPercentage}%&                                   ", ConsoleColor.White, ConsoleColor.Green);
                            }
                            Console.WriteLine("Decrypted: " + Path.GetFileName(paths[i]) + " " + (i + 1) / 2 + "/" + paths.Length / 2);
                        }
                        else if (paths[i].ToLower().Contains("local"))
                        {
                            for (int j = 1; j < listLines.Length - 1; j++)
                            {
                                byte[] content = new byte[offset[j]];
                                Array.Copy(allData, startpos[j], content, 0, offset[j]);
                                byte[] IV = new byte[16];
                                byte[] Key = new byte[16];

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

                                using Aes aesAlg = Aes.Create();
                                aesAlg.Key = Key;
                                aesAlg.IV = IV;
                                aesAlg.Padding = PaddingMode.None;
                                aesAlg.Mode = CipherMode.CBC;


                                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                                if (!Directory.Exists("game_files/"))
                                {
                                    Directory.CreateDirectory("game_files");
                                }
                                try
                                {
                                    Directory.CreateDirectory(@"game_files/" + Path.GetFileName(paths[i]));
                                }
                                catch { }
                                if (paths[i].ToLower().Contains("imagedata"))
                                {
                                    File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], content);
                                }
                                else
                                {
                                    byte[] bytes = DecryptData(aesAlg, content);

                                    try
                                    {
                                        File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], bytes);
                                    }
                                    catch { }
                                }

                                float percentageF = ((float)j / (listLines.Length - 2)) * 100;
                                string percentage = percentageF.ToString("0.0");
                                float totalPercentageF = ((float)(i + 1) / 2) / (paths.Length / 2) * 100;
                                string totalPercentage = totalPercentageF.ToString("0.0");
                                Editor.ColouredText($"\r&Extracted: &{names[j]} &from &{Path.GetFileName(paths[i])} & {j}&/&{(listLines.Length - 2)}  &{percentage}%& - {(i + 1) / 2}/{paths.Length / 2}  &{totalPercentage}%&                                   ", ConsoleColor.White, ConsoleColor.Green);

                            }
                            Console.WriteLine("\nDecrypted: " + Path.GetFileName(paths[i]) + " " + (i + 1) / 2 + "/" + paths.Length / 2);
                        }
                    }
                }
                Console.WriteLine("Finished: files can be found in " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/game_files/");
            }
        }
    }
}
