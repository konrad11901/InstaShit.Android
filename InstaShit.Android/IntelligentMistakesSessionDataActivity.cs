using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using InstaShitCore;
using Newtonsoft.Json;

namespace InstaShitAndroid
{
    [Activity(Label = "IntelligentMistakesSessionData")]
    public class IntelligentMistakesSessionDataActivity : ListActivity
    {
        List<IntelligentMistakesDataEntry> sessionData;
        ArrayAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            sessionData = JsonConvert.DeserializeObject<List<IntelligentMistakesDataEntry>>(Intent.Extras.GetString("session"));
            UpdateList();
            this.ListView.ItemClick += (sender, e) => 
            {
                var intent = new Intent(this, typeof(IntelligentMistakesDataEntryActivity));
                intent.PutExtra("entry", JsonConvert.SerializeObject(sessionData[e.Position]));
                intent.PutExtra("entryid", e.Position);
                StartActivityForResult(intent, 1);
            };

        }
        private void UpdateList()
        {
            var items = new List<string>();
            for (int i = 1; i <= sessionData.Count(); i++)
                items.Add($"Entry {i.ToString()}");
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            this.ListAdapter = adapter;
        }
        public override void OnBackPressed()
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("session", JsonConvert.SerializeObject(sessionData));
            resultIntent.PutExtra("sessionid", Intent.Extras.GetInt("sessionid"));
            SetResult(Result.Ok, resultIntent);
            Finish();
            base.OnBackPressed();
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if(requestCode == 1)
            {
                if(resultCode == Result.Ok)
                {
                    int id = data.GetIntExtra("entryid", -1);
                    var newEntry = JsonConvert.DeserializeObject<IntelligentMistakesDataEntry>(data.GetStringExtra("entry"));
                    if (newEntry.MaxNumberOfMistakes != 0 && newEntry.RiskPercentage != 0)
                        sessionData[id] = newEntry;
                    else
                        sessionData.RemoveAt(id);
                    UpdateList();

                }
            }
            else if(requestCode == 2)
            {
                if(resultCode == Result.Ok)
                {
                    var newEntry = JsonConvert.DeserializeObject<IntelligentMistakesDataEntry>(data.GetStringExtra("entry"));
                    if(newEntry.MaxNumberOfMistakes != 0 && newEntry.RiskPercentage != 0)
                    {
                        sessionData.Add(newEntry);
                        UpdateList();
                    }
                }
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_add)
            {
                var intent = new Intent(this, typeof(IntelligentMistakesDataEntryActivity));
                intent.PutExtra("entryid", -1);
                StartActivityForResult(intent, 2);
            }
            else if (id == Resource.Id.menu_delete)
            {
                if(sessionData.Count() != 0)
                {
                    sessionData.RemoveAt(sessionData.Count() - 1);
                    UpdateList();
                }
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
