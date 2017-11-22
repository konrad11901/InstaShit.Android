using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using InstaShitCore;

namespace InstaShitAndroid
{
    [Activity(Label = "IntelligentMistakesData")]
    public class IntelligentMistakesDataActivity : ListActivity
    {
        List<List<IntelligentMistakesDataEntry>> mistakesData;
        MyAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mistakesData = JsonConvert.DeserializeObject<List<List<IntelligentMistakesDataEntry>>>(Intent.Extras.GetString("mistakesdata"));
            UpdateList();
            this.ListView.ItemClick += (sender, e) => 
            {
                var intent = new Intent(this, typeof(IntelligentMistakesSessionDataActivity));
                intent.PutExtra("session", JsonConvert.SerializeObject(mistakesData[e.Position]));
                intent.PutExtra("sessionid", e.Position);
                Console.WriteLine(e.Position.ToString());
                StartActivityForResult(intent, 1);
            };
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    int id = data.GetIntExtra("sessionid", -1);
                    var newSession = JsonConvert.DeserializeObject<List<IntelligentMistakesDataEntry>>(data.GetStringExtra("session"));
                    if (newSession.Count != 0)
                        mistakesData[id] = newSession;
                    else
                        mistakesData.RemoveAt(id);
                    UpdateList();
                }
            }
            else if (requestCode == 2)
            {
                if(resultCode == Result.Ok)
                {
                    var newSession = JsonConvert.DeserializeObject<List<IntelligentMistakesDataEntry>>(data.GetStringExtra("session"));
                    if(newSession.Count != 0)
                    {
                        mistakesData.Add(newSession);
                        UpdateList();
                    }
                }
            }
        }
        private void UpdateList()
        {
            var items = new List<Tuple<string, string>>();
            for (int i = 1; i <= mistakesData.Count(); i++)
                items.Add(new Tuple<string, string>($"Session {i.ToString()}", $"{mistakesData[i - 1].Count()} entries"));
            adapter = new MyAdapter(this, Android.Resource.Layout.SimpleListItem2, Android.Resource.Id.Text1, items);
            this.ListAdapter = adapter;
        }
        public override void OnBackPressed()
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("mistakesdata", JsonConvert.SerializeObject(mistakesData));
            SetResult(Result.Ok, resultIntent);
            Finish();
            base.OnBackPressed();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if(id == Resource.Id.menu_add)
            {
                var intent = new Intent(this, typeof(IntelligentMistakesSessionDataActivity));
                intent.PutExtra("session", "[]");
                intent.PutExtra("sessionid", -1);
                StartActivityForResult(intent, 2);
            }
            else if(id == Resource.Id.menu_delete)
            {
                if(mistakesData.Count() != 0)
                {
                    mistakesData.RemoveAt(mistakesData.Count() - 1);
                    UpdateList();
                }
            }
            return base.OnOptionsItemSelected(item);
        }
    }
    public class MyAdapter : ArrayAdapter<Tuple<string, string>>
    {
        private IList<Tuple<string, string>> _objects;
        public MyAdapter (Context context, int resource, int textViewResourceId, IList<Tuple<string, string>> objects) : base(context, resource, textViewResourceId, objects)
        {
            _objects = objects;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = base.GetView(position, convertView, parent);
            TextView text1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            TextView text2 = view.FindViewById<TextView>(Android.Resource.Id.Text2);
            text1.Text = _objects[position].Item1;
            text2.Text = _objects[position].Item2;
            return view;
        }
    }
}
