using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class SpecificTreasures
    {
        static List<List<string>> treasure_types_str = new List<List<string>>()
        {
            new List<string> {"Energy Drink", "Giant Safe", "Relativity Clock", "Philosopher's Stone", "Smart Material Wall", "Super Register", "Legendary Cat Shield", "Legendary Cat Sword", "Energy Core", "Turbo Machine", "Management Bible" },
            new List<string> {"Aqua Crystal", "Plasma Crystal", "Ancient Tablet", "Mysterious Force", "Cosmic Energy", "Void Fruit", "Blood Fruit", "Sky Fruit", "Heaven's Fruit", "Time Machine", "Future Tech" },
            new List<string> {"Stellar Garnet", "Phoebe Beryl", "Lunar Citrine", "Ganymede Topaz", "Callisto Amethyst", "Titanium Fruit", "Antimatter Fruit", "Enigma Fruit", "Dark Matter", "Neutrino", "Mystery Mask" }
        };
        static List<List<List<int>>> treasure_levels = new List<List<List<int>>>()
        {
            // EoC
            new List<List<int>>
            {
                new List<int> {46, 45, 44, 43, 42, 41, 40 },
                new List<int> {39, 38, 37, 36 },
                new List<int> {35, 34, 33, 32, 31 },
                new List<int> {30, 29, 28, 27, 26, 25, 24},
                new List<int> {23, 22, 19},
                new List<int> {20, 21, 18},
                new List<int> {17, 16, 15},
                new List<int> {14, 13, 12, 11, 10, 9, 8},
                new List<int> {7, 6, 5, 4, 3, 2},
                new List<int> {1},
                new List<int> {47, 48}
            },
            // ItF
            new List<List<int>>
            {
                new List<int> {46, 42, 39, 36, 33, 30, 27, 24},
                new List<int> {22, 19, 16, 13, 10, 7, 4, 1},
                new List<int> {45, 44, 43},
                new List<int> {23},
                new List<int> {41, 40, 38, 37, 35 },
                new List<int> {18, 17, 15, 14},
                new List<int> {26, 25, 21, 20 },
                new List<int> {12, 11, 9, 8 },
                new List<int> {6, 5, 3, 2 },
                new List<int> {34, 32, 31, 29, 28 },
                new List<int> {47, 48}
            },
            // CotC
            new List<List<int>>
            {
                new List<int> {46, 45, 44, 43, 42},
                new List<int> {37, 36, 35, 34, 33},
                new List<int> {28, 27, 26, 25, 24},
                new List<int> {19, 18, 17, 16, 15},
                new List<int> {10, 9, 8, 7, 6},
                new List<int> {41, 40, 39, 38},
                new List<int> {32, 31, 30, 29},
                new List<int> {23, 22, 21, 20},
                new List<int> {14, 13, 12, 11},
                new List<int> {5, 3, 1, 48},
                new List<int> {4, 2, 47}
            }
        };
        public static List<List<List<Tuple<int, int>>>> GetTreasures(string path)
        {
            int pos = AllTreasures.GetTreasurePos(path);
            List<List<int>> treasure_data = new();
            for (int i = 0; i < treasure_types_str.Count * 3 + 1; i++)
            {
                if (i == 3) continue;
                int offset = pos + (200 * i) - (4 * i);
                int[] chapter_treasure_data = Editor.GetItemData(path, 48, 4, offset, false);
                treasure_data.Add(chapter_treasure_data.ToList());
            }
            List<List<List<Tuple<int, int>>>> encoded_treasure_data = EncodeTreasureLevels(treasure_data);
            return encoded_treasure_data;
        }
        public static List<List<List<Tuple<int, int>>>> EncodeTreasureLevels(List<List<int>> treasure_data)
        {
            List<List<List<Tuple<int ,int>>>> encoded_treasure_data = new();
            for (int i = 0; i < treasure_levels.Count * 3; i++)
            {
                List<List<Tuple<int, int>>> chapter_data = new();
                foreach (List<int> treasure_group in treasure_levels[Convert.ToInt32(Math.Floor(Convert.ToDecimal(i / 3)))])
                {
                    List<Tuple<int ,int>> treasure_groups = new();
                    foreach (int level in treasure_group)
                    {
                        Tuple<int, int> level_data = Tuple.Create(level -1, treasure_data[i][level -1]);
                        treasure_groups.Add(level_data);
                    }
                    chapter_data.Add(treasure_groups);
                }
                encoded_treasure_data.Add(chapter_data);
            }
            return encoded_treasure_data;
        }
        public static List<List<int>> DecodeTreasureLevels(List<List<List<Tuple<int, int>>>> encoded_treasure_data)
        {
            List<List<int>> treasure_data = new();
            for (int i = 0; i < encoded_treasure_data.Count; i++)
            {
                List<int> chapter_data = new int[48].ToList();
                foreach (List<Tuple<int, int>> treasure_groups in encoded_treasure_data[i])
                {
                    foreach (Tuple<int, int> level in treasure_groups)
                    {
                        chapter_data[level.Item1] = level.Item2;
                    }
                }
                treasure_data.Add(chapter_data);
            }
            return treasure_data;
        }
        public static void SetTreasures(string path, List<List<List<Tuple<int, int>>>> encoded_treasure_data)
        {
            List<List<int>> treasure_data = DecodeTreasureLevels(encoded_treasure_data);
            int pos = AllTreasures.GetTreasurePos(path);
            for (int i = 0; i < treasure_data.Count; i++)
            {
                if (i == 3) pos += 196;
                int offset = pos + (200 * i) - (4 * i);
                Editor.SetItemData(path, treasure_data[i].ToArray(), 4, offset);
            }
        }
        public static void VerySpecificTreasures(string path)
        {
            List<List<List<Tuple<int, int>>>> encoded_treasure_data = GetTreasures(path);
            Editor.ColouredText($"&What chapter do you want to edit{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(AllTreasures.chapters)}&{AllTreasures.chapters.Length + 1}.& All at once\n");
            string[] user_input = Console.ReadLine().Split(' ');
            foreach (string chapter_str in user_input)
            {
                int chapter_id = int.Parse(chapter_str) -1;
                Editor.ColouredText($"What treasure set do you want to edit?{Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(treasure_types_str[Convert.ToInt32(Math.Floor(Convert.ToDecimal(chapter_id / 3)))].ToArray())}");
                string[] user_sets = Console.ReadLine().Split(' ');
                foreach (string set in user_sets)
                {
                    int group_index = int.Parse(set) -1;

                    if (group_index > AllTreasures.chapters.Length || group_index < 0)
                    {
                        Console.WriteLine($"Error, please enter a number between 1 and 10");
                        continue;
                    }
                    Editor.ColouredText($"&What treasure level do you want? for treasure set &{treasure_types_str[Convert.ToInt32(Math.Floor(Convert.ToDecimal(chapter_id / 3)))][group_index]}& (&0&=none, &1&=inferior, &2&=normal, &3&=superior):\n");
                    int treasure_level = (int)Editor.Inputed();
                    List<Tuple<int, int>> group_data = encoded_treasure_data[chapter_id][group_index];
                    for (int i = 0; i < group_data.Count; i++)
                    {
                        Tuple<int, int> level = Tuple.Create(group_data[i].Item1, treasure_level);
                        encoded_treasure_data[chapter_id][group_index][i] = level;
                    }
                }
            }
            SetTreasures(path, encoded_treasure_data);
        }
    }
}
