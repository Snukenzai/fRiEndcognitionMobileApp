using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
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

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 10);
            }
            if(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                StartActivity(i);
            }
        }
    }
}