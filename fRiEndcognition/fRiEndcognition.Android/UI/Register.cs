﻿using System;
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
    [Activity(Label = "Register", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class Register : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            Button registered = FindViewById<Button>(Resource.Id.FinalRegister);
            registered.Click += Registered;
        }
        private void Registered(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(Camera));

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 10);
            }
            else
            {
                StartActivity(i);
            }
        }

        public void onRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == 10)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    Intent i = new Intent(this, typeof(Camera));
                    StartActivity(i);
                }
            }
        }
    }
}