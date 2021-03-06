﻿using Android.App;
using Android.OS;
using Android.Widget;

namespace InstaShitAndroid
{
    [Activity(Label = "Log")]
    public class LogActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var log = Intent.Extras.GetStringArrayList("log") ?? new string[0];
            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, log);
        }
    }
}