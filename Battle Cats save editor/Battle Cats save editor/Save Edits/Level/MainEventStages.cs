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
        public static void SetEventStages(string path, int[] chaptersToEdit)
        {
            long unlock = GetEventStagePos(path).Item3;
            // Times beaten/graphical fix
            long levels = GetEventStagePos(path).Item2;
            // Levels beaten/unlock next chapter/levels
            long levsBeaten = GetEventStagePos(path).Item1;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            // Stars unlocked/unlock next chapter

            if (levels == 0 || unlock == 0 || levsBeaten == 0)
            {
                Editor.Error();
            }
            Editor.ColouredText("&Do you want to set all of the stars/crowns at the same time (&1&), or individually (&2&)?\n");
            string sameOrIndividual = Console.ReadLine();
            int stars = 0;
            // Same
            if (sameOrIndividual == "1")
            {
                Editor.ColouredText("&How many stars/crowns do you want to complete for each chapter (&1&-&4&)\n");
                stars = (int)Editor.Inputed();
            }
            for (int i = 0; i < chaptersToEdit.Length; i++)
            {
                // Individual
                if (sameOrIndividual == "2")
                {
                    Editor.ColouredText($"&How many stars/crowns do you want to complete for chapter &{chaptersToEdit[i]}&? (&1&-&4&)\n");
                    stars = (int)Editor.Inputed();
                }
                // Levels beaten, required for next chapter to unlock
                int id = chaptersToEdit[i] - 1;
                stream.Position = levsBeaten + (id * 4);
                for (int j = 0; j < stars; j++)
                {
                    stream.WriteByte((byte)total_stages_per_chapter);
                }
                // Stars/crowns unlocked and required for next chapter to unlock
                stream.Position = unlock - 6152 + ((id + 1) * 4);
                stream.WriteByte(3);
                // Times stage has been beaten, required to avoid graphical issues
                stream.Position = levels + (id * 96);
                long startpos = stream.Position;
                for (int j = 0; j < stars; j++)
                {
                    for (int k = 0; k < total_stages_per_chapter; k++)
                    {
                        stream.WriteByte(1);
                        stream.Position += 7;
                    }
                    stream.Position = startpos + (j * 2) + 2;
                }
            }
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
                    unlock = i;
                    break;
                }
            }
            return Tuple.Create(levsBeaten, levels, unlock);
        }
        public static int total_stages_per_chapter = 8;
        public static List<Tuple<string, int>> GetEventData()
        {
            string[] data = Editor.MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Map%20Names/All_day_event.tsv")).Split('\n');
            string[] map_stage = Editor.MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Map%20Names/MapStageLimitMessage.csv")).Split('\n');

            List<Tuple<string, int>> event_data = new();

            foreach (string stage in data)
            {
                string[] stage_data = stage.Split("	".ToCharArray());
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
        public static void EventStages(string path)
        {
            int totalChapters = 1500;
            Console.WriteLine("Do you want some examples of stage ids? (yes/no)");
            string answer = Console.ReadLine();
            if (answer == "yes")
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
            string text = $"&What subchapter do you want to edit?:\nEnter a chapter id, you can enter multiple ids seperated by spaces, e.g &1 5 4 7&, or you can enter 2 ids seperated by a &-& to edit a range of" +
                " chapters, e.g &1&-&7&, or you can enter &all& to edit all subchapters at once (all also includes Sol)\n";
            int[] chaptersToEdit = UncannyLegends.AskLevels(null, totalChapters, text);
            SetEventStages(path, chaptersToEdit);
        }
    }
}
