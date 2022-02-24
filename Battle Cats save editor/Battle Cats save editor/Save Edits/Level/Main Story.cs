using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class MainStory
    {
        public static Tuple<int, int> GetStagePos(string path)
        {
            int offset = Editor.GetGameVersionOffset(path);
            int num_cleared = 946 + offset;
            int stages_cleared = 906 + offset;
            return Tuple.Create(num_cleared, stages_cleared);
        }
        static int total_chapters = 9;
        static int total_stages = 48;
        public static Tuple<List<int>, List<List<int>>> GetStageData(string path)
        {
            Tuple<int, int> poses = GetStagePos(path);
            int num_cleared = poses.Item1;
            int stages_cleared = poses.Item2;

            List<int> total_stages_cleared = Editor.GetItemData(path, total_chapters + 1, 4, stages_cleared, false).ToList();
            List<int> total_stages_cleared_trimmed = new();
            List<List<int>> total_data = new();

            for (int i = 0; i < total_chapters; i++)
            {
                int offset = 0;
                int index = 0;

                if (i > 2)
                {
                    offset = (total_stages * 4) + 12;
                    index = 1;
                }

                int pos = num_cleared + i * ((total_stages * 4) + 12) + offset;
                List<int> num_cleared_chap_data = Editor.GetItemData(path, total_stages, 4, pos, false).ToList();
                total_data.Add(num_cleared_chap_data);

                int total_stages_done = total_stages_cleared[i + index];
                total_stages_cleared_trimmed.Add(total_stages_done);
            }
            return Tuple.Create(total_stages_cleared_trimmed, total_data);
        }
        public static void SetStageData(string path, Tuple<List<int>, List<List<int>>> stage_data)
        {
            Tuple<int, int> poses = GetStagePos(path);
            int num_cleared = poses.Item1;
            int stages_cleared = poses.Item2;

            for (int i = 0; i < stage_data.Item1.Count; i++)
            {
                int offset = 0;
                int index = 0;

                if (i > 2)
                {
                    offset = (total_stages * 4) + 12;
                    index = 1;
                }

                int pos = num_cleared + i * ((total_stages * 4) + 12) + offset;
                Editor.SetItemData(path, stage_data.Item2[i].ToArray(), 4, pos);
                Editor.SetItemData(path, new int[] { stage_data.Item1[i] }, 4, stages_cleared + ((i + index) * 4));
            }
        }
        public static void Stage(string path)
        {
            Tuple<List<int>, List<List<int>>> stage_data = GetStageData(path);
            Editor.ColouredText($"&What chapter do you want to clear?{Editor.multipleVals}\n&{Editor.CreateOptionsList<string>(AllTreasures.chapters)}{AllTreasures.chapters.Length + 1}. &All at once\n");
            string[] response = Console.ReadLine().Split(' ');
            foreach (string chapter in response)
            {
                int chapter_id = int.Parse(chapter) - 1;
                if (chapter_id == AllTreasures.chapters.Length)
                {
                    List<int> total_stage_data = Enumerable.Repeat(total_stages, total_chapters).ToList();
                    List<List<int>> level_data = Enumerable.Repeat(Enumerable.Repeat(1, total_stages).ToList(), total_chapters).ToList();
                    stage_data = Tuple.Create(total_stage_data, level_data);
                }
                else
                {
                    stage_data.Item1[chapter_id] = total_stages;
                    stage_data.Item2[chapter_id] = Enumerable.Repeat(1, total_stages).ToList();
                }
            }
            SetStageData(path, stage_data);
            Console.WriteLine("Successfully cleared chapters");
        }
    }
}
