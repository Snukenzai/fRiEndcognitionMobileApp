﻿using System;
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
    [Activity(Label = "Register")]
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
            StartActivity(i);
        }
    }
}