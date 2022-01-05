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
        public static int GetLeadership(string path)
        {
            byte[] conditions = { 0x80, 0x38 };
            // Search for leadership position
            int pos = Editor.Search(path, conditions)[0] + 5;
            if (pos == 0)
            {
                Editor.Error();
            }
            return Editor.GetItemData(path, 1, 4, pos)[0];
        }
        public static void SetLeadership(string path, int leadership)
        {
            byte[] conditions = { 0x80, 0x38 };
            // Search for leadership position
            int pos = Editor.Search(path, conditions)[0] + 5;
            Editor.SetItemData(path, new int[] { leadership }, 4, pos);
        }
    }
}
