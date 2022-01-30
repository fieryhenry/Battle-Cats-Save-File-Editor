using Microsoft.WindowsAPICodePack.Dialogs;
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
        public static void EncryptData()
        {
            Console.WriteLine("Please select a folder of game content");
            CommonOpenFileDialog dialog = new();
            dialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\game_files";
            dialog.IsFolderPicker = true;
            dialog.Title = "Select a folder of game files";

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                EncryptData();
            }
            string path = dialog.FileName;
            string name = Path.GetFileName(path);

            string final_folder_name = "encrypted_files/";
            if (!Directory.Exists(final_folder_name))
            {
                Directory.CreateDirectory(final_folder_name);
            }

            Console.WriteLine("Creating list file...");
            byte[] list_bytes = CreateListFile(path);
            File.WriteAllBytes(final_folder_name + name + ".list", list_bytes);
            Console.WriteLine("Done");

            Console.WriteLine("Creating pack file...");
            byte[] pack_bytes = CreatePackFile(path);
            File.WriteAllBytes(final_folder_name + name + ".pack", pack_bytes);
            Console.WriteLine("Done");

            Editor.ColouredText($"&Encrypted files can be found at &{Path.GetFullPath(final_folder_name)}{name}.pack&\n");

        }
        public static byte[] EncryptListFile(string list_file)
        {
            using Aes aes = Aes.Create();

            byte[] IV = new byte[16];
            byte[] Key = Encoding.ASCII.GetBytes("b484857901742afc");

            aes.Key = Key;
            aes.IV = IV;
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.ECB;
            return EncryptAES(null, aes, Encoding.ASCII.GetBytes(list_file));
        }
        public static byte[] CreateListFile(string dir_name)
        {
            string[] files = Directory.GetFiles(dir_name);

            string list_file = files.Length + "\n";
            int offset = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                FileInfo fi = new(file);
                int length = (int)fi.Length;
                string file_name = Path.GetFileName(file);

                list_file += $"{file_name},{offset},{length}\n";
                offset += length;
            }
            byte[] list_data_encrypted = EncryptListFile(list_file);
            return list_data_encrypted;
        }
        public static Tuple<byte[], byte[], CipherMode> GetKeyAndIV(string file_name, string ver)
        {
            List<byte> IV = new();
            List<byte> Key = new();
            CipherMode mode = CipherMode.CBC;
            if (file_name.Contains("Server"))
            {
                IV = new byte[16].ToList();
                Key = Encoding.ASCII.GetBytes("89a0f99078419c28").ToList();
                mode = CipherMode.ECB;
            }
            else if (file_name.Contains("Local"))
            {
                if (!file_name.Contains("ImageData"))
                {
                    if (ver.ToLower() == "yes")
                    {
                        Key = Editor.StringToByteArray("d754868de89d717fa9e7b06da45ae9e3").ToList();
                        IV = Editor.StringToByteArray("40b2131a9f388ad4e5002a98118f6128").ToList();
                    }
                    else
                    {
                        Key = Editor.StringToByteArray("0ad39e4aeaf55aa717feb1825edef521").ToList();
                        IV = Editor.StringToByteArray("d1d7e708091941d90cdf8aa5f30bb0c2").ToList();
                    }
                    mode = CipherMode.CBC;
                }
            }
            return Tuple.Create(Key.ToArray(), IV.ToArray(), mode);
        }
        public static byte[] CreatePackFile(string dir_name)
        {
            Console.WriteLine("Are you using jp 10.8 and up?(yes/no)");
            string ver = Console.ReadLine();

            string[] files = Directory.GetFiles(dir_name);
            List<byte> data = new();

            using Aes aes = Aes.Create();
            aes.Padding = PaddingMode.None;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (file.ToLower().Contains("imagedatalocal"))
                {
                    byte[] file_data = File.ReadAllBytes(file);
                    data.AddRange(file_data.ToList());
                }
                else
                {
                    Tuple<byte[], byte[], CipherMode> aes_data = GetKeyAndIV(file, ver);
                    aes.Key = aes_data.Item1;
                    aes.IV = aes_data.Item2;
                    aes.Mode = aes_data.Item3;

                    data.AddRange(EncryptAES(file, aes));
                }
            }
            return data.ToArray();
        }
        public static byte[] EncryptAES(string path, Aes aes, byte[] data = null)
        {
            byte[] bytef;
            if (data != null) bytef = data;
            else
            {
                bytef = File.ReadAllBytes(path);
            }
            bytef = FileHandler.AddExtraBytes(null, false, bytef);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream ms = new(bytef);
            using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Read);
            cs.Read(bytef, 0, bytef.Length);

            return bytef;
        }

    }
}
