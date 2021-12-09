using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GamatotoHelper
    {
        public static void GamHelp(string path)
        {
            Editor.ColouredText("What helpers do you want?&\n&Type numbers separated by spaces\nThe different helper ids are as follows:&\nIntern &1 - 53&\nLacky &54 - 83&\nUnderling &84 - 108&\nAssistant &109 - 128&\nLegend &129 - 148&\ne.g entering " +
                "&3 69 120 86 110 &would set your helpers to &1& intern, &1& lackey, &2& assistants, &1& underling\nThe ids must be different to eachother, the max helpers you can have is &10\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Split(' ');
            int[] answerInt = new int[answer.Length];
            try
            {
                // Convert ids into ints
                answerInt = Array.ConvertAll(answer, s => int.Parse(s));
            }
            catch (Exception e)
            {
                Editor.ColouredText(e.Message + "\n", ConsoleColor.White, ConsoleColor.Red);
                GamHelp(path);
            }
            for (int i = 0; i < answerInt.Length; i++)
            {
                if (answerInt[i] < 1)
                {
                    Editor.ColouredText("Error: you can't have an id below 1\n", ConsoleColor.White, ConsoleColor.Red);
                    GamHelp(path);
                }
                if (answerInt[i] > 148)
                {
                    Editor.ColouredText("Error: you can't have an id above 148\n", ConsoleColor.White, ConsoleColor.Red);
                    GamHelp(path);
                }
            }
            // Turn ids into byte array
            byte[] bytes = answerInt.SelectMany(BitConverter.GetBytes).ToArray();

            int pos = Editor.ThirtySix(path)[0];
            bool found = false;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (pos > 0)
            {
                found = true;
            }
            stream.Position = pos - 1025;
            stream.Write(bytes, 0, bytes.Length);
            if (found)
            {
                // Format string to output what was edited
                Console.WriteLine("Success");
                int count = 0;
                if (answerInt.Length < 5)
                {
                    count = 5;
                }
                else
                {
                    count = answerInt.Length;
                }
                int[] helpNums = new int[count];
                for (int i = 0; i < answerInt.Length; i++)
                {
                    if (answerInt[i] <= 53) helpNums[0]++;
                    else if (answerInt[i] <= 83) helpNums[1]++;
                    else if (answerInt[i] <= 108) helpNums[2]++;
                    else if (answerInt[i] <= 128) helpNums[3]++;
                    else helpNums[4]++;
                }
                Console.WriteLine("\nSet helpers to:\n {0} intern(s)\n {1} lackey(s)\n {2} underling(s)\n {3} assistant(s)\n {4} legend(s)", helpNums[0], helpNums[1], helpNums[2], helpNums[3], helpNums[4]);
            }
            if (!found) Console.WriteLine("Sorry your gamatoto helper position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
        }
    }
}
