using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class EquipSlots
    {
        public static void Slots(string path)
        {
            Console.WriteLine("How many slots do you want to have unlocked?(max 15):");
            int slots = (int)Editor.Inputed();
            if (slots > 15) slots = 15;
            else if (slots < 0) slots = 0;

            byte[] conditions = { 0x2c, 0x01, 0x00, 0x00 };
            // Search for slot position
            int pos = Editor.Search(path, conditions, false, 0)[1];

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            stream.Position = pos - 5;
            stream.WriteByte((byte)slots);
            Console.WriteLine("Set unlocked slot amount to " + slots);
        }
    }
}
