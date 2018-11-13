using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

namespace friendcognition.Droid
{
    [Activity(Label = "friendcognition", Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {

        private SurfaceView cameraView;
        private TextView textView;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Main);
            //LoadApplication(new App());
        }
    }
}