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
            int catTickets = GetCatTicket(path);
            catTickets = Editor.AskSentances(catTickets, "normal cat tickets", false, 1999);
            SetCatTicket(path, catTickets);
            Editor.AskSentances(catTickets, "normal cat tickets", true);
        }
        public static int GetCatTicket(string path)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[3] - 8;
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetCatTicket(string path, int catticket)
        {
            int pos = Editor.GetCatRelatedHackPositions(path)[3] - 8;
            Editor.SetItemData(path, new int[] { catticket }, 4, pos);
        }
    }
}
