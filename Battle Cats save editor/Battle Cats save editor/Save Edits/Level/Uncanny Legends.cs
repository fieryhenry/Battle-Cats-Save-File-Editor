using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class UncannyLegends
    {
        public static int uncanny_amount = 40;
        public static int total_stages_per_chapter = 12;
        public static int[] AskLevels(string[] subchapter_examples, int totalChapters, string custom_text = null)
        {
            string toOutput = "";
            if (custom_text != null)
            {
                toOutput = custom_text;
            }
            else
            {
                 toOutput = $"&What subchapter do you want to edit?, enter an id starting at &1& = &{subchapter_examples[0]}&, &2& = &{subchapter_examples[1]}& etc, you can enter multiple ids seperated by spaces, e.g &1 5 4 7&, or you can enter 2 ids seperated by a &-& to edit a range of" +    " chapters, e.g &1&-&7&, or you can enter &all& to edit all subchapters at once\n";
            }
            Editor.ColouredText(toOutput);
            string input = Console.ReadLine();
            List<int> chaptersToEdit = new();
            if (input.Contains("-"))
            {
                int start = int.Parse(input.Split('-')[0]);
                int end = int.Parse(input.Split('-')[1]);
                for (int i = start; i <= end; i++)
                {
                    chaptersToEdit.Add(i);
                }
            }
            else if (input.ToLower() == "all")
            {
                int start = 1;
                int end = totalChapters + 1;
                for (int i = start; i < end; i++)
                {
                    chaptersToEdit.Add(i);
                }
            }
            else
            {
                string[] ids = input.Split(' ');
                int[] idsInt = Array.ConvertAll(ids, int.Parse);
                chaptersToEdit.AddRange(idsInt);
            }
            return chaptersToEdit.ToArray();
        }
        public static Tuple<int, int, int> GetStagePos(string path)
        {
            byte[] conditions_1 = { 0, 0, 0, 0x45, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0x47, 0, 0, 0, 0x28, 0, 0, 0, 0x0C, };

            int pos_1 = Editor.Search(path, conditions_1)[0] + 671;
            int pos_2 = pos_1 + 640;
            int pos_3 = pos_2 + 7680;

            return Tuple.Create(pos_1, pos_2, pos_3);
        }
        public static int[] GetStages(string path)
        {
            int pos = GetStagePos(path).Item1;
            int[] stages = Editor.GetItemData(path, uncanny_amount * 4, 4, pos);

            return stages;
        }
        public static void SetStage(string path, List<int> stages, List<int> stars)
        {
            int pos_1 = GetStagePos(path).Item1;
            int pos_2 = GetStagePos(path).Item2;
            int pos_3 = GetStagePos(path).Item3;

            List<int> allStages = Enumerable.Repeat(0, stages.Count * 4).ToList();
            for (int i = 0; i < stages.Count; i++)
            {
                for (int j = 0; j < stars[i]; j++)
                {
                    int set = 0;
                    if (stars[i] > 0)
                    {
                        set = total_stages_per_chapter;
                    }
                    allStages[(i * 4) + j] = set;
                }
            }

            Editor.SetItemData(path, allStages.ToArray(), 4, pos_1);

            List<int> stage_clear_num = Enumerable.Repeat(0, (stages.Count * total_stages_per_chapter) + 1).ToList();
            List<int> unlock_next_chapter = Enumerable.Repeat(0, allStages.Count).ToList();
            int count = 0;
            for (int i = 0; i < stages.Count / 4; i++)
            {
                if (stages[i] > 0)
                {
                    for (int j = 0; j < stars[i]; j++)
                    {
                        for (int k = 0; k < total_stages_per_chapter; k++)
                        {
                            count++;
                            stage_clear_num[(i * 48) + j + (k*4)] = count;
                        }
                    }
                    unlock_next_chapter[i] = 3;
                }
            }
            stage_clear_num[stage_clear_num.Count - 1] = 0;
            Editor.SetItemData(path, stage_clear_num.ToArray(), 4, pos_2);
            Editor.SetItemData(path, unlock_next_chapter.ToArray(), 4, pos_3);
        }
        public static void Uncanny_Legends(string path)
        {
            int[] stages = GetStages(path);

            string[] subchapter_examples = { "A New Legend", "Here Be Dragons" };

            int[] chapters_to_edit = AskLevels(subchapter_examples, uncanny_amount);

            Editor.ColouredText("&Do you want to set all of the stars/crowns at the same time (&1&), or individually (&2&)?\n");
            int choice = (int)Editor.Inputed();
            List<int> stars = Enumerable.Repeat(0, stages.Length).ToList();

            if (choice == 1)
            {
                Editor.ColouredText("&How many stars/crowns do you want to complete for each chapter (&1&-&4&)\n");
                int star = (int)Editor.Inputed();
                stars = Enumerable.Repeat(star, stages.Length).ToList();
            }

            for (int i = 0; i < chapters_to_edit.Length; i++)
            {
                if (choice == 2)
                {
                    Editor.ColouredText($"&How many stars/crowns do you want to complete for chapter &{chapters_to_edit[i]}&? (&1&-&4&)\n");
                    int star = (int)Editor.Inputed();
                    stars[i] = star;
                }
                stages[chapters_to_edit[i]] = total_stages_per_chapter;
            }
            SetStage(path, stages.ToList(), stars);
        }
    }
}
