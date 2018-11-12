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
    [Activity(Label = "Main", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class Main : Activity
    {

        private static readonly int camera_code = 10;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            Button loginButton = FindViewById<Button>(Resource.Id.LoginB);
            loginButton.Click += DoLogin;

            Button registerButton = FindViewById<Button>(Resource.Id.RegisterB);
            registerButton.Click += DoRegister;

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, camera_code);
            }

        }
        private void DoLogin(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(Login));
            StartActivity(i);
        }
        private void DoRegister(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(Register));
            StartActivity(i);
        }

        public void onRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == camera_code)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    Toast.MakeText(this, "Camera permission granted", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "Camera permission denied", ToastLength.Long).Show();
                }
            }
        }
    }
}