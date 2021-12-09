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
        public static void MD5Lib()
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
            string path = fd.FileName;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            List<KeyValuePair<string, byte[]>> listsHash = new();

            string[] order = { "DataLocal.list", "ImageDataLocal.list", "ImageLocal.list", "MapLocal.list", "NumberLocal.list", "resLocal.list", "UnitLocal.list", "ImageDataLocal_fr.list", "ImageLocal_fr.list", "MapLocal_fr.list", "resLocal_fr.list", "ImageDataLocal_it.list", "ImageLocal_it.list", "MapLocal_it.list", "resLocal_it.list", "ImageDataLocal_de.list", "ImageLocal_de.list", "MapLocal_de.list", "resLocal_de.list", "ImageDataLocal_es.list", "ImageLocal_es.list", "MapLocal_es.list", "resLocal_es.list", "DataLocal.pack", "ImageDataLocal.pack", "ImageLocal.pack", "MapLocal.pack", "NumberLocal.pack", "resLocal.pack", "UnitLocal.pack", "ImageDataLocal_fr.pack", "ImageLocal_fr.pack", "MapLocal_fr.pack", "resLocal_fr.pack", "ImageDataLocal_it.pack", "ImageLocal_it.pack", "MapLocal_it.pack", "resLocal_it.pack", "ImageDataLocal_de.pack", "ImageLocal_de.pack", "MapLocal_de.pack", "resLocal_de.pack", "ImageDataLocal_es.pack", "ImageLocal_es.pack", "MapLocal_es.pack", "resLocal_es.pack", "ImageServer_101000_00_en.list", "MapServer_101000_00_en.list", "NumberServer_101000_00_en.list", "UnitServer_101000_00_en.list", "ImageServer_100900_00_en.list", "MapServer_100900_00_en.list", "NumberServer_100900_00_en.list", "UnitServer_100900_00_en.list", "ImageServer_100800_00_en.list", "MapServer_100800_00_en.list", "NumberServer_100800_00_en.list", "UnitServer_100800_00_en.list", "ImageServer_100700_00_en.list", "MapServer_100700_00_en.list", "NumberServer_100700_00_en.list", "UnitServer_100700_00_en.list", "", "ImageServer_100600_01_en.list", "MapServer_100600_02_en.list", "NumberServer_100600_03_en.list", "UnitServer_100600_04_en.list", "LImageServer.list", "LMapServer.list", "LNumberServer.list", "LUnitServer.list", "KImageServer.list", "KMapServer.list", "KNumberServer.list", "KUnitServer.list", "JImageSever.list", "JMapServer.list", "JNumberServer.list", "JUnitServer.list", "IImageServer.list", "IMapServer.list", "INumberServer.list", "IUnitServer.list", "HImageServer.list", "HMapServer.list", "HNumberServer.list", "HUnitServer.list", "GImageServer.list", "GMapServer.list", "GNumberServer.list", "GUnitServer.list", "FImageServer.list", "FMapServer.list", "FNumberServer.list", "FUnitServer.list", "EImageServer.list", "EMapServer.list", "ENumberServer.list", "EUnitServer.list", "DImageServer.list", "DMapServer.list", "DNumberServer.list", "DUnitServer.list", "CImageServer.list", "CMapServer.list", "CNumberServer.list", "CUnitServer.list", "BNumberServer.list", "BUnitServer.list", "AMapServer.list", "ANumberServer.list", "AUnitServer.list", "ImageServer.list", "MapServer.list" };
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
            string[] paths = fd2.FileNames;
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
                    Console.WriteLine("The .pack/.list's md5 sum doesn't get checked(so is good to use without getting an error), or the file name is spelt wrong");
                }
                Console.WriteLine("Done!, you should now be able to put the lib file and the .pack/.lists into the game without data read error h01");
            }
        }

    }
}
