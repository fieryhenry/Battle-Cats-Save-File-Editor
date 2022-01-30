using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class OtotoCatCannon
    {
        static int cat_cannon_types = 8;
        public static List<Tuple<int, int>> GetCatCannons(string path)
        {
            int pos = Editor.GetOtotoPos(path) + 15;
            byte[] all_data = File.ReadAllBytes(path);
            List<Tuple<int, int>> cat_cannon_data = new();
            for (int i = 0; i < cat_cannon_types; i++)
            {
                int offset = i * 16;
                int unlocked = all_data[pos + offset];
                int level = all_data[pos + offset + 4];
                cat_cannon_data.Add(Tuple.Create(unlocked, level));
            }
            return cat_cannon_data;
        }
        public static void SetCatCannons(string path, List<Tuple<int, int>> cat_cannon_data)
        {
            int pos = Editor.GetOtotoPos(path) + 15;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            for (int i = 0; i < cat_cannon_data.Count; i++)
            {
                int offset = i * 16;
                stream.Position = pos + offset;
                stream.WriteByte((byte)cat_cannon_data[i].Item1);
                stream.Position = pos + offset + 4;
                stream.WriteByte((byte)cat_cannon_data[i].Item2);
            }
        }
        public static string[] cat_cannon_types_str =
        {
            "Improve Base", "Develop Slow Beam", "Develop Iron Wall", "Develop Thunderbolt", "Develop Waterblast", "Develop Holy Blast", "Develop Breakerblast", "Develop CurseBlast"
        };
        public static void CatCannon(string path)
        {
            List<Tuple<int, int>> cat_cannons = GetCatCannons(path);
            List<int> cat_cannon_levels = new();
            foreach (Tuple<int,int> cat_cannon in cat_cannons)
            {
                cat_cannon_levels.Add(cat_cannon.Item2 +1);
            }
            Editor.ColouredText($"&What do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList(cat_cannon_types_str, cat_cannon_levels.ToArray())}&{cat_cannon_types + 1}. &All at once\n");
            string[] response = Console.ReadLine().Split(' ');
            foreach (string cannon in response)
            {
                int cannon_id = int.Parse(cannon) -1;
                if (cannon_id == cat_cannon_types)
                {
                    Editor.ColouredText($"&What level do you want to upgrade the cat cannons to?(max &30&)\n");
                    int level = (int)Editor.Inputed();
                    for (int i = 0; i < cat_cannon_types; i++)
                    {
                        int max = 30;
                        if (i == 0) max = 20;
                        Tuple<int, int> cannon_data = Tuple.Create(3, Editor.MaxMinCheck(level, max) -1);
                        cat_cannons[i] = cannon_data;
                    }
                }
                else
                {
                    int max = 30;
                    if (cannon_id == 0) max = 20;
                    Editor.ColouredText($"&What level do you want for &{cat_cannon_types_str[cannon_id]}&? (max &{max}&)\n");
                    int level = (int)Editor.Inputed();
                    level = Editor.MaxMinCheck(level, max);
                    Tuple<int, int> cannon_data = Tuple.Create(3, level -1);
                    cat_cannons[cannon_id] = cannon_data;
                }
            }
            SetCatCannons(path, cat_cannons);
            cat_cannon_levels = new();
            foreach (Tuple<int, int> cat_cannon in cat_cannons)
            {
                cat_cannon_levels.Add(cat_cannon.Item2 +1);
            }
            Editor.ColouredText($"&Set cat cannons to:\n&{Editor.CreateOptionsList(cat_cannon_types_str, cat_cannon_levels.ToArray())}");
        }
        public static void CatCannon_old(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int pos = 0;

            // Search for cat cannon position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 8 && allData[i + 4] == 0 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 2 && allData[i + 12] == 0 && allData[i + 13] == 0 && allData[i + 14] == 0 && allData[i + 15] == 3 && allData[i + 16] == 0 && allData[i + 17] == 0 && allData[i + 18] == 0)
                {
                    pos = i + 15;
                    break;
                }
            }
            Editor.ColouredText("What cat cannon type do you want to edit?:\n&1.& Improve Base\n&2.& Develop Slow Beam\n&3.& Develop Iron Wall\n&4.& Develop Thunderbolt\n&5.& Develop Waterblast\n&6.& Develop Holy Blast\n&7.& Develop Breakerblast\n&8.& Develop Curseblast\n&You can edit multiple at once by entering multiple numbers separated by spaces\n");
            string[] answer = Console.ReadLine().Split(' ');
            for (int i = 0; i < answer.Length; i++)
            {
                int choice = int.Parse(answer[i]);
                int max = 30;

                // Set max upgrade amount
                if (choice == 1)
                {
                    max = 20;
                }
                // Position for the cat cannon is unlocked flag
                int unlockPos = pos + (16 * (choice - 1));
                stream.Position = unlockPos;
                stream.WriteByte(3);

                Console.WriteLine($"What level do you want for {choice}? (max {max})");
                int level = (int)Editor.Inputed();
                if (level == 0)
                {
                    stream.Position = unlockPos;
                    stream.WriteByte(0);
                }
                else if (level > max) level = max;
                else if (level < 1) level = 1;
                level -= 1;

                int levelPos = unlockPos + 4;
                stream.Position = levelPos;
                stream.WriteByte((byte)level);
            }
        }
    }
}
