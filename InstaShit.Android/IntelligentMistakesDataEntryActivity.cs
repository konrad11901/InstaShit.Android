
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Widget;
using InstaShitCore;
using Java.Lang;
using Newtonsoft.Json;

namespace InstaShitAndroid
{
    [Activity(Label = "IntelligentMistakesDataEntryActivity")]
    public class IntelligentMistakesDataEntryActivity : Activity
    {
        EditText riskPercentage;
        EditText maxMistakes;
        Button saveButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.IntelligentMistakesDataEntry);
            riskPercentage = FindViewById<EditText>(Resource.Id.riskPercentage);
            riskPercentage.SetFilters(new IInputFilter[] {new InputFilterMinMax(0, 100)});
            maxMistakes = FindViewById<EditText>(Resource.Id.maxMistakes);
            maxMistakes.SetFilters(new IInputFilter[] { new InputFilterMinMax(-1, 100) });
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            if (Intent.Extras.GetInt("entryid") != -1)
            {
                var entry = JsonConvert.DeserializeObject<IntelligentMistakesDataEntry>(Intent.Extras.GetString("entry", ""));
                riskPercentage.Text = entry.RiskPercentage.ToString();
                maxMistakes.Text = entry.MaxNumberOfMistakes.ToString();
            }
            saveButton.Click += (sender, e) =>
            {
                var localEntry = new IntelligentMistakesDataEntry()
                {
                    RiskPercentage = (riskPercentage.Text != "" ? Int32.Parse(riskPercentage.Text) : 0),
                    MaxNumberOfMistakes = (maxMistakes.Text != "" ? Int32.Parse(maxMistakes.Text) : 0)
                };
                Intent resultIntent = new Intent();
                resultIntent.PutExtra("entry", JsonConvert.SerializeObject(localEntry));
                resultIntent.PutExtra("entryid", Intent.Extras.GetInt("entryid"));
                SetResult(Result.Ok, resultIntent);
                Finish();
            };
        }
    }
    public class InputFilterMinMax : Java.Lang.Object, IInputFilter
    {
        private int min, max;
        public InputFilterMinMax(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            try
            {
                int input = Int32.Parse(dest.ToString().Insert(dstart, source.ToString()));
                if (IsInRange(min, max, input))
                    return null;
            }
            catch (System.Exception)
            {
               
            }
            return new Java.Lang.String(string.Empty);
        }
        private bool IsInRange(int a, int b, int c)
        {
            return b > a ? c >= a && c <= b : c >= b && c <= a;
        }
    }
}
