using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class LegendTickets
    {
        public static void LegendTicket(string path)
        {
            using var stream1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);

            stream1.Close();

            // Search for legend ticket position
            byte[] condtions2 = { 0x00, 0x78, 0x63, 0x01, 0x00 };
            int pos = Editor.Search(path, condtions2, false, allData.Length - 800)[0];

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (pos <= 0)
            {
                Console.WriteLine("Error, your legend ticket position couldn't be found, please report this on discord");
                return;
            }
            byte[] ticketB = new byte[4];
            stream.Position = pos + 5;
            stream.Read(ticketB, 0, 4);

            int tickets = BitConverter.ToInt16(ticketB, 0);
            Console.WriteLine($"You have {tickets} legend tickets");
            Console.WriteLine("How many legend tickets do you want? (max 4)");
            tickets = (int)Editor.Inputed();
            if (tickets > 4) tickets = 4;
            else if (tickets < 0) tickets = 0;
            byte[] bytes = Editor.Endian(tickets);

            stream.Position = pos + 5;
            stream.Write(bytes, 0, 4);
            Console.WriteLine($"Set legend tickets to {tickets}");
        }
    }
}
