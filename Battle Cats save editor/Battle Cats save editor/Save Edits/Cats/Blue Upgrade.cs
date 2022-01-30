using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class BlueUpgrade
    {
        public static int TotalUpgrades = 10;
        public static Tuple<List<int>, List<int>> GetBlueUpgrades(string path)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[2] + (Editor.GetCatAmount(path) * 4);
            int[] Levels = Editor.GetItemData(path, (TotalUpgrades + 1) * 2, 2, pos + 4);
            List<int> plusLevels = new();
            List<int> baseLevels = new();

            for (int i = 0; i < Levels.Length; i++)
            {
                if (i == 2 || i == 3)
                {
                    continue;
                }
                if (i % 2 == 0)
                {
                    plusLevels.Add(Levels[i]);
                }
                else
                {
                    baseLevels.Add(Levels[i] + 1);
                }
            }

            return Tuple.Create(baseLevels, plusLevels);
        }
        public static void SetBlueUpgrades(string path, Tuple<List<int>, List<int>> upgrades)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[2] + (Editor.GetCatAmount(path) * 4);
            upgrades.Item1.Insert(1, 1);
            upgrades.Item2.Insert(1, 0);

            List<int> levels = new();

            for (int i = 0; i < upgrades.Item1.Count; i++)
            {
                levels.Add(upgrades.Item2[i]);
                levels.Add(upgrades.Item1[i] - 1);
            }
            Editor.SetItemData(path, levels.ToArray(), 2, pos + 4);
        }
        public static void Blue(string path)
        {
            string[] types =
            {
                "Cat Cannon Attack", "Cat Cannon Range", "Cat Cannon Charge",
                "Worker Cat Rate", "Worker Cat Wallet", "Base Defnse",
                "Research", "Accountant", "Study", "Cat Energy"
            };
            Tuple<List<int>, List<int>> upgrades = GetBlueUpgrades(path);
            int[] baseLevels = upgrades.Item1.ToArray();
            int[] plusLevels = upgrades.Item2.ToArray();

            string toOutput = "&Current Levels:\n";

            for (int i = 0;i < baseLevels.Length; i++)
            {
                toOutput += $"&{types[i]}&: {baseLevels[i]}+{plusLevels[i]}\n";
            }
            Editor.ColouredText(toOutput);
            Editor.ColouredText($"&What do you want to edit? {Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(types)}&{types.Length +1}.& All at once\n");
            string[] answer = Console.ReadLine().Split(' ');
            foreach (string choice in answer)
            {
                int id = int.Parse(choice) -1;
                if (id == types.Length)
                {
                    Editor.ColouredText($"&What do you want to set the level to? (e.g 20+10):\n");
                    string[] response = Console.ReadLine().Split('+');
                    int base_level = int.Parse(response[0]);
                    int plus_level = int.Parse(response[1]);

                    if (base_level == 0)
                    {
                        base_level = 1;
                    }

                    List<int> base_level_set = Enumerable.Repeat(base_level, upgrades.Item1.Count).ToList();
                    List<int> plus_level_set = Enumerable.Repeat(plus_level, upgrades.Item2.Count).ToList();
                    upgrades = Tuple.Create(base_level_set, plus_level_set);
                }
                else
                {
                    Editor.ColouredText($"&What do you want to set the level of &{types[id]}& to? (e.g 20+10):\n");
                    string[] response = Console.ReadLine().Split('+');
                    int base_level = int.Parse(response[0]);
                    int plus_level = int.Parse(response[1]);

                    if (base_level == 0)
                    {
                        base_level = 1;
                    }

                    upgrades.Item1[id] = base_level;
                    upgrades.Item2[id] = plus_level;
                }
            }
            SetBlueUpgrades(path, upgrades);
        }
    }
}
