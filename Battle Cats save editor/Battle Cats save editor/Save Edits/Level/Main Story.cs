using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class MainStory
    {
        public static void Stage(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Editor.ColouredText("What chapters do you want to complete?(1-9)\n1.&Empire of cats chapter 1&\n2.&Empire of cats chapter 2&\n3.&Empire of cats chapter 3&\n4.&Into the future chapter 1&\n5.&Into the future chapter 2&\n6.&Into the future chapter 3&\n7.&Cats of the cosmos chapter 1&" +
                "\n8.&Cats of the cosmos chapter 2&\n9.&Cats of the cosmos chapter 3&\n10.&All chapters&\n", ConsoleColor.White, ConsoleColor.Cyan);
            int choice = (int)Editor.Inputed();
            // Starting position of stage cleared flags
            int startPos = 946;
            // Length of each chapter's stage cleared flags, 16 0x00 bytes separate each chapter
            int blockLen = (47 * 4) + 16;
            // Position of total number of stages cleared
            int lvlCountPos = 906;
            // All chapters
            if (choice == 10)
            {
                // Set stages to be cleared
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 48; i++)
                    {
                        stream.Position = startPos + (i * 4);
                        stream.WriteByte(1);
                    }
                    startPos += blockLen;
                }
                // Set total number of stages cleared
                for (int i = 0; i < 10; i++)
                {
                    stream.Position = lvlCountPos + (i * 4);
                    stream.WriteByte(48);
                }
            }
            // Specific chapter
            else if (choice < 10)
            {
                if (choice > 3)
                {
                    choice++;
                }
                // Set start point to correct chapter
                startPos += (choice - 1) * blockLen;
                // Set stages to be cleared
                for (int i = 0; i < 48; i++)
                {
                    stream.Position = startPos + (i * 4);
                    stream.WriteByte(1);
                }
                // Set total number of stages cleared
                stream.Position = lvlCountPos + ((choice - 1) * 4);
                stream.WriteByte(48);
            }
            else
            {
                Console.WriteLine("Please enter a recognised number");
                stream.Close();
                Stage(path);
            }

        }
    }
}
