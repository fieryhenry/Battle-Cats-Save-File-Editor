using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class RareTickets
    {
        public static void CatTicketRare(string path)
        {
            Console.WriteLine("How many Rare Cat Tickets do you want(max 299)");
            int catTickets = (int)Editor.Inputed();
            if (catTickets > 299) catTickets = 299;
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
            stream2.Position = occurrence[3] - 4;
            stream2.Write(bytes, 0, 2);
        }
    }
}
