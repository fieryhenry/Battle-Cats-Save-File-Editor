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
        public static int GetTalentOrbPos(string path)
        {
            byte[] allData = File.ReadAllBytes(path);

            // Search for game version 9.5
            byte[] conditions = { 0x84, 0x61, 0x01 };
            int startPos = Editor.Search(path, conditions, true, allData.Length - 16)[0] - 2;

            return startPos;
        }
        public static int[] GetTalentOrbs(string path)
        {
            int startPos = GetTalentOrbPos(path);

            byte[] allData = File.ReadAllBytes(path);

            int orbCountTypes = allData[startPos + 4] * 3;
            int totalNumOrbTypes = 155;

            int[] orbs = new int[totalNumOrbTypes];

            for (int i = 0; i < orbCountTypes; i += 3)
            {
                int orbID = allData[startPos + i + 9] - 1;
                orbs[orbID] = allData[startPos + i + 8];
            }
            return orbs;
        }
        public static void SetTalentOrbs(string path, int[] orbs)
        {
            int startPos = GetTalentOrbPos(path);
            byte[] allData = File.ReadAllBytes(path);

            List<byte> allDataList = allData.ToList();
            int orbCountTypes = allData[startPos + 4] * 3;

            allDataList.RemoveRange(startPos + 8, orbCountTypes);
            allDataList[startPos + 4] = (byte)orbs.Length;

            List<byte> toInsert = new();

            for (int i = 0; i < orbs.Length; i++)
            {
                byte[] add = {0, (byte)orbs[i], (byte)(i+1)};
                toInsert.AddRange(add);
            }
            allDataList.InsertRange(startPos + 7, toInsert);
            File.WriteAllBytes(path, allDataList.ToArray());
        }
        public static void TalentOrb(string path)
        {
            string[] orbList = { "Red D attack", "Red C attack", "Red B attack", "Red A attack", "Red S attack", "Red D defense", "Red C defense", "Red B defense", "Red A defense", "Red S defense", "Floating D attack", "Floating C attack", "Floating B attack", "Floating A attack", "Floating S attack", "Floating D defense", "Floating C defense", "Floating B defense", "Floating A defense", "Floating S defense", "Black D attack", "Black C attack", "Black B attack", "Black A attack", "Black S attack", "Black D defense", "Black C defense", "Black B defense", "Black A defense", "Black S defense", "Metal D defense", "Metal C defense", "Metal B defense", "Metal A defense", "Metal S defense", "Angel D attack", "Angel C attack", "Angel B attack", "Angel A attack", "Angel S attack", "Angel D defense", "Angel C defense", "Angel B defense", "Angel A defense", "Angel S defense", "Alien D attack", "Alien C attack", "Alien B attack", "Alien A attack", "Alien S attack", "Alien D defense", "Alien C defense", "Alien B defense", "Alien A defense", "Alien S defense", "Zombie D attack", "Zombie C attack", "Zombie B attack", "Zombie A attack", "Zombie S attack", "Zombie D defense", "Zombie C defense", "Zombie B defense", "Zombie A defense", "Zombie S defense" };
            string[] orbTargets = { "Red", "Floating", "Black", "Metal", "Angel", "Alien", "Zombie" };
            string[] orbGrades = { "D", "C", "B", "A", "S" };
            string[] orbTypes = { "Strong", "Massive", "Tough", "Attack", "Defense"};

            List<string> orbS = new();

            orbS.AddRange(orbList);
            int length2 = orbS.Count;
            // Create other orb types for Strong, Massive, and Tough
            for (int i = 0; i < orbTargets.Length; i++)
            {
                // Metal only has defense up orbs
                if (orbTargets[i] != "Metal")
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 5; l++)
                        {
                            orbS.Add(orbTargets[i] + " " + orbGrades[l] + " " + orbTypes[k]);
                        }
                    }
                }
            }
            int[] orbs = GetTalentOrbs(path);
            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(orbS.ToArray(), orbs, true, true)}");
            string separator = "--------------------";
            Editor.ColouredText($"&Enter a name for a talent orb using the items below:{Editor.multipleVals}\n{separator}\n");
            Editor.ColouredText($"{Editor.CreateOptionsList<string>(orbTargets, null, false)}&{separator}\n");
            Editor.ColouredText($"{Editor.CreateOptionsList<string>(orbGrades, null, false)}&{separator}\n");
            Editor.ColouredText($"{Editor.CreateOptionsList<string>(orbTypes, null, false)}&{separator}\n");
            Editor.ColouredText($"&For example: &Floating& s &defense angel& d &attack zombie& b &massive black& a &strong&, you can also enter the ids for the talent orbs\n");
            List<string> answer = Console.ReadLine().Split(' ').ToList();
            List<string> answer_sep = new();
            for (int i = 0; i < answer.Count; i++)
            {
                if (answer[i].All(char.IsDigit))
                {
                    answer_sep.Add(answer[i]);
                    answer.RemoveAt(i);
                    i--;
                }
                else if (i % 3 == 0)
                {
                    string toAdd = $"{answer[i]} {answer[i + 1]} {answer[i + 2]}";
                    answer_sep.Add(toAdd);
                }
            }
            foreach (string talent_orb in answer_sep)
            {
                int result = orbS.FindIndex(x => x.ToLower() == talent_orb.ToLower());
                if (talent_orb.All(char.IsDigit))
                {
                    result = int.Parse(talent_orb) -1;
                }
                else if (result == -1)
                {
                    Console.WriteLine("Error, orb doesn't exist, check your spelling and try again");
                    continue;
                }
                Editor.ColouredText($"&What do you want to set the value for &{orbS[result]}& to?:\n");
                int val = (int)Editor.Inputed();
                orbs[result] = val;
            }
            SetTalentOrbs(path, orbs);
            Editor.ColouredText($"&Successfully set talent orbs to:\n&{Editor.CreateOptionsList(orbS.ToArray(), orbs, true, true)}");

        }
    }
}