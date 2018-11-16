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
using static friendcognition.Droid.StateController;

namespace friendcognition.Droid
{
    [Activity(Label = "Menu", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class Menu : Activity
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
            Intent i = new Intent(this, typeof(Profile));
            StartActivity(i);
        }
        private void OpenMain(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
        private void OpenCamera(object sender, EventArgs e)
        {
            CameraType cameraType = StateControllerInstance.GetCameraType();
            if (cameraType == CameraType.Camera)
            {
                Intent i = new Intent(this, typeof(Camera));
                StartActivity(i);
            }
            else if (cameraType == CameraType.RegisterCamera)
            {
                Intent i = new Intent(this, typeof(RegisterCamera));
                StartActivity(i);
            }
            
        }
    }
}