using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class NP
    {
        public static void np(string path)
        {
            int np = GetNP(path);
            np = Editor.AskSentances(np, "NP", false, 10000);
            SetNP(path, np);
            Editor.AskSentances(np, "NP", true);
        }
        public static int GetNP(string path)
        {
            byte[] conditions = { 0x80, 0x38 };
            // Search for np position
            int pos = Editor.Search(path, conditions)[0] + - 5;
            if (pos == 0)
            {
                Editor.Error();
            }
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetNP(string path, int np)
        {
            byte[] conditions = { 0x80, 0x38 };
            // Search for np position
            int pos = Editor.Search(path, conditions)[0] + - 5;
            Editor.SetItemData(path, new int[] { np }, 4, pos);
        }
    }
}
