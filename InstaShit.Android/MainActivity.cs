using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.Content;
using System.IO;

namespace InstaShitAndroid
{
    [Activity(Label = "InstaShit.Android", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        public TextView StatusText;
        public ProgressBar ProgressBar;
        public Button StartButton;
        public TextView AnswerCountText;
        public Button LogButton;
        public Button SettingsButton;
        public static readonly List<string> Log = new List<string>();
        Receiver _receiver;
        public static int Counter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var FooterText = FindViewById<TextView>(Resource.Id.footerText);
            string version = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
            FooterText.Text = $"InstaShit.Android v{version} - Created by Konrad Krawiec";
            StatusText = FindViewById<TextView>(Resource.Id.statusTextView);
            StartButton = FindViewById<Button>(Resource.Id.startButton);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            AnswerCountText = FindViewById<TextView>(Resource.Id.answerCountText);
            LogButton = FindViewById<Button>(Resource.Id.logButton);
            SettingsButton = FindViewById<Button>(Resource.Id.settingsButton);
            _receiver = new Receiver();
            _receiver.Activity = this;
            if(savedInstanceState != null)
                StatusText.Text = savedInstanceState.GetString("statusText", "Status: Stopped");
            if(StatusText.Text == "Status: Stopped")
            {
                StartButton.Text = "Start InstaShit";
                StartButton.Enabled = true;
                ProgressBar.Indeterminate = false;
                ProgressBar.Progress = 0;
            }
            else if (StatusText.Text == "Status: Running")
            {
                StartButton.Text = "Stop InstaShit";
                StartButton.Enabled = true;
                ProgressBar.Indeterminate = true;
                SettingsButton.Enabled = false;
                AnswerCountText.Visibility = Android.Views.ViewStates.Visible;
            }
            else if (StatusText.Text == "Status: Stopping")
            {
                StartButton.Text = "Stopping...";
                StartButton.Enabled = false;
                SettingsButton.Enabled = false;
                ProgressBar.Indeterminate = true;
                AnswerCountText.Visibility = Android.Views.ViewStates.Visible;
            }
            else if (StatusText.Text == "Status: Finished")
            {
                StartButton.Text = "Start InstaShit";
                StartButton.Enabled = true;
                StatusText.Text = "Status: Stopped";
                ProgressBar.Indeterminate = false;
                ProgressBar.Progress = 100;
            }
            AnswerCountText.Text = "Answered questions: " + Counter;
            LogButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(LogActivity));
                intent.PutStringArrayListExtra("log", Log);
                StartActivity(intent);
            };
            SettingsButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
            };
            StartButton.Click += (sender, e) =>
            {
                if (StartButton.Text == "Start InstaShit")
                {
                    Log.Clear();
                    if(!File.Exists(GetFileLocation("settings.json")))
                    {
                        AlertDialog.Builder alertBuilder = new AlertDialog.Builder(this);
                        alertBuilder.SetTitle("Error");
                        alertBuilder.SetMessage("The settings file doesn't exist. Do you want to create it now?");
                        alertBuilder.SetPositiveButton("Yes", (senderAlert, args) =>
                        {
                            var intent = new Intent(this, typeof(SettingsActivity));
                            StartActivity(intent);
                        });
                        alertBuilder.SetNegativeButton("No", (senderAlert, args) =>
                        {
                            
                        });
                        var dialog = alertBuilder.Create();
                        dialog.Show();
                        Log.Add("Can't find settings file!");
                        return;
                    }
                    InstaShitService.ShouldContinue = true;
                    StartButton.Text = "Stop InstaShit";
                    StatusText.Text = "Status: Running";
                    SettingsButton.Enabled = false;
                    ProgressBar.Indeterminate = true;
                    Intent instaShitIntent = new Intent(this, typeof(InstaShitService));
                    Counter = 0;
                    AnswerCountText.Text = "Answered questions: " + Counter;
                    AnswerCountText.Visibility = Android.Views.ViewStates.Visible;
                    StartService(instaShitIntent);
                }
                else
                {
                    InstaShitService.ShouldContinue = false;
                    StatusText.Text = "Status: Stopping";
                    StartButton.Text = "Stopping...";
                    StartButton.Enabled = false;
                }
            };
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(_receiver, new IntentFilter("com.InstaShit.Android.INFORMATION"));
        }
        protected override void OnDestroy()
        {
            LocalBroadcastManager.GetInstance(this).UnregisterReceiver(_receiver);
            base.OnDestroy();
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("statusText", StatusText.Text);
            base.OnSaveInstanceState(outState);
        }
        private string GetFileLocation(string fileName)
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
        }
    }
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class Receiver : BroadcastReceiver
    {
        public MainActivity Activity { get; set; }
        public override void OnReceive(Context context, Intent intent)
        {
            string s = intent.GetStringExtra("com.InstaShit.Android.INFORMATION_MESSAGE");
            if (s == "FINISHED")
            {
                Activity.StartButton.Text = "Start InstaShit";
                Activity.StatusText.Text = "Status: Finished";
                Activity.ProgressBar.Indeterminate = false;
                Activity.ProgressBar.Progress = 100;
                Activity.SettingsButton.Enabled = true;
            }
            else if (s == "STOPPED" || s == "Login failed!")
            {
                Activity.StartButton.Text = "Start InstaShit";
                Activity.StartButton.Enabled = true;
                Activity.StatusText.Text = "Status: Stopped";
                Activity.ProgressBar.Indeterminate = false;
                Activity.ProgressBar.Progress = 0;
                Activity.SettingsButton.Enabled = true;
                Activity.AnswerCountText.Visibility = Android.Views.ViewStates.Gone;
            }
            else if (s == "SUCCESS")
            {
                MainActivity.Counter++;
                Activity.AnswerCountText.Text = "Answered questions: " + MainActivity.Counter;
            }
            MainActivity.Log.Add(s);
        }
    }
}

