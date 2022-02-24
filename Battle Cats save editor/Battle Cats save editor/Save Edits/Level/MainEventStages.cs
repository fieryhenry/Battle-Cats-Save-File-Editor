using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class MainEventStages
    {
        public class StageData
        {
            public List<Unlock> unlocks = new();
            public List<Levels> levels = new();
            public List<LevelsBeaten> levelsbeaten = new();
        }
        public class Unlock
        {
            public List<int> stars = new();
        }
        public class Levels
        {
            public List<LevelData> leveldata = new();
        }
        public class LevelData
        {
            public List<int> stars = new();
        }
        public class LevelsBeaten
        {
            public List<int> stars = new();
        }
        static int total_subchapters = 1500;
        static int levels_per_subchapter = 12;
        public static object GetStarData(int[] input_arr, string type, int total_subs, int levels_per_sub)
        {
            int[][] data = Editor.SplitArray(input_arr, 4);
            if (type == "levelsbeaten")
            {
                List<LevelsBeaten> star_data = new ();
                for (int i = 0; i < data.Length; i++)
                {
                    LevelsBeaten level = new();
                    level.stars = data[i].ToList();
                    star_data.Add(level);
                }
                return star_data;
            }
            else if (type == "unlock")
            {
                List<Unlock> star_data = new();
                for (int i = 0; i < data.Length; i++)
                {
                    Unlock Unlock = new();
                    Unlock.stars = data[i].ToList();
                    star_data.Add(Unlock);
                }
                return star_data;
            }
            else
            {
                List<Levels> levels = new();
                for (int i = 0; i < total_subs; i++)
                {
                    Levels level = new();
                    for (int j = 0; j < levels_per_sub; j++)
                    {
                        LevelData levelData = new();
                        for (int k = 0; k < 4; k++)
                        {
                            levelData.stars.Add(input_arr[i * 48 + j * 4 + k]);
                            //levels[i].leveldata[j].stars[k] = input_arr[i * 48 + j * 4 + k];
                        }
                        level.leveldata.Add(levelData);
                    }
                    levels.Add(level);
                }
                return levels;
            }

        }
        public static int[] DecodeStarData(StageData stageData, string type)
        {
            List<int> data = new();
            if (type == "levelsbeaten")
            {
                for (int i = 0; i < stageData.levelsbeaten.Count; i++)
                {
                    for (int j = 0; j < stageData.levelsbeaten[i].stars.Count; j++)
                    {
                        data.Add(stageData.levelsbeaten[i].stars[j]);
                    }
                }
            }
            else if (type == "levels")
            {
                int[] data_test = new int[stageData.levels.Count * 4 * levels_per_subchapter];

                for (int i = 0; i < stageData.levels.Count; i++)
                {
                    for (int j = 0; j < stageData.levels[i].leveldata.Count; j++)
                    {
                        for (int k = 0; k < stageData.levels[i].leveldata[j].stars.Count; k++)
                        {
                            data_test[i*48 + j*4 + k] = stageData.levels[i].leveldata[j].stars[k];
                        }
                    }
                }
                data = data_test.ToList();
            }
            else
            {
                for (int i = 0; i < stageData.unlocks.Count; i++)
                {
                    for (int j = 0; j < stageData.unlocks[i].stars.Count; j++)
                    {
                        data.Add(stageData.unlocks[i].stars[j]);
                    }
                }
            }
            return data.ToArray();
        }
        public static StageData GetSubchapterData(string path)
        {
            StageData stageData = new();
            int levsBeaten = GetEventStagePos(path).Item1;
            int levelsCleared = GetEventStagePos(path).Item2;
            int unlockNextChapter = GetEventStagePos(path).Item3;

            int[] levelsBeatenArray = Editor.GetItemData(path, total_subchapters * 4, 1, levsBeaten, false);
            int[] levelsClearedArray = Editor.GetItemData(path, total_subchapters * 4 * levels_per_subchapter, 2, levelsCleared, false);
            int[] unlockNextChapterArray = Editor.GetItemData(path, total_subchapters * 4, 1, unlockNextChapter, false);

            List<LevelsBeaten> levels_beaten_data = (List<LevelsBeaten>)GetStarData(levelsBeatenArray, "levelsbeaten", total_subchapters, levels_per_subchapter);
            List<Levels> levels_data = (List<Levels>)GetStarData(levelsClearedArray, "levels", total_subchapters, levels_per_subchapter);
            List<Unlock> unlock_data = (List<Unlock>)GetStarData(unlockNextChapterArray, "unlock", total_subchapters, levels_per_subchapter);

            stageData.levelsbeaten = levels_beaten_data;
            stageData.levels = levels_data;
            stageData.unlocks = unlock_data;

            return stageData;
        }
        public static void SetSubchapterData(string path, StageData stageData)
        {
            int levsBeaten = GetEventStagePos(path).Item1;
            int levelsCleared = GetEventStagePos(path).Item2;
            int unlockNextChapter = GetEventStagePos(path).Item3;

            int[] levelsBeatenArray = DecodeStarData(stageData, "levelsbeaten");
            int[] levelsClearedArray = DecodeStarData(stageData, "levels");
            int[] unlockNextChapterArray = DecodeStarData(stageData, "unlock");

            Editor.SetItemData(path, levelsBeatenArray, 1, levsBeaten);
            Editor.SetItemData(path, levelsClearedArray, 2, levelsCleared);
            Editor.SetItemData(path, unlockNextChapterArray, 1, unlockNextChapter);
        }
        public static Tuple<int, int, int> GetEventStagePos(string path)
        {
            int levsBeaten = 0;
            int levels = 0;
            int unlock = 0;

            byte[] allData = File.ReadAllBytes(path);
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 5 && allData[i + 1] == 0x2c && allData[i + 2] == 1 && allData[i + 3] == 4 && allData[i + 4] == 0x0c)
                {
                    levsBeaten = i + 6005;
                    levels = i + 12005;
                }
                else if (allData[i] == 0x2C && allData[i + 1] == 01 && allData[i + 2] == 0 && allData[i - 1] == 0 && allData[i + 3] == 0 && allData[i - 2] == 0 && allData[i - 3] == 0)
                {
                    unlock = i - 6152;
                    if (levels != 0)
                    {
                        break;
                    }
                }
            }
            return Tuple.Create(levsBeaten, levels, unlock);
        }
        public static List<Tuple<string, int>> GetEventData()
        {
            string[] data = Editor.MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Map%20Names/map_list.tsv")).Split('\n');
            string[] map_stage = Editor.MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Map%20Names/MapStageLimitMessage.csv")).Split('\n');

            List<Tuple<string, int>> event_data = new();

            foreach (string stage in data)
            {
                string stage_stripped = stage.Replace("&", "and");
                if (stage_stripped.Length == 0)
                {
                    continue;
                }
                string[] stage_data = stage_stripped.Split('\t');
                string name = stage_data[0];
                int id = int.Parse(stage_data[1]) - 699;

                event_data.Add(Tuple.Create(name, id));
            }
            foreach (string map in map_stage)
            {
                string[] stage_data = map.Split('|');
                int id = int.Parse(stage_data[0]) - 100699;
                string name = stage_data[3].Trim('"');

                event_data.Add(Tuple.Create(name, id));
            }
            event_data.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            event_data.Reverse();
            return event_data;

        }
        public static StageData AskEventStagesDecoder(string path, StageData data, int[] chaptersToEdit)
        {
            Editor.ColouredText("&Do you want to set all of the stars/crowns at the same time (&1&), or individually (&2&)?\n");
            string sameOrIndividual = Console.ReadLine();
            int stars = 0;
            // Same
            if (sameOrIndividual == "1")
            {
                Editor.ColouredText("&How many stars/crowns do you want to complete for each chapter (&1& to &4&)\n");
                stars = (int)Editor.Inputed();
            }

            foreach (int chapter_id in chaptersToEdit)
            {
                if (sameOrIndividual == "2")
                {
                    Editor.ColouredText($"&How many stars/crowns do you want to complete for chapter &{chapter_id}&? (&1&-&4&)\n");
                    stars = (int)Editor.Inputed();
                }
                for (int i = 0; i < stars; i++)
                {
                    data.unlocks[chapter_id - 1].stars[i] = 3;
                    data.levelsbeaten[chapter_id - 1].stars[i] = data.levels[chapter_id - 1].leveldata.Count;
                    for (int j = 0; j < data.levels[chapter_id - 1].leveldata.Count; j++)
                    {
                        data.levels[chapter_id - 1].leveldata[j].stars[i] = 1;
                    }
                }
            }
            return data;
        }
        public static void EventStages(string path)
        {
            StageData data = GetSubchapterData(path);

            Console.WriteLine("Do you want some examples of stage ids? (yes/no)");
            string answer = Console.ReadLine();
            if (answer == "yes")
            {
                DisplayEventData();
            }
            string text = $"&What subchapter do you want to edit?:\nEnter a chapter id, you can enter multiple ids seperated by spaces, e.g &1 5 4 7&, or you can enter 2 ids seperated by a &-& to edit a range of" +
                " chapters, e.g &1&-&7&, or you can enter &all& to edit all subchapters at once (all also includes Sol)\n";
            int[] chaptersToEdit = UncannyLegends.AskLevels(null, total_subchapters, text);
            data = AskEventStagesDecoder(path, data, chaptersToEdit);
            SetSubchapterData(path, data);
            Console.WriteLine("Successfly cleared stages");
        }
        public static void DisplayEventData()
        {
            List<Tuple<string, int>> event_data = GetEventData();
            List<string> stage_names = new();
            List<int> stage_ids = new();

            foreach (Tuple<string, int> stage in event_data)
            {
                stage_names.Add(stage.Item1);
                stage_ids.Add(stage.Item2);
            }

            Editor.ColouredText($"{Editor.CreateOptionsList(stage_names.ToArray(), stage_ids.ToArray(), false)}");
            Console.WriteLine("You'll have to figure out other ids");
        }
    }
}
