using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using InstaShitCore;
using Newtonsoft.Json;
using Android.Util;

namespace InstaShitAndroid
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : Activity
    {
        EditText loginText;
        EditText passwordText;
        EditText minSleep;
        EditText maxSleep;
        Switch typoSwitch;
        Switch synonymSwitch;
        Button saveButton;
        Button editDataButton;
        List<List<IntelligentMistakesDataEntry>> mistakesData;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            loginText = FindViewById<EditText>(Resource.Id.loginText);
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            minSleep = FindViewById<EditText>(Resource.Id.minSleep);
            maxSleep = FindViewById<EditText>(Resource.Id.maxSleep);
            typoSwitch = FindViewById<Switch>(Resource.Id.typoSwitch);
            synonymSwitch = FindViewById<Switch>(Resource.Id.synonymSwitch);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            editDataButton = FindViewById<Button>(Resource.Id.editDataButton);
            Settings localSettings;
            if (savedInstanceState != null)
                mistakesData = JsonConvert.DeserializeObject<List<List<IntelligentMistakesDataEntry>>>(savedInstanceState.GetString("mistakesdata"));
            else
            {
                if (File.Exists(GetFileLocation("settings.json")))
                    localSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(GetFileLocation("settings.json")));
                else
                {
                    localSettings = new Settings();
                    localSettings.IntelligentMistakesData = new List<List<IntelligentMistakesDataEntry>>();
                }
                loginText.Text = localSettings.Login;
                passwordText.Text = localSettings.Password;
                minSleep.Text = localSettings.MinimumSleepTime.ToString();
                maxSleep.Text = localSettings.MaximumSleepTime.ToString();
                typoSwitch.Checked = localSettings.AllowTypo;
                synonymSwitch.Checked = localSettings.AllowSynonym;
                mistakesData = localSettings.IntelligentMistakesData;
            }
            saveButton.Click += (sender, e) =>
            {
                File.WriteAllText(GetFileLocation("settings.json"), JsonConvert.SerializeObject(CreateSettings(), Formatting.Indented));
                Finish();
            };
            editDataButton.Click += (sender, e) => 
            {
                var intent = new Intent(this, typeof(IntelligentMistakesDataActivity));
                intent.PutExtra("mistakesdata", JsonConvert.SerializeObject(mistakesData));
                StartActivityForResult(intent, 1);
            };
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    mistakesData = JsonConvert.DeserializeObject<List<List<IntelligentMistakesDataEntry>>>(data.GetStringExtra("mistakesdata"));
                }
            }
        }
        private Settings CreateSettings() => new Settings()
        {
            Login = loginText.Text,
            Password = passwordText.Text,
            MinimumSleepTime = Int32.Parse(minSleep.Text),
            MaximumSleepTime = Int32.Parse(maxSleep.Text),
            IntelligentMistakesData = mistakesData,
            AllowTypo = typoSwitch.Checked,
            AllowSynonym = synonymSwitch.Checked
        };
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("mistakesdata", JsonConvert.SerializeObject(mistakesData));
            base.OnSaveInstanceState(outState);
        }
        private string GetFileLocation(string fileName)
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
        }
    }
}