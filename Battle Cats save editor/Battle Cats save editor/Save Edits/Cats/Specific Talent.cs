using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class SpecificTalent
    {
        public static void SpecificTalents(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int pos = 0;
            int len = 0;

            // Search for talent data position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x37 && allData[i - 1] == 0x00 && allData[i + 1] == 0x00 && allData[i + 225] == 0x4d && allData[i + 226] == 0x00)
                {
                    pos = i + 319;
                    len = allData[pos];
                    break;

                }
            }
            if (pos == 0)
            {
                Console.WriteLine("Error, your talent position couldn't be found, please report this to me on discord");
                return;
            }
            // Get all types of skills and their maxes
            Dictionary<int, Tuple<string, int>> data = Editor.GetSkillData();

            Console.WriteLine("Enter cat id(you can enter multiple ids separated by spaces to edit multiple cats at once):");
            string[] catIDsAnswer = Console.ReadLine().Split(' ');
            // Loop through each entered id
            for (int l = 0; l < catIDsAnswer.Length; l++)
            {
                int catID = int.Parse(catIDsAnswer[l]);
                bool found = false;
                // Loop through all talent data
                for (int i = pos + 4; i < pos + len * 48; i += 1)
                {
                    // Cat id
                    byte[] idData = { allData[i], allData[i + 1] };
                    byte[] idB = Editor.Endian(BitConverter.ToInt16(idData, 0));
                    int id = BitConverter.ToInt32(idB, 0);

                    // Number of skills
                    int len2 = allData[i + 4];
                    if (id == catID)
                    {
                        found = true;
                        Console.WriteLine("Do you want to max out the talent level for this cat,(1), or do you want to edit each skill individually(2)?:");
                        string choice = Console.ReadLine();
                        // Max
                        if (choice == "1")
                        {
                            // Loop through this cat's talent data and set it to max
                            for (int j = 1; j <= len2; j++)
                            {
                                int skillID = allData[i + (8 * j)];
                                int value = allData[i + 4 + (8 * j)];
                                stream.Position = i + 4 + (8 * j);
                                if (value > 10 || id > Editor.catAmount)
                                {
                                    i = pos + len * 48;
                                    break;
                                }
                                // If cameraman cat critical, max value is 1
                                if (id == 149 && skillID == 13)
                                {
                                    stream.WriteByte(1);
                                }
                                // If Catasaurus critical, max value is 5 
                                else if (id == 46 && skillID == 13)
                                {
                                    stream.WriteByte(5);
                                }
                                else
                                {
                                    stream.WriteByte((byte)data[skillID].Item2);
                                }

                            }
                        }
                        else if (choice == "2")
                        {
                            Dictionary<int, int> CatData = new();
                            // Loop through cat data and store what skills it has
                            for (int j = 1; j <= len2; j++)
                            {
                                int skillID = allData[i + (8 * j)];
                                CatData.Add(skillID, i + (8 * j));
                            }
                            int index = 1;
                            string[] skillsDesc = new string[5];
                            string[] skillsIDs = new string[5];
                            string[] skillsMax = new string[5];
                            // Find out the name, id and max of each of its skills
                            foreach (KeyValuePair<int, int> catSkillData in CatData)
                            {
                                int id2 = index;
                                string desc = data[catSkillData.Key].Item1;
                                skillsDesc[index - 1] = desc;
                                skillsIDs[index - 1] = catSkillData.Key.ToString();
                                skillsMax[index - 1] = data[catSkillData.Key].Item2.ToString();
                                Editor.ColouredText($"&{id2}.& {desc}\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                                index++;
                            }
                            Console.WriteLine("Enter skill id, you can enter multiple values separated by spaces:");
                            string[] ids = Console.ReadLine().Split(' ');
                            // Loop through entered ids
                            for (int k = 0; k < ids.Length; k++)
                            {
                                int idInt = int.Parse(ids[k]);
                                int realID = int.Parse(skillsIDs[idInt - 1]);
                                bool stop = false;
                                if (idInt > 5)
                                {
                                    Console.WriteLine("Error, skill id is too large");
                                    stop = true;
                                }
                                else if (idInt < 1)
                                {
                                    Console.WriteLine("Error, skill id is too small");
                                    stop = true;
                                }
                                if (!stop)
                                {
                                    int value = 0;
                                    // If cameraman cat critical, max value is 1
                                    if (id == 149 && realID == 13)
                                    {
                                        Editor.ColouredText($"&What do you want to set &{skillsDesc[idInt - 1]}& to? (max:1):", ConsoleColor.White, ConsoleColor.DarkYellow);
                                        value = (int)Editor.Inputed();
                                        if (value > 1) value = 1;
                                        else if (value < 0) value = 0;
                                    }
                                    // If Catasaurus critical, max value is 5 
                                    else if (id == 46 && realID == 13)
                                    {
                                        Editor.ColouredText($"&What do you want to set &{skillsDesc[idInt - 1]}& to? (max:5):", ConsoleColor.White, ConsoleColor.DarkYellow);
                                        value = (int)Editor.Inputed();
                                        if (value > 5) value = 5;
                                        else if (value < 0) value = 0;
                                    }
                                    else
                                    {
                                        Editor.ColouredText($"&What do you want to set &{skillsDesc[idInt - 1]}& to? (max:{skillsMax[idInt - 1]}):", ConsoleColor.White, ConsoleColor.DarkYellow);
                                        value = (int)Editor.Inputed();
                                        if (value > int.Parse(skillsMax[idInt - 1])) value = int.Parse(skillsMax[idInt - 1]);
                                        else if (value < 0) value = 0;
                                    }
                                    stream.Position = CatData[realID] + 4;
                                    stream.WriteByte((byte)value);
                                }
                            }

                        }
                    }
                    // Go to next cat
                    i += (8 * len2) + 7;
                }
                if (!found)
                {
                    Console.WriteLine("Error, this cat doesn't exist, or have any talents");
                }
            }
        }
    }
}
