using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.IO;
using InstaShitCore;
using Newtonsoft.Json;

namespace InstaShitAndroid
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : Activity
    {
        EditText _loginText;
        EditText _passwordText;
        EditText _minSleep;
        EditText _maxSleep;
        Switch _typoSwitch;
        Switch _synonymSwitch;
        Button _saveButton;
        Button _editDataButton;
        List<List<IntelligentMistakesDataEntry>> _mistakesData;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            _loginText = FindViewById<EditText>(Resource.Id.loginText);
            _passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            _minSleep = FindViewById<EditText>(Resource.Id.minSleep);
            _maxSleep = FindViewById<EditText>(Resource.Id.maxSleep);
            _typoSwitch = FindViewById<Switch>(Resource.Id.typoSwitch);
            _synonymSwitch = FindViewById<Switch>(Resource.Id.synonymSwitch);
            _saveButton = FindViewById<Button>(Resource.Id.saveButton);
            _editDataButton = FindViewById<Button>(Resource.Id.editDataButton);
            Settings localSettings;
            if (savedInstanceState != null)
                _mistakesData = JsonConvert.DeserializeObject<List<List<IntelligentMistakesDataEntry>>>(savedInstanceState.GetString("mistakesdata"));
            else
            {
                if (File.Exists(GetFileLocation("settings.json")))
                    localSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(GetFileLocation("settings.json")));
                else
                {
                    localSettings = new Settings();
                    localSettings.IntelligentMistakesData = new List<List<IntelligentMistakesDataEntry>>();
                }
                _loginText.Text = localSettings.Login;
                _passwordText.Text = localSettings.Password;
                _minSleep.Text = localSettings.MinimumSleepTime.ToString();
                _maxSleep.Text = localSettings.MaximumSleepTime.ToString();
                _typoSwitch.Checked = localSettings.AllowTypo;
                _synonymSwitch.Checked = localSettings.AllowSynonym;
                _mistakesData = localSettings.IntelligentMistakesData;
            }
            _saveButton.Click += (sender, e) =>
            {
                File.WriteAllText(GetFileLocation("settings.json"), JsonConvert.SerializeObject(CreateSettings(), Formatting.Indented));
                Finish();
            };
            _editDataButton.Click += (sender, e) => 
            {
                var intent = new Intent(this, typeof(IntelligentMistakesDataActivity));
                intent.PutExtra("mistakesdata", JsonConvert.SerializeObject(_mistakesData));
                StartActivityForResult(intent, 1);
            };
            Button advancedButton = FindViewById<Button>(Resource.Id.advancedButton);
            advancedButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(AdvancedSettingsActivity));
                StartActivity(intent);
            };
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    _mistakesData = JsonConvert.DeserializeObject<List<List<IntelligentMistakesDataEntry>>>(data.GetStringExtra("mistakesdata"));
                }
            }
        }
        private Settings CreateSettings() => new Settings()
        {
            Login = _loginText.Text,
            Password = _passwordText.Text,
            MinimumSleepTime = Int32.Parse(_minSleep.Text),
            MaximumSleepTime = Int32.Parse(_maxSleep.Text),
            IntelligentMistakesData = _mistakesData,
            AllowTypo = _typoSwitch.Checked,
            AllowSynonym = _synonymSwitch.Checked
        };
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("mistakesdata", JsonConvert.SerializeObject(_mistakesData));
            base.OnSaveInstanceState(outState);
        }
        private string GetFileLocation(string fileName)
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
        }
    }
}