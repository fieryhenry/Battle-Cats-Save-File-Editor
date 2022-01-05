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
        public static void RareCatTickets(string path)
        {
            int rareCatTickets = GetRareCatTicket(path);
            rareCatTickets = Editor.AskSentances(rareCatTickets, "rare cat tickets", false, 299);
            SetRareCatTicket(path, rareCatTickets);
            Editor.AskSentances(rareCatTickets, "rare cat tickets", true);
        }
        public static int GetRareCatTicket(string path)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[3] - 4;
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetRareCatTicket(string path, int rareCatTicket)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[3] - 4;
            Editor.SetItemData(path, new int[] { rareCatTicket }, 4, pos);
        }
    }
}
