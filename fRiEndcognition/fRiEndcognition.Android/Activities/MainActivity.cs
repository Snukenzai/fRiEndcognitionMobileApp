using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Android;

namespace friendcognition.Droid
{
    [Activity(Label = "MainActivity", Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
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
            Intent i = new Intent(this, typeof(LoginActivity));
            StartActivity(i);
        }
        private void DoRegister(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(RegisterActivity));
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
