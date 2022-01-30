using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            content = FileHandler.AddExtraBytes(null, false, content);
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
        public static List<Tuple<string, int, int>> DecodeList(string path)
        {
            List<List<string>> csv_data = FileHandler.ReadStringCSV(path);
            List<Tuple<string, int, int>> decoded_csv_data = new();
            foreach (List<string> line in csv_data)
            {
                string file_name = line[0];
                int start_offset = int.Parse(line[1]);
                int length = int.Parse(line[2]);
                Tuple<string, int, int> line_data = Tuple.Create(file_name, start_offset, length);
                decoded_csv_data.Add(line_data);
            }
            return decoded_csv_data;
        }
        public static List<Tuple<string, int, int>> UnpackList(Tuple<string, string> file_group)
        {
            List<Tuple<string, int, int>> list_data;

            byte[] pack_md5 = Encoding.ASCII.GetBytes("b484857901742afc");
            byte[] empty_iv = new byte[16];

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = pack_md5;
            aesAlg.IV = empty_iv;
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;

            byte[] encrypted_list_data = File.ReadAllBytes(file_group.Item2);
            byte[] decrypted_list_data = DecryptData(aesAlg, encrypted_list_data);
            string path = $@"{decrypted_lists}/{Path.GetFileName(file_group.Item2)}";

            File.WriteAllBytes(path, decrypted_list_data);
            list_data = DecodeList(path);
            return list_data;
        }
        public static bool UnpackPack(string path, List<Tuple<string, int, int>> list_data, string version, int index, int total_files)
        {
            List<byte> allData = File.ReadAllBytes(path).ToList();

            string file_name = Path.GetFileName(path);

            using Aes aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            byte[] Key;
            byte[] IV;
            if (version.ToLower() == "yes")
            {
                Key = Editor.StringToByteArray("d754868de89d717fa9e7b06da45ae9e3");
                IV = Editor.StringToByteArray("40b2131a9f388ad4e5002a98118f6128");
            }
            else
            {
                Key = Editor.StringToByteArray("0ad39e4aeaf55aa717feb1825edef521");
                IV = Editor.StringToByteArray("d1d7e708091941d90cdf8aa5f30bb0c2");
            }
            if (file_name.ToLower().Contains("server"))
            {
                aesAlg.Mode = CipherMode.ECB;
                Key = Encoding.ASCII.GetBytes("89a0f99078419c28");
                IV = new byte[16];
            }
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Padding = PaddingMode.None;
            if (list_data.Count == 0) return false;

            current_bytes_unpacked = 0;
            current_files_unpacked = 0;


            for (int i = 0; i < list_data.Count; i++)
            {
                Tuple<string, int, int> file = list_data[i];
                byte[] file_data = allData.GetRange(file.Item2, file.Item3).ToArray();
                byte[] data_to_use;
                if (file_name.ToLower().Contains("imagedatalocal"))
                {
                    data_to_use = file_data;
                }
                else
                {
                    data_to_use = DecryptData(aesAlg, file_data);
                }
                Directory.CreateDirectory(@"game_files/" + Path.GetFileNameWithoutExtension(path));
                File.WriteAllBytes(@"game_files/" + Path.GetFileNameWithoutExtension(path) + "/" + file.Item1, data_to_use);

                float percentageF = ((float)i / (list_data.Count - 1)) * 100;
                string percentage = percentageF.ToString("0.0");
                float totalPercentageF = (float)(index + 1) / total_files * 100;
                string totalPercentage = totalPercentageF.ToString("0.0");
                    
                Editor.ColouredText($"\r&Extracted: &{file.Item1,35} &from &{Path.GetFileName(file_name), 22} & {i, 4}&/&{(list_data.Count - 1)} &{percentage,4}%& - {(index + 1),2}/{total_files}  &{totalPercentage,4}%&                                   ", ConsoleColor.White, ConsoleColor.Green);
                total_files_unpacked++;
                total_bytes_unpacked += data_to_use.Length;
                current_files_unpacked++;
                current_bytes_unpacked += data_to_use.Length;
            }
            return true;
        }
        static readonly string decrypted_lists = @"decrypted_lists";
        static int total_files_unpacked = 0;
        static int current_files_unpacked = 0;
        static long current_bytes_unpacked = 0;
        static long total_bytes_unpacked = 0;
        public static void Decrypt()
        {

            Console.WriteLine("Are you running game version jp 10.8 and up? (yes, no)?");
            string ver = Console.ReadLine();
            OpenFileDialog fd = new()
            {
                Multiselect = true,
                Filter = "files (*.pack)|*.pack"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .pack files");
                Editor.Options();
            }
            string[] paths = fd.FileNames;
            if (!Directory.Exists(decrypted_lists))
            {
                Directory.CreateDirectory(decrypted_lists);
            }
            Stopwatch timer_total = Stopwatch.StartNew();

            List<Tuple<string, string>> files = new();
            foreach (string path in paths)
            {
                if (path.EndsWith(".pack"))
                {
                    string dir = Path.GetDirectoryName(path);
                    string list_path = Path.Combine(dir, path.TrimEnd(".pack".ToCharArray()) + ".list");
                    Tuple<string, string> group = Tuple.Create(path, list_path);
                    files.Add(group);
                }
            }
            for (int i = 0; i < files.Count; i++)
            {
                Stopwatch timer = Stopwatch.StartNew();

                Tuple<string, string> file_group = files[i];
                List<Tuple<string, int, int>> list_data = UnpackList(file_group);

                bool success = UnpackPack(file_group.Item1, list_data, ver, i, files.Count);

                timer.Stop();
                TimeSpan timespan = timer.Elapsed;

                string time = string.Format("&{0:00}&:&{1:00}&:&{2:00}&", timespan.Minutes, timespan.Seconds, timespan.Milliseconds / 10);

                if (success)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Editor.ClearCurrentConsoleLine();
                    Editor.ColouredText($"&Unpacked: &{Path.GetFileName(file_group.Item1), 35}& to &{@"game_files/" + Path.GetFileNameWithoutExtension(file_group.Item1), 40}/& in {time, 5} & {current_files_unpacked, 5}& files, and &{Editor.ConvertBytesToMegabytes(current_bytes_unpacked), 5} MB&\n", New: ConsoleColor.Cyan);
                }
            }
            timer_total.Stop();
            TimeSpan timespan2 = timer_total.Elapsed;

            string time2 = string.Format("&{0:00}&:&{1:00}&:&{2:00}&", timespan2.Minutes, timespan2.Seconds, timespan2.Milliseconds / 10);
            Editor.ColouredText($"&Finished: unpacked &{files.Count}& packs, &{total_files_unpacked}& files and &{Editor.ConvertBytesToMegabytes(total_bytes_unpacked)} MB& in {time2}\n                                                      \n", New: ConsoleColor.Magenta);

        }
    }
}
