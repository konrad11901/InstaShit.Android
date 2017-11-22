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
        public TextView statusText;
        public ProgressBar progressBar;
        public Button startButton;
        public TextView answerCountText;
        public Button logButton;
        public Button settingsButton;
        public static readonly List<string> log = new List<string>();
        Receiver receiver;
        public static int counter = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            statusText = FindViewById<TextView>(Resource.Id.statusTextView);
            startButton = FindViewById<Button>(Resource.Id.startButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            answerCountText = FindViewById<TextView>(Resource.Id.answerCountText);
            logButton = FindViewById<Button>(Resource.Id.logButton);
            settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
            receiver = new Receiver();
            receiver.activity = this;
            if(savedInstanceState != null)
                statusText.Text = savedInstanceState.GetString("statusText", "Status: Stopped");
            if(statusText.Text == "Status: Stopped")
            {
                startButton.Text = "Start InstaShit";
                startButton.Enabled = true;
                progressBar.Indeterminate = false;
                progressBar.Progress = 0;
            }
            else if (statusText.Text == "Status: Running")
            {
                startButton.Text = "Stop InstaShit";
                startButton.Enabled = true;
                progressBar.Indeterminate = true;
                settingsButton.Enabled = false;
                answerCountText.Visibility = Android.Views.ViewStates.Visible;
            }
            else if (statusText.Text == "Status: Stopping")
            {
                startButton.Text = "Stopping...";
                startButton.Enabled = false;
                settingsButton.Enabled = false;
                progressBar.Indeterminate = true;
                answerCountText.Visibility = Android.Views.ViewStates.Visible;
            }
            else if (statusText.Text == "Status: Finished")
            {
                startButton.Text = "Start InstaShit";
                startButton.Enabled = true;
                statusText.Text = "Status: Stopped";
                progressBar.Indeterminate = false;
                progressBar.Progress = 100;
            }
            answerCountText.Text = "Answered questions: " + counter;
            logButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(LogActivity));
                intent.PutStringArrayListExtra("log", log);
                StartActivity(intent);
            };
            settingsButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
            };
            startButton.Click += (sender, e) =>
            {
                if (startButton.Text == "Start InstaShit")
                {
                    log.Clear();
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
                        log.Add("Can't find settings file!");
                        return;
                    }
                    InstaShitService.shouldContinue = true;
                    startButton.Text = "Stop InstaShit";
                    statusText.Text = "Status: Running";
                    settingsButton.Enabled = false;
                    progressBar.Indeterminate = true;
                    Intent instaShitIntent = new Intent(this, typeof(InstaShitService));
                    counter = 0;
                    answerCountText.Text = "Answered questions: " + counter;
                    answerCountText.Visibility = Android.Views.ViewStates.Visible;
                    StartService(instaShitIntent);
                }
                else
                {
                    InstaShitService.shouldContinue = false;
                    statusText.Text = "Status: Stopping";
                    startButton.Text = "Stopping...";
                    startButton.Enabled = false;
                }
            };
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(receiver, new IntentFilter("com.InstaShit.Android.INFORMATION"));
        }
        protected override void OnDestroy()
        {
            LocalBroadcastManager.GetInstance(this).UnregisterReceiver(receiver);
            base.OnDestroy();
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("statusText", statusText.Text);
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
        public MainActivity activity { get; set; }
        public override void OnReceive(Context context, Intent intent)
        {
            string s = intent.GetStringExtra("com.InstaShit.Android.INFORMATION_MESSAGE");
            if (s == "FINISHED")
            {
                activity.startButton.Text = "Start InstaShit";
                activity.statusText.Text = "Status: Finished";
                activity.progressBar.Indeterminate = false;
                activity.progressBar.Progress = 100;
                activity.settingsButton.Enabled = true;
            }
            else if (s == "STOPPED" || s == "Login failed!")
            {
                activity.startButton.Text = "Start InstaShit";
                activity.startButton.Enabled = true;
                activity.statusText.Text = "Status: Stopped";
                activity.progressBar.Indeterminate = false;
                activity.progressBar.Progress = 0;
                activity.settingsButton.Enabled = true;
                activity.answerCountText.Visibility = Android.Views.ViewStates.Gone;
            }
            else if (s == "SUCCESS")
            {
                MainActivity.counter++;
                activity.answerCountText.Text = "Answered questions: " + MainActivity.counter;
            }
            MainActivity.log.Add(s);
        }
    }
}

