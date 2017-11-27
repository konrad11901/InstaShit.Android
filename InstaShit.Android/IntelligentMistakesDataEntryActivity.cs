
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
        EditText _riskPercentage;
        EditText _maxMistakes;
        Button _saveButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.IntelligentMistakesDataEntry);
            _riskPercentage = FindViewById<EditText>(Resource.Id.riskPercentage);
            _riskPercentage.SetFilters(new IInputFilter[] {new InputFilterMinMax(0, 100)});
            _maxMistakes = FindViewById<EditText>(Resource.Id.maxMistakes);
            _maxMistakes.SetFilters(new IInputFilter[] { new InputFilterMinMax(-1, 100) });
            _saveButton = FindViewById<Button>(Resource.Id.saveButton);
            if (Intent.Extras.GetInt("entryid") != -1)
            {
                var entry = JsonConvert.DeserializeObject<IntelligentMistakesDataEntry>(Intent.Extras.GetString("entry", ""));
                _riskPercentage.Text = entry.RiskPercentage.ToString();
                _maxMistakes.Text = entry.MaxNumberOfMistakes.ToString();
            }
            _saveButton.Click += (sender, e) =>
            {
                var localEntry = new IntelligentMistakesDataEntry()
                {
                    RiskPercentage = (_riskPercentage.Text != "" ? Int32.Parse(_riskPercentage.Text) : 0),
                    MaxNumberOfMistakes = (_maxMistakes.Text != "" ? Int32.Parse(_maxMistakes.Text) : 0)
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
                // ignored
            }
            return new Java.Lang.String(string.Empty);
        }
        private bool IsInRange(int a, int b, int c)
        {
            return b > a ? c >= a && c <= b : c >= b && c <= a;
        }
    }
}
