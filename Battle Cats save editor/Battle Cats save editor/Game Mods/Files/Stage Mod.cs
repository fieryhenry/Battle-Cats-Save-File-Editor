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
        public static List<int> AddEnemyIDs(List<List<int>> EnemySlots)
        {
            List<int> enemyIDs = new();
            for (int i = 0; i < EnemySlots.Count; i++)
            {
                if (EnemySlots[i][0] != 0)
                {
                    enemyIDs.Add(EnemySlots[i][0]);
                }
            }
            return enemyIDs;
        }
        public static void Stagecsv()
        {
            List<string> editOptionsS = new()
            {
                "Basic stage properties",
                "Enemy Slots",
                "Castle ID"
            };
            List<string> stageInfoS = new()
            {
                "Stage Width",
                "Base health",
                "Minimum spawn frame",
                "Maximum spawn frame",
                "Background type",
                "Maximum enemies",
            };
            List<string> enemyInfoS = new()
            {
                "Enemy ID",
                "Amount to spawn in total",
                "First spawn frame",
                "Time between spawns in frames min",
                "Time between spawns in frames max",
                "Spawn when base health has reached %",
                "Front z-layer",
                "Back z-layer",
                "Boss flag",
                "Strength multiplier",
            };

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

            List<List<int>> AllstageData = FileHandler.ReadCSV(path);
            List<int> StageID = new();

            int startPos = 0;
            if (AllstageData[0].Count < 9)
            {
                startPos = 1;
                StageID = AllstageData[0];
            }
            List<int> StageInfo = AllstageData[startPos];
            List<List<int>> EnemyInfo = AllstageData.GetRange(startPos + 1, AllstageData.Count - (startPos + 1));
            bool SuccessStageID = int.TryParse(string.Join("", StageID), out _);
            if (EnemyInfo[0].Count == 9)
            {
                try
                {
                    enemyInfoS.RemoveAt(9);
                }
                catch
                {

                }
            }
            List<int> enemyIDs = AddEnemyIDs(EnemyInfo);
            if (!SuccessStageID)
            {
                editOptionsS.RemoveAt(2);
            } 

            Editor.ColouredText($"&What do you want to edit?:\n&{Editor.CreateOptionsList<string>(editOptionsS.ToArray())}");
            int answer = (int)Editor.Inputed();
            if (answer > 2 + Convert.ToInt32(SuccessStageID) || answer < 0)
            {
                Console.WriteLine("Please enter a recognised number");
                Stagecsv();
            }
            if (answer == 1)
            {
                Editor.ColouredText($"&What do you want to edit?:\n&{Editor.CreateOptionsList(stageInfoS.ToArray(), StageInfo.ToArray())}");
                string[] input = Console.ReadLine().Split(' ');
                foreach (string toEditS in input)
                {
                    int toEdit = int.Parse(toEditS);
                    Editor.ColouredText($"&What do you want to set &{stageInfoS[toEdit - 1]}& to?:\n");
                    int val = (int)Editor.Inputed();
                    StageInfo[toEdit - 1] = val;
                }
            }
            else if (answer == 2)
            {
                List<string> slots = new();
                foreach (int enemy_id in enemyIDs)
                {
                    slots.Add($"Enemy id: &{enemy_id}&");
                }
                Editor.ColouredText($"&What slot do you want to edit?(To add new slots, just enter a number greater than {enemyIDs.Count}(You can edit multiple slots at once by entering multiple ids sperated by spaces:\n&{Editor.CreateOptionsList<int>(slots.ToArray())}");
                string[] input = Console.ReadLine().Split(' ');
                foreach (string slotNumS in input)
                {
                    enemyIDs = AddEnemyIDs(EnemyInfo);
                    int slotNum = int.Parse(slotNumS);
                    if (slotNum > enemyIDs.Count +1)
                    {
                        slotNum = enemyIDs.Count +1;
                    }
                    if (slotNum > EnemyInfo.Count)
                    {
                        List<int> toAddExtra = Enumerable.Repeat(0, EnemyInfo[slotNum - 2].Count).ToList();
                        toAddExtra[7] = 9;
                        EnemyInfo.Add(toAddExtra);
                        enemyIDs.Add(0);
                    }
                    Editor.ColouredText($"&What do you want to edit?:(You can edit multiple values by entering multiple numbers separated by spaces\n&{Editor.CreateOptionsList<int>(enemyInfoS.ToArray(), EnemyInfo[slotNum -1].ToArray())}");
                    input = Console.ReadLine().Split(' ');
                    foreach (string attribute in input)
                    {
                        int id = int.Parse(attribute);
                        if (id == 2)
                        {
                            Editor.ColouredText($"&What do you want to set &{enemyInfoS[id - 1]}& to?(&0& = &unlimited& spawn amount):\n");
                        }
                        else
                        {
                            Editor.ColouredText($"&What do you want to set &{enemyInfoS[id - 1]}& to?:\n");
                        }
                        int val = (int)Editor.Inputed();
                        EnemyInfo[slotNum -1][id -1] = val;
                    }
                }
                
            }
            else if (answer == 3)
            {
                Editor.ColouredText($"&Castle ID : &{string.Join("", StageID)}&\nWhat do you want to set the castle id to?:\n");
                StageID = Editor.ConvertCharArrayToIntArray(Console.ReadLine().ToArray()).ToList();              
            }
            List<List<int>> finalData = new();
            if (SuccessStageID)
            {
                finalData.Add(StageID);
            }
            finalData.Add(StageInfo);
            finalData.AddRange(EnemyInfo);
            FileHandler.WriteCSV(finalData, path, true);
        }
    }
}