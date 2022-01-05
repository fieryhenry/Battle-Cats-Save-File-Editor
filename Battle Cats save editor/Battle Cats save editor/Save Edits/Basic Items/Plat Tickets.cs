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
            int platinumTickets = GetPlatinumTickets(path);
            platinumTickets = Editor.AskSentances(platinumTickets, "platinum tickets", false, 9);
            SetPlatinumTickets(path, platinumTickets);
            Editor.AskSentances(platinumTickets, "platinum tickets", true);
        }
        public static int GetPlatinumTickets(string path)
        {
            int pos = Editor.GetPlatinumTicketPos(path)[1] + 8;
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetPlatinumTickets(string path, int platinumTickets)
        {
            int pos = Editor.GetPlatinumTicketPos(path)[1] + 8;
            Editor.SetItemData(path, new int[] { platinumTickets }, 4, pos);
        }
    }
}
