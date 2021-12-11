using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class TalentOrbs
    {
        public static void TalentOrb(string path)
        {
            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream2.Length;
            byte[] allData = new byte[length];
            stream2.Read(allData, 0, length);

            stream2.Close();

            // Talent orb data is between game version 9.5 and 9.7
            // Search for game version 9.5
            byte[] conditions = { 0x84, 0x61, 0x01 };
            int startPos = Editor.Search(path, conditions, true, allData.Length - 16)[0] - 2;

            // Search for game version 9.7
            byte[] conditions2 = { 0x4c, 0x62, 0x01 };
            int endPos = Editor.Search(path, conditions2, true, allData.Length - 16)[0];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            // Number of orb slots in your data
            int orbCountTypes = allData[startPos + 4] * 3;

            // 155 total different orb types
            int totalNumOrbTypes = 155;
            int[] orbs = new int[totalNumOrbTypes];

            // Loop through your orbs and store what you have
            for (int i = 0; i < orbCountTypes; i += 3)
            {
                int orbID = allData[startPos + i + 9] - 1;
                orbs[orbID] = allData[startPos + i + 8];
            }

            string[] orbList = { "Red D attack", "Red C attack", "Red B attack", "Red A attack", "Red S attack", "Red D defense", "Red C defense", "Red B defense", "Red A defense", "Red S defense", "Floating D attack", "Floating C attack", "Floating B attack", "Floating A attack", "Floating S attack", "Floating D defense", "Floating C defense", "Floating B defense", "Floating A defense", "Floating S defense", "Black D attack", "Black C attack", "Black B attack", "Black A attack", "Black S attack", "Black D defense", "Black C defense", "Black B defense", "Black A defense", "Black S defense", "Metal D defense", "Metal C defense", "Metal B defense", "Metal A defense", "Metal S defense", "Angel D attack", "Angel C attack", "Angel B attack", "Angel A attack", "Angel S attack", "Angel D defense", "Angel C defense", "Angel B defense", "Angel A defense", "Angel S defense", "Alien D attack", "Alien C attack", "Alien B attack", "Alien A attack", "Alien S attack", "Alien D defense", "Alien C defense", "Alien B defense", "Alien A defense", "Alien S defense", "Zombie D attack", "Zombie C attack", "Zombie B attack", "Zombie A attack", "Zombie S attack", "Zombie D defense", "Zombie C defense", "Zombie B defense", "Zombie A defense", "Zombie S defense" };
            string[] orbTargets = { "Red", "Floating", "Black", "Metal", "Angel", "Alien", "Zombie" };
            string[] orbGrades = { "D", "C", "B", "A", "S" };
            string[] orbTypes = { "Strong", "Massive", "Tough", };

            List<string> orbS = new();

            orbS.AddRange(orbList);
            int length2 = orbS.Count;
            // Create other orb types for Strong, Massive, and Tough
            for (int i = 0; i < orbTargets.Length; i++)
            {
                // Metal only has defense up orbs
                if (orbTargets[i] != "Metal")
                {
                    for (int k = 0; k < orbTypes.Length; k++)
                    {
                        for (int l = 0; l < orbGrades.Length; l++)
                        {
                            orbS.Add(orbTargets[i] + " " + orbGrades[l] + " " + orbTypes[k]);
                        }
                    }
                }
            }
            Console.WriteLine("You have:");
            string toOutput = "";
            // Format string to output your current orbs
            for (int i = 0; i < orbs.Length; i++)
            {
                if (orbs[i] == 1)
                {
                    toOutput += "&" + orbs[i] + "& " + orbS[i] + " &orb\n&";
                }
                else if (orbs[i] > 1)
                {
                    toOutput += "&" + orbs[i] + "& " + orbS[i] + " &orbs\n&";
                }
            }
            Editor.ColouredText(toOutput);
            List<byte> allDataList = new(allData);

            // Remove your orb data from save file
            allDataList.RemoveRange(startPos + 8, orbCountTypes);

            Editor.ColouredText("\n&What orbs do you want?(Enter the full name, in format - {&type&} {&letter&} {&attack&/&defense&/&strong&/&massive&/&tough&}, e.g &red d attack&, or &floating s defense&, note that for metal, &only defense up orbs exist&\nIf you want to edit multiple, enter 1 full" +
                " orb name and then another orb name, separated by and underscore, e.g, &red s strong&_&alien c tough&\nYou can also enter orb ids instead if you want to. You can enter &clear& if you want to remove all of your talent orbs\n");
            string input = Console.ReadLine();
            string[] orbNames = input.Split('_');

            List<int> ids = new();
            List<int> amounts = new();

            bool skip = false;
            // Clear orb storage
            if (orbNames[0].ToLower() == "clear")
            {
                skip = true;
                for (int i = 0; i < orbs.Length; i++)
                {
                    orbs[i] = 0;
                }
                Editor.ColouredText("Cleared all talent orbs from storage\n", ConsoleColor.White, ConsoleColor.Red);
            }
            // Orb data to insert into save file
            byte[] insert = new byte[totalNumOrbTypes * 3];
            // Loop through all orb types
            for (int i = 0; i < totalNumOrbTypes; i++)
            {
                // Set orb id
                insert[(i * 3) + 1] = (byte)(i + 1);
                // Set orb data to previous amounts
                insert[i * 3] = (byte)orbs[i];
            }
            // Loop through orbs entered by user
            for (int i = 0; i < orbNames.Length && !skip; i++)
            {
                // If the user entered an orb id instead of name
                try
                {
                    int id = int.Parse(orbNames[i]);
                    if (id > orbS.Count + 1)
                    {
                        Console.WriteLine("orb id is too large");
                    }
                    else
                    {
                        ids.Add(id);
                    }
                }
                // If the user entered name instead of an orb id
                catch
                {
                    // Humiliate user if they spelt angel wrong
                    if (orbNames[i].ToLower().Contains("angle"))
                    {
                        Editor.ColouredText("You put angle instead of angel, I assume you mean angel so it has been corrected\n\n", ConsoleColor.White, ConsoleColor.Red);
                        orbNames[i] = orbNames[i].ToLower();
                        orbNames[i] = orbNames[i].Replace("angle", "angel");
                    }
                    // Check if entered orb exists
                    if (!orbS.Exists(orb => orb.ToLower() == orbNames[i].ToLower()))
                    {
                        Console.WriteLine("Orb: " + orbNames[i] + " doesn't exist!");
                    }
                    else
                    {
                        // Add the id of the orb to a list
                        ids.Add(orbS.FindIndex(orb => orb.ToLower() == orbNames[i].ToLower()));
                    }
                }
            }
            // Loop through user entered orb ids
            for (int i = 0; i < ids.Count; i++)
            {
                Editor.ColouredText("&What amount of &" + orbS[ids[i]] + "& Orbs do you want to set?(max 255 per orb): ");
                amounts.Add((int)Editor.Inputed());
                // Set orb amount
                insert[ids[i] * 3] = (byte)amounts[i];
            }
            // Set total number of orb types in save file
            allDataList[startPos + 4] = (byte)totalNumOrbTypes;
            // Insert modified orb data into save file
            allDataList.InsertRange(startPos + 8, insert);

            stream.Close();
            // Write data to save file
            File.WriteAllBytes(path, allDataList.ToArray());
        }
    }
}
