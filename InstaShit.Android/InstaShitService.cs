using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using System.Threading.Tasks;
using InstaShitAndroid.InstaShitClasses;

namespace InstaShitAndroid
{
    [Service]
    public class InstaShitService : IntentService
    {
        public static volatile bool ShouldContinue = true;
        public InstaShitService() : base("InstaShitService")
        {
            
        }

        public void SendMessage(string messsage)
        {
            Intent intent = new Intent("com.InstaShit.Android.INFORMATION");
            intent.PutExtra("com.InstaShit.Android.INFORMATION_MESSAGE", messsage);
            LocalBroadcastManager.GetInstance(this).SendBroadcast(intent);
        }
        protected override async void OnHandleIntent(Intent intent)
        {
            Intent notificationIntent = new Intent(this, typeof(MainActivity));
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            var notification = new NotificationCompat.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText("InstaShit is now running.")
                .SetSmallIcon(Resource.Drawable.ic_stat_name)
                .SetContentIntent(pendingIntent)
                .SetOngoing(true);
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(1, notification.Build());
            InstaShit instaShit = new InstaShit();
            if (!ShouldContinue)
            {
                StopSelf();
                SendMessage("STOPPED");
                notificationManager.Cancel(1);
                return;
            }
            if (await instaShit.TryLoginAsync())
                SendMessage("Successfully logged in!");
            else
            {
                SendMessage("Login failed!");
                notificationManager.Cancel(1);
                return;
            }
            if (!ShouldContinue)
            {
                StopSelf();
                SendMessage("STOPPED");
                notificationManager.Cancel(1);
                return;
            }
            if (await instaShit.IsNewSessionAsync())
                SendMessage("Starting new session");
            else
                SendMessage("It looks like session was already started, continuing.");
            while (true)
            {
                if (!ShouldContinue)
                {
                    StopSelf();
                    SendMessage("STOPPED");
                    notificationManager.Cancel(1);
                    return;
                }
                var answer = await instaShit.GetAnswerAsync();
                if (answer == null)
                    break;
                if (!ShouldContinue)
                {
                    StopSelf();
                    SendMessage("STOPPED");
                    notificationManager.Cancel(1);
                    return;
                }
                int sleepTime = instaShit.SleepTime;
                SendMessage($"Sleeping... ({sleepTime}ms)");
                await Task.Delay(sleepTime);
                if (!ShouldContinue)
                {
                    StopSelf();
                    SendMessage("STOPPED");
                    notificationManager.Cancel(1);
                    return;
                }
                bool correctAnswer = answer.Word == answer.AnswerWord;
                string message;
                if (correctAnswer)
                    message = "Attempting to answer";
                else
                    message = $"Attempting to incorrectly answer(\"{answer.AnswerWord}\")";
                message += $" question about word \"{answer.Word}\" with id {answer.WordId}";
                SendMessage(message);
                if (await instaShit.TryAnswerQuestionAsync(answer))
                    SendMessage("SUCCESS");
                else
                {
                    SendMessage("Oops, something went wrong :( \n Please report this error to the bot's author.");
                    return;
                }
            }
            SendMessage("Session successfully finished.");
            var childResults = await instaShit.GetResultsAsync();
            if (childResults.PreviousMark != "NONE")
                SendMessage("Mark from previous week: " + childResults.PreviousMark);
            SendMessage("Days of work in this week: " + childResults.DaysOfWork);
            SendMessage("From extracurricular words: +" + childResults.ExtraParentWords);
            SendMessage("Teacher's words: " + childResults.TeacherWords);
            SendMessage("Extracurricular words in current edition: " + childResults.ParentWords);
            SendMessage("Mark as of today at least: " + childResults.CurrrentMark);
            SendMessage("Days until the end of this week: " + childResults.WeekRemainingDays);
            SendMessage("Saving session data...");
            instaShit.SaveSessionData();
            SendMessage("FINISHED");
            notification = new NotificationCompat.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetSmallIcon(Resource.Drawable.ic_stat_name)
                .SetContentText("Session successfully finished.")
                .SetContentIntent(pendingIntent);
            notificationManager.Notify(1, notification.Build());
        }
    }
}