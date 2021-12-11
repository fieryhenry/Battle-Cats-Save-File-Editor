using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class EnemyMod
    {
        public static void EnemyCSV()
        {
            OpenFileDialog fd = new()
            {
                Filter = "files (t_unit.csv)|t_unit.csv",
                Title = "Select a t_unit.csv file"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .csv files");
                Editor.Options();
            }
            string path = fd.FileName;
            List<string> csvData = File.ReadAllLines(path).ToList();

            List<List<string>> enemy_data = new();
            foreach (string line in csvData)
            {
                string[] split = line.Split(',');
                if (split.Length > 15)
                {
                    enemy_data.Add(line.Split(',').ToList());
                }
            }

            string[] values =
            {
                "HP", "Knockback amount", "Movement Speed", "Attack Power", "Time between attacks", "Attack range", "Money Drop",
                "?", "Width", "?", "Red flag", "Area attack flag", "Foreswing", "Floating flag", "Black flag", "Metal Flag", "Traitless Flag",
                "Angel Flag", "Alien Flag", "Zombie Flag", "Knock back chance", "Freeze chance", "Freeze duration", "Slow Chance",
                "Slow Duration", "Crit Chance", "Base Destroyer Flag", "Wave Chance", "Wave Level", "Weaken Chance", "Weaken Duration",
                "Weaken %", "Strengthen Activation health", "Strengthen Multiplier", "Survive Lethal Chance", "Long distance start", "Long distance range",
                "Wave Immunity", "?", "Knockback immunity", "Freeze immunity", "Slow immunity", "Weaken immunity", "Burrow count",
                "Burrow distance", "Revive count", "Revive time", "Percentage of health after revive", "Witch Flag", "Is enemy base flag", "loop?",
                "?", "Self-Destruct Flag (2=self-destruct)", "?", "Soul when dead", "Second attack damage", "Third attack damage", "Second attack start frame",
                "Third attack start frame", "Use ability on first hit flag", "Second attack flag", "Third attack flag", "?",
                "Some flag for gudetama", "Barrier HP", "Warp Chance", "Warp Duration", "Warp min range", "Warp max range",
                "Starred Alien Flag (2-4 = god)", "Some flag for doge sun and the winds", "Eva angel flag", "Relic flag", "Curse Chance",
                "Cuse Duration", "Savage Blow Chance", "Savage Blow Multiplier", "Invincibility Chance", "Invincibility Duration",
                "Toxic Chance", "Toxic Percentage health", "Surge Chance", "Surge Min range", "Surge max range", "Surge Level",
                "Some flag for doge sun, wind enemies and doron", "Mini wave toggle", "Shield HP", "Percentage HP healed when knockbacked",
                "Surge Chance when killed", "Surge min range when killed", "Surge max range when killed", "Surge Level when killed",
                "Aku flag"
            };
            Console.WriteLine("\nThanks to @Vivi-chan#0301 on discord as well as BCU for what the values in the csv mean\nWhat enemy do you want to edit? (enemy id, you can enter multiple ids separated by spaces to edit multiple at once):");
            string[] enemyIDs = Console.ReadLine().Split(' ');
            for (int k = 0; k < enemyIDs.Length; k++)
            {
                int Enemyid = int.Parse(enemyIDs[k]);
                string output = "&";
                for (int i = 0; i < values.Length; i++)
                {
                    output += $"{i}.& {values[i]}:& {enemy_data[Enemyid][i]}\n";
                }
                Editor.ColouredText(output);

                Editor.ColouredText($"&What do you want to edit in enemy id: &{Enemyid}&? (You can enter multiple ids separated by spaces to edit multiple at once)\n");
                string[] answer = Console.ReadLine().Split(' ');
                for (int i = 0; i < answer.Length; i++)
                {
                    int traitID = int.Parse(answer[i]);
                    Editor.ColouredText($"&What value do you want to set &{values[traitID]}& to? (for flags, 1 = on, 0 = off):");
                    int value = (int)Editor.Inputed();
                    enemy_data[Enemyid][traitID] = value.ToString();
                }
                string final = "";
                for (int i = 0; i < enemy_data.Count; i++)
                {
                    string line = "";
                    for (int j = 0; j < enemy_data[i].Count; j++)
                    {
                        line += $"{enemy_data[i][j]},";
                    }
                    line = line.TrimEnd(',');
                    final += $"{line}\r\n";
                }
                File.WriteAllText(path, final);

                byte[] allBytes = File.ReadAllBytes(path);

                // Make sure file length is divisible by 16, so it encrypts properly
                List<byte> ls = allBytes.ToList();
                int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
                rem *= 16;
                rem -= ls.Count;
                for (int i = 0; i < rem && rem != 16; i++)
                {
                    ls.Add((byte)rem);
                }
                File.WriteAllBytes(path, ls.ToArray());
            }
            Editor.ColouredText($"&Finished, wrote data to: &{path}&\n");
        }
    }
}
