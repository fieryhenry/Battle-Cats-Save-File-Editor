using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Battle_Cats_save_editor.SaveEdits.MainEventStages;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class UncannyLegends
    {
        public static int uncanny_amount = 0;
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
            byte[] allData = File.ReadAllBytes(path);

            byte[] conditions_1 = { 0, 0, 0, 0x45, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0x47, 0, 0, 0, 0x28, 0, 0, 0, 0x0C, };
            
            byte[] mult = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0,};

            int pos = Editor.Search(path, conditions_1, mult: mult)[0];
            uncanny_amount = allData[pos + 19];
            int pos_1 = pos + (uncanny_amount * 16) + 31;
            int pos_2 = pos_1 + (uncanny_amount * 16);
            int pos_3 = pos_2 + (uncanny_amount * 16 * 12);

            return Tuple.Create(pos_1, pos_2, pos_3);
        }
        public static StageData GetSubchapterData(string path)
        {
            StageData stageData = new();
            int levsBeaten = GetStagePos(path).Item1;
            int levelsCleared = GetStagePos(path).Item2;
            int unlockNextChapter = GetStagePos(path).Item3;

            if (levsBeaten < 1000 || levelsCleared < 1000 || unlockNextChapter < 1000)
            {
                Editor.Error();
            }


            int[] levelsBeatenArray = Editor.GetItemData(path, uncanny_amount * 4, 4, levsBeaten, false);
            int[] levelsClearedArray = Editor.GetItemData(path, uncanny_amount * 4 * total_stages_per_chapter, 4, levelsCleared, false);
            int[] unlockNextChapterArray = Editor.GetItemData(path, uncanny_amount * 4, 4, unlockNextChapter, false);

            List<LevelsBeaten> levels_beaten_data = (List<LevelsBeaten>)GetStarData(levelsBeatenArray, "levelsbeaten", uncanny_amount, total_stages_per_chapter);
            List<Levels> levels_data = (List<Levels>)GetStarData(levelsClearedArray, "levels", uncanny_amount, total_stages_per_chapter);
            List<Unlock> unlock_data = (List<Unlock>)GetStarData(unlockNextChapterArray, "unlock", uncanny_amount, total_stages_per_chapter);

            stageData.levelsbeaten = levels_beaten_data;
            stageData.levels = levels_data;
            stageData.unlocks = unlock_data;

            return stageData;
        }
        public static void Uncanny_Legends(string path)
        {
            StageData data = GetSubchapterData(path);

            string[] subchapter_examples = { "A New Legend", "Here Be Dragons" };

            int[] chapters_to_edit = AskLevels(subchapter_examples, uncanny_amount);
            data = AskEventStagesDecoder(path, data, chapters_to_edit);
            SetSubchapterData(path, data);
        }
        public static void SetSubchapterData(string path, StageData stageData)
        {
            int levsBeaten = GetStagePos(path).Item1;
            int levelsCleared = GetStagePos(path).Item2;
            int unlockNextChapter = GetStagePos(path).Item3;

            int[] levelsBeatenArray = DecodeStarData(stageData, "levelsbeaten");
            int[] levelsClearedArray = DecodeStarData(stageData, "levels");
            int[] unlockNextChapterArray = DecodeStarData(stageData, "unlock");

            Editor.SetItemData(path, levelsBeatenArray, 4, levsBeaten);
            Editor.SetItemData(path, levelsClearedArray, 4, levelsCleared);
            Editor.SetItemData(path, unlockNextChapterArray, 4, unlockNextChapter);
        }
    }
}
