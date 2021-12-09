using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class PlatTickets
    {
        public static void PlatinumTickets(string path)
        {
            Console.WriteLine("How many Platinum Cat Tickets do you want(max 9)");
            int platCatTickets = (int)Editor.Inputed();
            if (platCatTickets > 9) platCatTickets = 9;
            int pos = Editor.ThirtySix(path)[1];
            byte[] bytes = Editor.Endian(platCatTickets);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            if (pos > 0)
            {
                found = true;
            }
            stream.Position = pos + 8;
            stream.Write(bytes, 0, 4);
            if (found) Console.WriteLine("Success");
            if (!found) Console.WriteLine("Sorry your platinum cat ticket position couldn't be found, please report this on discord\nThank you");
        }
    }
}
