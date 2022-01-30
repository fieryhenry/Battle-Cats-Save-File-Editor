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
        public static void Tickets(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Normal Tickets",
                "Rare Tickets",
                "Platinum Tickets",
                "Legend Tickets",
                "Platinum Shards (using this instead of platinum tickets reduces the chance of a ban)"
            };
            string toOutput = "Warning: editing these at all has a risk of getting your save banned\n&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            Editor.ColouredText(toOutput);
            switch ((int)Editor.Inputed())
            {
                case 0:
                    Editor.Options();
                    break;
                case 1:
                    CatTicket(path);
                    break;
                case 2:
                    RareTickets.RareCatTickets(path);
                    break;
                case 3:
                    PlatTickets.PlatinumTickets(path);
                    break;
                case 4:
                    LegendTickets.LegendTicket(path);
                    break;
                case 5:
                    PlatinumShards.PlatShards(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }
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
