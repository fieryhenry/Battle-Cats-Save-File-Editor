using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class Catseye
    {
        public static void Catseyes(string path)
        {
            Console.WriteLine("How many catseyes of each type do you want(max 65535)");
            int platCatTickets = (int)Editor.Inputed();

            byte[] bytes = Editor.Endian(platCatTickets);

            int[] occurrence = Editor.OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            int pos = occurrence[8] + (Editor.catAmount * 4) + 8;
            if (pos > 0)
            {
                found = true;
                stream.Position = pos;
            }
            for (int i = 0; i < 5; i++)
            {
                stream.Write(bytes, 0, 4);
            }
            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your catseye position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");

        }
    }
}
