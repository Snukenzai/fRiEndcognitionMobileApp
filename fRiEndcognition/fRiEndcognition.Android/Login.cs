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

namespace friendcognition.Droid
{
    [Activity(Label = "Login", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class Login : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            Button logined = FindViewById<Button>(Resource.Id.FinalLogin);
            logined.Click += Logined;
        }
        private void Logined(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(Camera));
            StartActivity(i);
        }
    }
}