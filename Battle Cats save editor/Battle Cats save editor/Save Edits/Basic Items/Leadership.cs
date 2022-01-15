using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class Leadership
    {
        public static void leadership(string path)
        {
            int leadership = GetLeadership(path);
            leadership = Editor.AskSentances(leadership, "leadership", false, 10000);
            SetLeadership(path, leadership);
            Editor.AskSentances(leadership, "leadership", true);
        }
        public static int GetLeadershipPos(string path)
        {
            byte[] conditions = { 0x80, 0x38, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0x48, 0x39, 01 };
            byte[] mult = { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 };
            // Search for leadership position
            int pos = Editor.Search(path, conditions, mult: mult)[0];
            if (pos == 0)
            {
                Editor.Error();
            }

            return pos;
        }
        public static int GetLeadership(string path)
        {
            int pos = GetLeadershipPos(path) + 5;
            return Editor.GetItemData(path, 1, 2, pos)[0];
        }
        public static void SetLeadership(string path, int leadership)
        {
            int pos = GetLeadershipPos(path) + 5;
            Editor.SetItemData(path, new int[] { leadership }, 2, pos);
        }
    }
}
