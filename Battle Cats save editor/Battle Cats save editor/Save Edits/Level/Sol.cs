using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class StoriesOfLegend
    {
        public static void Sol(string path)
        {
            int totalChapters = 49;
            int[] chaptersToEdit = UncannyLegends.AskLevels(new string[] { "Legend Begins", "Passion Land" }, totalChapters);
            MainEventStages.SetEventStages(path, chaptersToEdit);
        }
    }
}
