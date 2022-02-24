using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Battle_Cats_save_editor.Editor;
using Battle_Cats_save_editor.SaveEdits;

namespace Battle_Cats_save_editor
{
    public class CopyData
    {
        public static object RunGetFunction(string feature_name, string path_original)
        {
            Tuple<Delegate, Delegate> methods = FeatureHandler.copy_methods[feature_name];
            return methods.Item1.DynamicInvoke(new object[] { path_original });
        }
        public static void RunSetFunction(string feature_name, string path_new, List<object> perameters)
        {
            Tuple<Delegate, Delegate> methods = FeatureHandler.copy_methods[feature_name];
            perameters = perameters.Prepend(path_new).ToList();
            methods.Item2.DynamicInvoke(perameters.ToArray());
        }
        public static void Copy(string feature_name, string path_selected)
        {
            Console.WriteLine("Do you want to copy data from another save into the currently selected save(1), or copy data from the currently selected save into another save(2)?");
            string answer = Console.ReadLine();
            string path_has_data = "";
            string path_to_get_data = "";
            if (answer == "1")
            {
                path_to_get_data = path_selected;
                OpenFileDialog dialog = new();
                dialog.Title = "Select a save to copy from";
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    path_has_data = dialog.FileName;
                }
                else
                {
                    Console.WriteLine("Please select a save file");
                    Copy(feature_name, path_selected);
                }
            }
            else
            {
                path_has_data = path_selected;
                OpenFileDialog dialog = new();
                dialog.Title = "Select a save to copy into";
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    path_to_get_data = dialog.FileName;
                    CreateBackup(path_to_get_data);
                }
                else
                {
                    Console.WriteLine("Please select a save file");
                    Copy(feature_name, path_selected);
                }
            }
            override_warning_message = true;
            object return_val = RunGetFunction(feature_name, path_has_data);
            RunSetFunction(feature_name, path_to_get_data, new List<object> { return_val });
            PatchSaveFile.PatchSaveData(path_to_get_data);
            PatchSaveFile.PatchSaveData(path_has_data);
            override_warning_message = false;
        }
    }
}
