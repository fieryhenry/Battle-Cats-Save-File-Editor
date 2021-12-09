using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class StageMod
    {
        public static void Stagecsv()
        {
            OpenFileDialog fd = new()
            {
                Filter = "files (stage*.csv)|stage*.csv",
                Title = "Select a stage*.csv file"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .csv files");
                Editor.Options();
            }
            string path = fd.FileName;
            string[] csvData = File.ReadAllLines(path);

            string stageID = string.Join("", csvData[0].Split(','));
            int hasID = 2;
            int hasID2 = 1;
            Console.WriteLine(csvData[0].Split(',').Length);
            // Check if stage csv contains a stage id
            if (csvData[0].Split(',').Length > 7)
            {
                hasID = 1;
                hasID2 = 0;
                stageID = "None";
            }
            int index = stageID.IndexOf('/');

            // Remove comments from file
            string stageIDTrim = stageID;
            try
            {
                stageIDTrim = stageID.Remove(index);
            }
            catch
            {

            }
            // Store main data about the stage, e.g base health, stage width, max enemies
            string[] baseData = csvData[hasID2].Split(',');

            string[] BaseStrings =
            {
                "Stage Width", "Base health", "Minimum spawn frame", "Maximum spawn frame", "Background type", "Maximum enemies",
            };
            string BaseCol = "";
            for (int i = 0; i < BaseStrings.Length; i++)
            {
                BaseCol += $"&{BaseStrings[i]}:& {baseData[i]}\n";
            }
            string[] EnemyData = new string[csvData.Length - hasID];

            string[] EnemyStrings =
            {
                "Enemy ID", "Amount to spawn in total", "First spawn frame", "Time between spawns in frames min",
                "Time between spawns in frames max", "Spawn when base health has reached %", "Front z-layer", "Back z-layer", "Boss flag",
                "Strength multiplier"
            };
            int fail = 0;
            List<List<string>> EnemySlotData = new();
            // Loop through enemy slots
            for (int i = 0; i < csvData.Length - hasID; i++)
            {
                // Set enemy data i to enemy slot i
                EnemyData[i] = csvData[i + hasID];
                // Split data into an array
                string[] allData = EnemyData[i].Split(',');
                // Turn data into list
                List<string> LsData = allData.ToList();
                // Check if this is the end of the enemy slots
                if (LsData.Count < 5)
                {
                    fail = i;
                    break;
                }
                // Add enemy slot data to list
                EnemySlotData.Add(LsData);
            }

            Editor.ColouredText($"Stage ID:&{stageIDTrim}\n{BaseCol}", ConsoleColor.White, ConsoleColor.DarkYellow);
            for (int i = 0; i < EnemySlotData.Count; i++)
            {
                Editor.ColouredText($"\n&Enemy Slot &{i + 1}&:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                for (int j = 0; j < EnemySlotData[i].Count; j++)
                {
                    if (EnemySlotData[i][0] == "0")
                    {
                        Editor.ColouredText("Empty\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                        break;
                    }
                    if (j == 1 && EnemySlotData[i][j] == "0")
                    {
                        EnemySlotData[i][j] = "unlimited";
                    }
                    try
                    {
                        Editor.ColouredText($"&{EnemyStrings[j]}:&{EnemySlotData[i][j]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    }
                    catch
                    {

                    }
                }
            }
            Editor.ColouredText("&What do you want to edit?(1 &stage data&, 2 &enemy spawning data&):\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int answer = (int)Editor.Inputed();
            string complete = "";
            // Stage data
            if (answer == 1)
            {
                Console.WriteLine("What do you want to edit?(you can enter multiple ids separated by spaces to edit multiple at once):");
                for (int i = 0; i < BaseStrings.Length; i++)
                {
                    Editor.ColouredText($"&{i + 1}. &{BaseStrings[i]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                string[] response = Console.ReadLine().Split(' ');
                for (int j = 0; j < response.Length; j++)
                {
                    int id = int.Parse(response[j]);
                    Editor.ColouredText($"&What do you want to set &{BaseStrings[id - 1]}& to?:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    string val = Console.ReadLine();
                    baseData[id - 1] = val;
                }
                for (int i = 0; i < baseData.Length; i++)
                {
                    if (i == baseData.Length - 1 && hasID == 1)
                    {
                        // If it's the final item, don't add a comma
                        complete += $"{baseData[i]}";
                    }
                    else
                    {
                        complete += $"{baseData[i]},";
                    }
                }
                // Set base data to edited base data
                csvData[hasID2] = complete;
            }
            // Enemy data
            else if (answer == 2)
            {
                Console.WriteLine("What enemy slot do you want to edit?(you can enter multiple slots separated by spaces to edit multiple at once):");
                for (int i = 0; i < EnemySlotData.Count; i++)
                {
                    Editor.ColouredText($"{i + 1}. &Enemy id:& {EnemySlotData[i][0]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                string[] response = Console.ReadLine().Split(' ');
                for (int i = 0; i < response.Length; i++)
                {
                    int slot = int.Parse(response[i]);
                    Editor.ColouredText($"&What do you want to edit in slot &{slot}&?(you can enter multiple slots separated by spaces to edit multiple at once):\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    for (int j = 0; j < EnemyStrings.Length; j++)
                    {
                        Editor.ColouredText($"&{j + 1}.& {EnemyStrings[j]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    }
                    string[] response2 = Console.ReadLine().Split(' ');
                    for (int j = 0; j < response2.Length; j++)
                    {
                        int toEdit = int.Parse(response2[j]);
                        Editor.ColouredText($"&What do you want to set &{EnemyStrings[toEdit - 1]}& to?:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                        string val = Console.ReadLine();
                        Console.WriteLine(slot);
                        EnemySlotData[slot - 1][toEdit - 1] = val;
                    }
                }
                for (int i = 0; i < EnemySlotData.Count; i++)
                {
                    for (int j = 0; j < EnemySlotData[i].Count; j++)
                    {
                        if (EnemySlotData[i][j] == "unlimited")
                        {
                            EnemySlotData[i][j] = "0";
                        }
                        if (j == EnemySlotData[i].Count - 1)
                        {
                            // If it's the final item, don't add a comma
                            complete += $"{EnemySlotData[i][j]}";
                        }
                        else
                        {
                            complete += $"{EnemySlotData[i][j]},";
                        }
                    }
                    complete += "\n";
                }
            }
            else
            {
                Console.WriteLine("Please enter either 1 or 2");
                Stagecsv();
            }
            string Final = "";
            // If csv has a stage id
            if (hasID == 2)
            {
                Final += csvData[0] + "\n";
            }
            // If base data was modified
            if (answer == 1)
            {
                // Add edited base data to final string
                Final += complete + "\n";
                // Add enemy data to final string
                for (int i = 0; i < EnemyData.Length; i++)
                {
                    Final += EnemyData[i] + "\n";
                }
            }
            else
            {
                // Add base data to final string
                Final += csvData[hasID2] + "\n";
                // Add enemy data to final string
                Final += complete;
            }
            // Removing trailing newlines from file to make sure file only has 1 newline
            Final = Final.Trim('\n');
            Final += "\n";
            // If no other data exists at the end of the enemy slots, skip the next for loop and add a newline
            if (fail == 0)
            {
                fail = 50;
                Final += "\n";
            }
            // If other data exists at the end of the enemy slots, add it to the final string
            for (int i = fail + hasID; i < csvData.Length - 1; i++)
            {
                Final += csvData[i] + "\n";
            }
            // Write final string to the file
            File.WriteAllText(path, Final);
            Console.WriteLine("\nData: \n" + Final + "\n");

            // Make sure file length is divisible by 16, so encryption works
            List<byte> ls = File.ReadAllBytes(path).ToList();
            int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
            rem *= 16;
            rem -= ls.Count;
            // Add data to end of file so file length is divisible by 16
            for (int i = 0; i < rem && rem != 16; i++)
            {
                ls.Add((byte)rem);
            }
            // Write finished data to file
            File.WriteAllBytes(path, ls.ToArray());
        }

    }
}
