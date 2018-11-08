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
    [Activity(Label = "Camera", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class Camera : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Camera);

            ImageButton menu = FindViewById<ImageButton>(Resource.Id.Menu);
            menu.Click += OpenMenu;
        }
        private void OpenMenu(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(Menu));
            StartActivity(i);
        }
    }
}