using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class MD5Libnative
    {
        public static string[] GetStrNullSep(string path, int[] positions)
        {
            List<byte> allData = File.ReadAllBytes(path).ToList();

            string[] strs = new string[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                int startpos = 0;
                for (int j = 0; j < 64; j++)
                {
                    if (allData[positions[i] - j] == 0)
                    {
                        startpos = positions[i] - j + 1;
                        break;
                    }
                }
                int endpos = positions[i] + 5;

                List<byte> bytes = allData.GetRange(startpos, endpos - startpos);

                strs[i] = Encoding.ASCII.GetString(bytes.ToArray());
            }
            return strs;
        }
        public static string[] GetOrder(string path)
        {
            int len = File.ReadAllBytes(path).Length;

            byte[] conditions_list = { 0x2e, 0x6c, 0x69, 0x73, 0x74, 0x00 };
            byte[] conditions_pack = { 0x2e, 0x70, 0x61, 0x63, 0x6b, 0x00 };
            Console.WriteLine("Getting pack and list positions, please wait...");
            int[] poses = Editor.Search(path, conditions_list, endpoint: len - 200000, startpoint: 700000, stop_after: 128);
            int[] poses2 = Editor.Search(path, conditions_pack, endpoint: len - 200000, startpoint: 700000, stop_after: 128);
            List<string> file_str = GetStrNullSep(path, poses).ToList();
            file_str.AddRange(GetStrNullSep(path, poses2).ToList());

            List<string> local_files = new();
            List<string> server_files = new();

            List<string> full_order = new();

            foreach (string file in file_str)
            {
                bool isPack = false;
                if (file.ToLower().Contains("pack"))
                {
                    isPack = true;
                }
                string[] split_str = file.Split('_');

                if (file.ToLower().Contains("local"))
                {
                    if (split_str.Length  == 2)
                    {
                        if (file.ToLower().Contains("unit") || file.ToLower().Contains("number"))
                        {
                            continue;
                        }
                    }
                    local_files.Add(file);
                }
                else if (file.ToLower().Contains("server"))
                {
                    if (isPack)
                    {
                        continue;
                    }
                    if (split_str.Length == 4 && !split_str[3].ToLower().Contains("en"))
                    {
                        continue;
                    }
                    server_files.Add(file);
                }
            }
            full_order.AddRange(local_files);
            full_order.AddRange(server_files);

            return full_order.ToArray();
        }
        public static void MD5Lib(string path_orig)
        {
            string path = "";

            if (path_orig.EndsWith(".so"))
            {
                path = path_orig;
            }
            else
            {
                Console.WriteLine("Please select an so file");
                OpenFileDialog fd = new()
                {
                    Filter = "files (*.so)|*.so"
                };
                if (fd.ShowDialog() != DialogResult.OK)
                {
                    Console.WriteLine("Please select .so files");
                    Editor.Options();
                }
                path = fd.FileName;
            }


            List<string> order = GetOrder(path).ToList();

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            List<KeyValuePair<string, byte[]>> listsHash = new();

            int prevIndex = 0;
            for (int i = 6000000; i < length - 10000; i++)
            {
                if (allData[i] == 0x2e && allData[i + 1] == 0x70 && allData[i + 2] == 0x61 && allData[i + 3] == 0x63 && allData[i + 4] == 0x6b)
                {
                    prevIndex = i + 6;
                    for (int j = i + 4; j > i - 64; j--)
                    {
                        if (allData[j] == 0x00)
                        {
                            break;
                        }
                    }
                }
            }
            int count = 0;
            int num = 0;
            int pos = 0;
            if (order[0] == "ImageDataLocal.list")
            {
                for (int i = 0; i < order.Count; i++)
                {
                    if (order[i].Contains("ImageDataLocal."))
                    {
                        order[i] = order[i].Replace("ImageDataLocal", "DataLocal");
                    }
                }
                order.Add("ImageServer.list");
                order.Add("MapServer.list");
            }
            for (int i = prevIndex; i < length; i++)
            {
                num++;
                string listName = "";
                try
                {
                    listName = order[count];
                }
                catch
                {
                    pos = i;
                    break;
                }
                string hash = "";
                if (num % 33 == 0)
                {
                    for (int j = i - 32; j < i; j++)
                    {
                        hash += Convert.ToChar(allData[j]);
                    }
                    count++;
                    byte[] hash3 = Encoding.ASCII.GetBytes(hash);
                    listsHash.Add(new KeyValuePair<string, byte[]>(listName, hash3));
                }
            }
            Console.WriteLine("Please select .pack and .list files, you can select multiple at once");
            OpenFileDialog fd2 = new()
            {
                Filter = "files (*.pack; .list)|*.pack;*.list",
                Multiselect = true
            };
            if (fd2.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .pack/.list files");
                Editor.Options();
            }
            List<string> paths = fd2.FileNames.ToList();
            List<string> fileNames = new();
            List<string> hashes = new();
            List<string> unchecked_files = new();
            List<string> unchecked_hashes = new();
            foreach (string path2 in paths)
            {
                string hash2 = Editor.CalculateMD5(path2);
                byte[] hashBytes = Encoding.ASCII.GetBytes(hash2);
                bool found = false;
                for (int i = 0; i < listsHash.Count; i++)
                {
                    if (Path.GetFileName(path2) == listsHash[i].Key)
                    {
                        found = true;
                        hashes.Add(hash2);
                        fileNames.Add(Path.GetFileName(path2));
                        for (int j = prevIndex; j < length - 10000; j++)
                        {
                            if (allData[j] == listsHash[i].Value[0] && allData[j + 1] == listsHash[i].Value[1] && allData[j + 2] == listsHash[i].Value[2] && allData[j + 3] == listsHash[i].Value[3] && allData[j + 4] == listsHash[i].Value[4] && allData[j + 5] == listsHash[i].Value[5] && allData[j + 6] == listsHash[i].Value[6] && allData[j + 7] == listsHash[i].Value[7] && allData[j + 8] == listsHash[i].Value[8] && allData[j + 9] == listsHash[i].Value[9] && allData[j + 10] == listsHash[i].Value[10] && allData[j + 11] == listsHash[i].Value[11] && allData[j + 12] == listsHash[i].Value[12] && allData[j + 13] == listsHash[i].Value[13] && allData[j + 14] == listsHash[i].Value[14] && allData[j + 15] == listsHash[i].Value[15])
                            {
                                stream.Position = j;
                                stream.Write(hashBytes, 0, hashBytes.Length);
                                break;
                            }
                        }
                    }
                }
                if (!found)
                {
                    unchecked_hashes.Add(hash2);
                    unchecked_files.Add(Path.GetFileName(path2));
                }
            }
            Editor.ColouredText($"&Done, successfully changed checksum for files:\n&{Editor.CreateOptionsList(fileNames.ToArray(), hashes.ToArray(), false)}");
            Editor.ColouredText($"&\nFiles that don't get checked:\n&{Editor.CreateOptionsList(unchecked_files.ToArray(), unchecked_hashes.ToArray(), false)}");
        }

    }
}
