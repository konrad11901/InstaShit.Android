
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.ShareFile;

namespace InstaShitAndroid
{
    [Activity(Label = "Advanced mode")]
    public class AdvancedSettingsActivity : Activity
    {
        EditText settingsText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AdvancedSettings);
            settingsText = FindViewById<EditText>(Resource.Id.settingsText);
            if (File.Exists(GetFileLocation("settings.json")))
                settingsText.Text = File.ReadAllText(GetFileLocation("settings.json"));
            Button saveButton = FindViewById<Button>(Resource.Id.saveButton);
            saveButton.Click += (sender, e) =>
            {
                File.WriteAllText(GetFileLocation("settings.json"), settingsText.Text);
                Finish();
            };
            Button shareButton = FindViewById<Button>(Resource.Id.shareButton);
            shareButton.Click += (sender, e) =>
            {
                File.WriteAllText(GetFileLocation("settings.json"), settingsText.Text);
                File.Copy(GetFileLocation("settings.json"),
                          Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "settings.json"), true);
                CrossShareFile.Current.ShareLocalFile(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "settings.json"),
                                                      "Share settings file");
            };
            AlertDialog.Builder alertBuilder = new AlertDialog.Builder(this);
            alertBuilder.SetTitle("Warning");
            alertBuilder.SetMessage("This mode should mostly be used to copy " +
                                    "InstaShit settings to other program. For example, " +
                                    "you can use \"Share\" option to configure " +
                                    "InstaShit bot on Telegram.\nYou can also " +
                                    "modify InstaShit settings here, but " +
                                    "please be sure what you're doing since " +
                                    "you can break this app easily with uncareful changes.");
            alertBuilder.SetNeutralButton("Ok", (senderAlert, args) =>
            {
                
            });
            var dialog = alertBuilder.Create();
            dialog.Show();
        }
        private string GetFileLocation(string fileName)
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
        }
    }
}
