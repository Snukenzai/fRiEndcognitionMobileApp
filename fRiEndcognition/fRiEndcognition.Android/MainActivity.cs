using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Vision;

namespace friendcognition.Droid
{
    [Activity(Label = "friendcognition", Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {

        private SurfaceView cameraView;
        private TextView textView;

        private CameraSource cameraSource;
        private const int RequestCameraPermissionID = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Main);
            //LoadApplication(new App());

            cameraView = FindViewByID<SurfaceView>(Resource.ID.surface_view);
            textView = FindViewByID<TextView>(Resource.ID.text_view);

           
        }
    }
}