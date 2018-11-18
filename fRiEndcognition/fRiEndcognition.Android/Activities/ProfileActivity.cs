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
    [Activity(Label = "ProfileActivity", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class ProfileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Profile);
        }
    }
}