using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Battle_Cats_save_editor.SaveEdits.MainEventStages;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class StoriesOfLegend
    {
        public static void Sol(string path)
        {
            int total_subchapters = 49;
            StageData data = GetSubchapterData(path);

            string[] examples = { "The Legend Begins", "Passion Land" };
            int[] chaptersToEdit = UncannyLegends.AskLevels(examples, total_subchapters);
            data = AskEventStagesDecoder(path, data, chaptersToEdit);
            SetSubchapterData(path, data);
        }
    }
}
