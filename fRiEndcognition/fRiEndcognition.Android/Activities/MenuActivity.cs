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
    [Activity(Label = "MenuActivity", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class MenuActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Menu);

            Button logout = FindViewById<Button>(Resource.Id.logoutButton);
            logout.Click += OpenMain;

            Button profile = FindViewById<Button>(Resource.Id.profileButton);
            profile.Click += OpenProfile;

            Button camera = FindViewById<Button>(Resource.Id.cameraButton);
            camera.Click += OpenCamera;
        }
        private void OpenProfile(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ProfileActivity));
            StartActivity(i);
        }
        private void OpenMain(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
        private void OpenCamera(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(CameraActivity));
            StartActivity(i);

        }
    }
}