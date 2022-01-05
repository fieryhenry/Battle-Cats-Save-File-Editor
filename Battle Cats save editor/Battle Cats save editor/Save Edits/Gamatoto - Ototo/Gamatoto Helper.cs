using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class GamatotoHelper
    {
        public static int total_helper_types = 5;
        public static int[] SortHelpers(int[] helpers)
        {
            int[] sorted_helpers = new int[total_helper_types];
            for (int i = 0; i < helpers.Length; i++)
            {
                if (helpers[i] <= 53) sorted_helpers[0]++;
                else if (helpers[i] <= 83) sorted_helpers[1]++;
                else if (helpers[i] <= 108) sorted_helpers[2]++;
                else if (helpers[i] <= 128) sorted_helpers[3]++;
                else if (helpers[i] <= 148) sorted_helpers[4]++;
            }
            return sorted_helpers;
        }
        public static int[] UnsortHelpers(int[] sorted_helpers)
        {
            List<int> unsorted_helpers = new();
            for (int i = 0; i < sorted_helpers.Length; i++)
            {
                for (int j = 0; j < sorted_helpers[i]; j++)
                {
                    if (i == 0) unsorted_helpers.Add(1 + j);
                    else if (i == 1) unsorted_helpers.Add(54 + j);
                    else if (i == 2) unsorted_helpers.Add(84 + j);
                    else if (i == 3) unsorted_helpers.Add(109 + j);
                    else if (i == 4) unsorted_helpers.Add(129 + j);
                }
            }
            return unsorted_helpers.ToArray();
        }
        public static int[] GetHelpers(string path)
        {
            int pos = Editor.GetPlatinumTicketPos(path)[0] - 1022;
            List<int> helpers = Editor.GetItemData(path, 25, 4, pos).ToList();
            helpers.RemoveAll(i => i == -1);

            return SortHelpers(helpers.ToArray());
        }
        public static void SetHelpers(string path, int[] helpers)
        {
            helpers = UnsortHelpers(helpers);
            int pos = Editor.GetPlatinumTicketPos(path)[0] - 1022;
            Editor.SetItemData(path, helpers, 4, pos);
        }
        public static void GamHelp(string path)
        {
            int[] helpers = GetHelpers(path);
            string[] helper_names =
            {
                "Interns", "Lackys", "Underlings", "Assistants", "Legends"
            };
            Editor.ColouredText($"&You have:\n&{Editor.CreateOptionsList(helper_names, helpers, false)}Total &: {helpers.Sum()}&\n");
            Editor.ColouredText($"&What do you want to edit? {Editor.multipleVals}:\n&{Editor.CreateOptionsList<string>(helper_names)}");
            string[] answer = Console.ReadLine().Split(' ');

            foreach (string input in answer)
            {
                int choice = int.Parse(input) - 1;
                Editor.ColouredText($"&What do you want to set the amount of &{helper_names[choice]}& to? (max of 10 helpers total to allow gamatoto to an expedition):\n");
                int val = (int)Editor.Inputed();
                helpers[choice] = val;
            }
            SetHelpers(path, helpers);
            Editor.ColouredText($"&Set gamatoto helpers to:\n&{Editor.CreateOptionsList(helper_names, helpers, false)}Total &: {helpers.Sum()}&\n");

        }
    }
}
