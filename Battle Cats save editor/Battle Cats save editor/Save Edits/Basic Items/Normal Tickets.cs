using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class NormalTickets
    {
        public static void CatTicket(string path)
        {
            Console.WriteLine("How many Cat Tickets do you want(max 1999)");
            int catTickets = (int)Editor.Inputed();
            if (catTickets > 1999) catTickets = 1999;
            else if (catTickets < 0) catTickets = 0;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");
            byte[] bytes = Editor.Endian(catTickets);

            stream.Close();

            int[] occurrence = Editor.OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            stream2.Position = occurrence[3] - 8;

            stream2.Write(bytes, 0, 2);
        }
    }
}
