using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class UnitMod
    {
        public static void Unitcsv()
        {
            OpenFileDialog fd = new()
            {
                Filter = "files (unit*.csv)|unit*.csv",
                Title = "Select a unit*.csv file"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .csv files");
                Editor.Options();
            }
            string path = fd.FileName;
            List<List<int>> unitData = FileHandler.ReadCSV(path);

            for (int i = 0; i < unitData.Count; i++)
            {
                List<int> extra = new();
                int currLen = unitData[i].Count;
                for (int j = 0; j < values.Length - currLen; j++)
                {
                    int toAdd = 0;
                    switch (currLen + j + 1)
                    {
                        case 56: toAdd = -1; break;
                        case 58: toAdd = -1; break;
                        case 64: toAdd = 1; break;
                        case 67: toAdd = -1; break;
                    }
                    extra.Add(toAdd);

                }
                unitData[i].AddRange(extra);
            }

            Editor.ColouredText("Thanks to this resource: &https://pastebin.com/JrCTPnUV& for help with parsing the unit data\n", ConsoleColor.White, ConsoleColor.Green);

            string[] forms =
            {
                "First Form",
                "Second Form",
                "Third Form"
            };

            Editor.ColouredText($"What do you want to edit?:\n{Editor.CreateOptionsList<string>(forms)}{Editor.multipleVals}\n");
            string[] answer = Console.ReadLine().Split(' ');
            foreach (string selectedForm in answer)
            {
                int choice = int.Parse(selectedForm);
                if (choice > 3 || choice < 1)
                {
                    Console.WriteLine("Please enter a number that is recognised");
                    continue;
                }
                string[] CurrentDataS = Array.ConvertAll(unitData[choice - 1].ToArray(), x => x.ToString());
                Editor.ColouredText($"{Editor.CreateOptionsList(values, unitData[choice-1].ToArray())}What do you want to edit for the {forms[choice - 1]}?:{Editor.multipleVals}\n");
                answer = Console.ReadLine().Split(' ');
                foreach (string idS in answer)
                {
                    int id = int.Parse(idS);
                    Editor.ColouredText($"&What do you want to set &{values[id - 1]}& to?:(for flags, 1 = enable, duration/time = frames, chance = 0->100)\n");
                    int val = (int)Editor.Inputed();
                    unitData[choice - 1][id - 1] = val;
                }
            }
            FileHandler.WriteCSV(unitData, path, true);
        }
        static readonly string[] values =
        {
            "HP", "Knockback amount", "Movement Speed", "Attack Power", "Time between attacks", "Attack Range", "Base cost", "Recharge time",
            "Hit box position", "Hit box size", "Red effective flag", "Always zero", "Area attack flag", "Attack animation", "Min z layer",
            "Max z layer", "Floating effective flag", "Black effective flag", "Metal effective flag", "White effective flag", "Angel effective flag", "Alien effective flag",
            "Zombie effective flag", "Strong against flag", "Knockback chance", "Freeze chance", "Freeze duration", "Slow chance", "Slow duration",
            "Resistant flag", "Triple damage flag", "Critical chance", "Attack only flag", "Extra money from enemies flag", "Base destroyer flag", "Wave chance",
            "Wave attack level", "Weaken chance", "Weaken duration", "Weaken to (decrease attack to percentage left)", "HP remain strength",
            "Boost strength multiplier", "Survive chance", "If unit is metal flag", "Long range start", "Long range append", "Immune to wave flag", "Block wave flag",
            "Resist knockbacks flag", "Resist freeze flag", "Resist slow flag", "Resist weaken flag", "Zombie killer flag", "Witch killer flag","Witch effective flag", "Not effected by boss wave flag",
            "Frames before automatically dying -1 to never die automatically", "Always -1", "Death after attack flag", "Second attack power", "Third attack power", "Second attack animation", "Third attack animation", "Use ability on first hit flag",
            "Second attack flag", "Third attack flag", "Spawn animation, -1, 0", "Soul animation -1, 0, 1, 2, 3, 5, 6, 7", "Unike spawn animation", "Gudetama soul animation",
            "Barrier break chance", "Warp Chance", "Warp Duration", "Min warp distance", "Max warp Distance", "Warp blocker flag", "Eva Angel Effective",
            "Eva angel killer flag", "Relic effective flag", "Immune to curse flag", "Insanely tough flag", "Insane damage flag", "Savage blow chance", "Savage blow level", "Dodge attack chance",
            "Dodge attack duration", "Surge attack chance", "Surge attack min range", "Surge attack max range", "Surge attack level", "Toxic immunity flag", "Surge immunity flag", "Curse chance", "Curse duration", "Unkown", "Aku shield break chance",
            "Aku effective flag"
        };
    }
}
