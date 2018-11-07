using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace friendcognition.Droid
{
    [Activity(Label = "MainActivity", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            Button loginButton = FindViewById<Button>(Resource.Id.LoginB);
            loginButton.Click += DoLogin;

            Button registerButton = FindViewById<Button>(Resource.Id.RegisterB);
            registerButton.Click += DoRegister;

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
    }
}
