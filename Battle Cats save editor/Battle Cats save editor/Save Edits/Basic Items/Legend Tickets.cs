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
            int legendTickets = GetLegendTicket(path);
            legendTickets = Editor.AskSentances(legendTickets, "legend tickets", false, 4);

            SetLegendTicket(path, legendTickets);
            Editor.AskSentances(legendTickets, "legend tickets", true);
        }
        public static int GetLegendTicketPos(string path)
        {
            byte[] allData = File.ReadAllBytes(path);

            // Search for legend ticket position
            byte[] condtions2 = { 0x00, 0x78, 0x63, 0x01, 0x00 };
            int pos = Editor.Search(path, condtions2, false, allData.Length - 800)[0] + 5;
            if (pos < 1000)
            {
                Editor.Error();
            }
            return pos;
        }
        public static int GetLegendTicket(string path)
        {
            int pos = GetLegendTicketPos(path);
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetLegendTicket(string path, int legendticket)
        {
            int pos = GetLegendTicketPos(path);
            Editor.SetItemData(path, new int[] { legendticket }, 4, pos);
        }
    }
}
