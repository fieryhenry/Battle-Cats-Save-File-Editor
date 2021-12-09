using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class FixElsewhere
    {
        public static void Elsewhere(string path)
        {
            Console.WriteLine("Please select a working save that doesn't have 'Save is used elsewhere' and has never had it in the past\nPress enter to select that save");
            Console.ReadLine();
            var FD = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "working battle cats save(*.*)|*.*"
            };
            string path2 = "";
            if (FD.ShowDialog() == DialogResult.OK)
            {
                string[] fileToOpen = FD.FileNames;
                path2 = Path.Combine(fileToOpen[0]);
            }
            else
            {
                Editor.ColouredText("\nPlease select your save\n\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                Editor.SelSave();
            }
            byte[] condtions1 = { 0x2d, 0x00, 0x00, 0x00, 0x2e };
            // Search for rough inquiry code position in second save
            int pos1 = Editor.Search(path2, condtions1)[0];

            using var stream1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);

            byte[] codeBytes = new byte[36];
            byte[] codeBytes2 = new byte[9];
            byte[] iqExtra = new byte[11];
            byte[] lastKey = new byte[45];
            int[] found = new int[6];
            stream1.Close();

            // Search for token position in second save
            byte[] condtions2 = { 0x78, 0x63, 0x01};
            int pos2 = Editor.Search(path2, condtions2, false, allData.Length - 800)[0];

            using var stream = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite);

            // Search for inquiry code position from rough position in second save
            for (int j = 1900; j < 2108; j++)
            {
                if (allData[pos1 - j] == 9 && allData[pos1 - j + 1] == 0 && allData[pos1 - j + 2] == 0 && allData[pos1 - j + 3] == 0 && allData[pos1 - j - 1] == 0 && allData[pos1 - j + 23] == 0x2c)
                {
                    found[0] = 1;
                    // Save it in an array
                    Array.Copy(allData, pos1 - j + 4, iqExtra, 0, 11);
                    break;
                }
            }
            // Check for token
            for (int i = pos2 + 9; i < pos2 + 100; i++)
            {
                if (allData[i] >= 48 && allData[i + 1] >= 48 && allData[i + 2] >= 48 && allData[i + 3] >= 48)
                {
                    pos2 = i;
                    found[1] = 1;
                    break;
                }
            }
            // Save token to array
            Array.Copy(allData, pos2, lastKey, 0, 45);

            if (found.Sum() < 2)
            {
                Console.WriteLine("Sorry a position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            // Search for rough inquiry code in first save
            int pos3 = Editor.Search(path, condtions1)[0];
            stream.Close();
            using var stream3 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            length = (int)stream3.Length;
            allData = new byte[length];
            stream3.Read(allData, 0, length);

            stream3.Close();
            // Search for token position in first save
            int pos4 = Editor.Search(path, condtions2, false, allData.Length - 800)[0];

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            // Search for inquiry code position starting from rough position
            for (int j = 1900; j < 2108; j++)
            {
                if (allData[pos3 - j] == 9 && allData[pos3 - j + 1] == 0 && allData[pos3 - j + 2] == 0 && allData[pos3 - j + 3] == 0 && allData[pos3 - j - 1] == 0 && allData[pos3 - j + 23] == 0x2c)
                {
                    found[3] = 1;
                    stream2.Position = pos3 - j + 4;
                    // Set inquiry code in first save to inquiry code in second save
                    stream2.Write(iqExtra, 0, 11);
                    break;
                }
            }
            for (int i = pos4 + 9; i < pos4 + 100; i++)
            {
                if (allData[i] >= 48 && allData[i + 1] >= 48 && allData[i + 2] >= 48 && allData[i + 3] >= 48)
                {
                    pos4 = i;
                    break;
                }
            }
            stream2.Position = pos4;
            // Set token in first save to token in second save
            stream2.Write(lastKey, 0, 45);
            found[4] = 1;

            if (found.Sum() < 4)
            {
                Console.WriteLine("Sorry a position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            else
            {
                Console.WriteLine("Success");
            }
        }
    }
}
