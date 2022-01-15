using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class AllTalent
    {
        public static void Talent(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Talent upgrade all cats",
                "Talent upgrade specific cats"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            Editor.ColouredText(toOutput);
            switch ((int)Editor.Inputed())
            {
                case 0:
                    Editor.Options();
                    break;
                case 1:
                    AllTalent.AllTalents(path);
                    break;
                case 2:
                    SpecificTalent.SpecificTalents(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }
        public static void AllTalents(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int pos = 0;
            int len = 0;

            // Search for talent position
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
                Editor.Error();
                return;
            }
            // Get all types of skills and their maxes
            Dictionary<int, Tuple<string, int>> data = Editor.GetSkillData();

            // Loop through talent data
            for (int i = pos + 4; i < pos + len * 48; i += 1)
            {
                // Cat id
                byte[] idData = { allData[i], allData[i + 1] };
                byte[] idB = Editor.Endian(BitConverter.ToInt16(idData, 0));
                int id = BitConverter.ToInt16(idB, 0);

                // Number of skills
                int len2 = allData[i + 4];
                // Loop through each skill and set it to its max value
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
                        try
                        {
                            stream.WriteByte((byte)data[skillID].Item2);
                        }
                        catch
                        {

                        }
                    }
                }
                // Move to the next cat
                i += (8 * len2) + 7;
            }
        }
    }
}
