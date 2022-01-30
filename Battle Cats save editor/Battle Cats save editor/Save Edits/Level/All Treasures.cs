using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class AllTreasures
    {
        public static int GetTreasurePos(string path)
        {
            return 2986 + Editor.GetGameVersionOffset(path);
        }
        public static List<List<int>> GetTreasures(string path)
        {
            int pos = GetTreasurePos(path);
            int chapter_amount = 9;
            List<List<int>> treasure_data = new();
            for (int i = 0; i < chapter_amount; i++)
            {
                int offset = 0;
                if (i > 2)
                {
                    offset = 1;
                }
                List<int> chapter_data=  Editor.GetItemData(path, 48, 4, pos + ((i + offset) * 196), warning:false).ToList();
                treasure_data.Add(chapter_data);
            }
            return treasure_data;
        }
        public static void SetTreasures(string path, List<List<int>> treasure_data)
        {
            int pos = GetTreasurePos(path);
            for (int i = 0; i < treasure_data.Count; i++)
            {
                int offset = 0;
                if (i > 2)
                {
                    offset = 1;
                }
                Editor.SetItemData(path, treasure_data[i].ToArray(), 4, pos + ((i + offset) * 196));
            }
        }
        public static string[] chapters =
        {
                "Empire of Cats 1", "Empire of Cats 2", "Empire of Cats 3",
                "Into the Future 1", "Into the Future 2", "Into the Future 3",
                "Cats of the Cosmos 1", "Cats of the Cosmos 2", "Cats of the Cosmos 3"
        };
        public static void MaxTreasures(string path)
        {
            Editor.ColouredText($"&What chapter do you want to edit{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(chapters)}&{chapters.Length + 1}.& All at once\n");
            string[] choices = Console.ReadLine().Split(' ');
            bool individual = false;
            List<List<int>> treasure_data = GetTreasures(path);
            if (choices.Length > 1)
            {
                Editor.ColouredText("&Do you want to set the treasure level indivudally for each chapter(&1&), or all at once?(&2&)\n");
                string response = Console.ReadLine();
                if (response == "1")
                {
                    individual = true;
                }
            }
            int level = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                int chapter_id = int.Parse(choices[i]) -1;
                if (!individual && i == 0)
                {
                    Editor.ColouredText($"&What treasure level do you want? (0=none, 1=inferior, 2=normal, 3=superior):\n");
                    level = (int)Editor.Inputed();
                }
                else if (individual)
                {
                    Editor.ColouredText($"&What treasure level do you want for chapter {chapters[chapter_id]}? (0=none, 1=inferior, 2=normal, 3=superior):\n");
                    level = (int)Editor.Inputed();
                }
                if (chapter_id == 9)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        treasure_data[j] = Enumerable.Repeat(level, 48).ToList();
                    }
                }
                else
                {
                    treasure_data[chapter_id] = Enumerable.Repeat(level, 48).ToList();
                }
            }
            SetTreasures(path, treasure_data);
            Console.WriteLine($"Set treasures successfuly");

        }
    }
}
