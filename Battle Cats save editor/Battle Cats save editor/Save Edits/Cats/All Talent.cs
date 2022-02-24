using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class AllTalent
    {
        public static int GetTalentPos(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x4c && data[i + 4] == 0x21 && data[i + 8] == 0x0f && data[i + 12] == 0x04 && data[i + 9652] == 0x37 && data[i + 9877] == 0x4d)
                {
                    return i + 9971;
                }
            }
            return 0;
        }
        public static Dictionary<int, List<Tuple<int, byte, byte>>> GetTalents(string path)
        {
            Console.WriteLine("Getting talent data");
            byte[] allData = File.ReadAllBytes(path);
            int pos = GetTalentPos(path);
            if (pos == 0)
            {
                Editor.Error();
            }

            int total_cats = BitConverter.ToInt16(new byte[] { allData[pos], allData[pos + 1] }, 0);
            Dictionary<int, List<Tuple<int, byte, byte>>> all_talent_data = new();
            for (int i = pos + 4; all_talent_data.Count < total_cats; i++)
            {
                int cat_id = BitConverter.ToInt16(new byte[] { allData[i], allData[i + 1] }, 0);
                int talent_amount = allData[i + 4];
                List<Tuple<int, byte, byte>> cat_talent_data = new();
                for (int j = 0; j < talent_amount; j++)
                {
                    byte talent_id = allData[i + 8 + (j * 8)];
                    byte level = allData[i + 8 + (j * 8) + 4];

                    Tuple<int, byte, byte> talent_pair = Tuple.Create(i + 8 + (j * 8) + 4, talent_id, level);
                    cat_talent_data.Add(talent_pair);
                }
                i += (talent_amount * 8) + 7;
                all_talent_data.Add(cat_id, cat_talent_data);
            }
            return all_talent_data;
        }
        public static void SetTalents(string path, Dictionary<int, List<Tuple<int, byte, byte>>> talent_data)
        {
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            foreach (KeyValuePair<int, List<Tuple<int, byte, byte>>> cat in talent_data)
            {
                int cat_id = cat.Key;
                List<Tuple<int, byte, byte>> talents = cat.Value;
                foreach (Tuple<int, byte, byte> talent in talents)
                {
                    int pos = talent.Item1;
                    stream.Position = pos;
                    stream.WriteByte(talent.Item3);
                }
            }
        }
        public static List<string> format_skill_description(string[] skill_descriptions)
        {
            skill_descriptions[0] = "";
            List<string> trimmed_descriptions = new();
            foreach (string skill_description in skill_descriptions)
            {
                int pipe_index = skill_description.IndexOf('|');
                if (pipe_index == -1) continue;
                string description_trimmed = skill_description.Remove(0, pipe_index + 1);

                int period_index = description_trimmed.IndexOf('.');
                if (period_index != -1)
                {
                    description_trimmed = description_trimmed.Remove(period_index);
                }
                int less_than_index = description_trimmed.IndexOf('<');
                if (less_than_index != -1)
                {
                    description_trimmed = description_trimmed.Remove(less_than_index);
                }
                trimmed_descriptions.Add(description_trimmed);
            }
            return trimmed_descriptions;
        }
        public static JToken SearchWithAbilityID(JToken talent_data, int ability_id)
        {
            foreach (JToken talent_skill_data in talent_data)
            {
                if (Convert.ToInt32(talent_skill_data["ability_id"]) == ability_id)
                {
                    return talent_skill_data;
                }
            }
            return null;
        }
        public static int GetIndexFromList(List<Tuple<int, byte, byte>> list, int val)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == val)
                {
                    return i;
                }
            }
            return -1;
        }
        public static void MaxTalents(string path)
        {
            Dictionary<int, List<Tuple<int, byte, byte>>> curr_talent_data = GetTalents(path);
            JToken talent_skill_data = GetSkillData();
            foreach (KeyValuePair<int, List<Tuple<int, byte, byte>>> cat_data in curr_talent_data)
            {
                for (int i = 0; i < cat_data.Value.Count; i++)
                {
                    Tuple<int, byte, byte> talent = cat_data.Value[i];
                    if (cat_data.Key < 9) continue;
                    JToken talent_data = SearchWithAbilityID(talent_skill_data, talent.Item2);
                    if (talent_data == null) continue;
                    int max;
                    if (talent_data["max"] == null) max = 1;
                    else
                    {
                        max = talent_data["max"].ToObject<int>();
                    }
                    if (talent_data["max_other_cro"] != null)
                    {
                        if (talent_data["max_other_cro"][$"{cat_data.Key}"] == null) continue;
                        max = talent_data["max_other_cro"][$"{cat_data.Key}"].ToObject<int>();
                    }
                    int pos = curr_talent_data[cat_data.Key][i].Item1;
                    int skill_id = curr_talent_data[cat_data.Key][i].Item2;
                    Tuple<int, byte, byte> skill_data = Tuple.Create(pos, (byte)skill_id, (byte)max);
                    curr_talent_data[cat_data.Key][i] = skill_data;
                }
            }
            SetTalents(path, curr_talent_data);
            Editor.ColouredText($"&Successfully set talents\n");

        }
        public static void IndividualTalents(string path)
        {
            JToken talent_skill_data = GetSkillData();
            string[] skill_descriptions = Editor.MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Talent%20Data/SkillDescriptions.csv")).Split('\n');
            List<string> trimmed_descriptions = format_skill_description(skill_descriptions);

            Dictionary<int, List<Tuple<int, byte, byte>>> curr_talent_data = GetTalents(path);

            Editor.ColouredText($"&Enter the cat release order for a cat {Editor.multipleVals}:\n");
            string[] response = Console.ReadLine().Split(' ');

            foreach (string cat_id_str in response)
            {
                int cat_id = int.Parse(cat_id_str);

                if (!curr_talent_data.ContainsKey(cat_id))
                {
                    Editor.ColouredText($"&Cat &{cat_id}& does not have talents in your current game version");
                    continue;
                }
                for (int i = 0; i < curr_talent_data[cat_id].Count; i++)
                {
                    Tuple<int, byte, byte> talent_cat_data = curr_talent_data[cat_id][i];

                    JToken talent_data = SearchWithAbilityID(talent_skill_data, talent_cat_data.Item2);

                    int text_id = talent_data["text_ids"].ToObject<int[]>()[0];
                    string desc = trimmed_descriptions[text_id - 1];
                    Editor.ColouredText($"&{i + 1}. &{desc}& : &{curr_talent_data[cat_id][i].Item3}&\n");
                }
                Editor.ColouredText($"&{curr_talent_data[cat_id].Count + 1}. &All at once\n");
                Editor.ColouredText($"&What talent do you want to edit?{Editor.multipleVals}:\n");
                string[] answer = Console.ReadLine().Split(' ');
                foreach (string skill_id_str in answer)
                {
                    int skill_index = int.Parse(skill_id_str) - 1;
                    if (skill_index == curr_talent_data[cat_id].Count)
                    {
                        Editor.ColouredText($"&What level do you want to set the talents to? (max &10&):\n");
                        int input_level = (int)Editor.Inputed();
                        for (int i = 0; i < curr_talent_data[cat_id].Count; i++)
                        {
                            int cat_pos = curr_talent_data[cat_id][i].Item1;
                            int skillid = curr_talent_data[cat_id][i].Item2;

                            JToken talentdata = SearchWithAbilityID(talent_skill_data, skillid);
                            int maximum = talentdata["max"].ToObject<int>();
                            if (talentdata["max_other_cro"] != null)
                            {
                                maximum = talentdata["max_other_cro"][$"{cat_id}"].ToObject<int>();
                            }
                            int level_to_set = Editor.MaxMinCheck(input_level, maximum);
                            Tuple<int, byte, byte> skilldata = Tuple.Create(cat_pos, (byte)skillid, (byte)level_to_set);
                            curr_talent_data[cat_id][i] = skilldata;
                        }
                        break;
                    }
                    if (skill_index > curr_talent_data[cat_id].Count)
                    {
                        Console.WriteLine($"Error, please enter a number between 1 and {curr_talent_data[cat_id].Count+1}");
                        continue;
                    }
                    int skill_id = curr_talent_data[cat_id][skill_index].Item2;

                    JToken talent_data = SearchWithAbilityID(talent_skill_data, skill_id);
                    int max = talent_data["max"].ToObject<int>();

                    if (talent_data["max_other_cro"] != null)
                    {
                        max = talent_data["max_other_cro"][$"{cat_id}"].ToObject<int>();
                    }

                    Editor.ColouredText($"&What level do you want to set &{trimmed_descriptions[skill_id - 1]}& to? (max &{max}&):\n");
                    int level = (int)Editor.Inputed();
                    level = Editor.MaxMinCheck(level, max);

                    int pos = curr_talent_data[cat_id][skill_index].Item1;
                    Tuple<int, byte, byte> skill_data = Tuple.Create(pos, (byte)skill_id, (byte)level);
                    curr_talent_data[cat_id][skill_index] = skill_data;
                }
            }
            SetTalents(path, curr_talent_data);
            Editor.ColouredText($"&Successfully set talents\n");

        }
        public static JToken GetSkillData()
        {
            Console.WriteLine("Getting skill data");
            string talent_json = Editor.MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Talent%20Data/talents.json"));
            JObject talent_skill_data = JObject.Parse(talent_json);

            return talent_skill_data["Skills"];
        }
    }
}
