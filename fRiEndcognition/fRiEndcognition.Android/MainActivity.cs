using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android;
using Android.Util;
using Android.Support.Design.Widget;
using Android.Gms.Vision.Faces;
using Android.Gms.Vision;
using Java.Lang;
using Android.Gms.Common;
using Android.Support.V7.App;
using Xamarin.Essentials;

namespace friendcognition.Droid
{
    [Activity(Label = "friendcognition", Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        private static readonly string TAG = "friendcognition";
        private CameraSource cameraSource = null;
        private CameraSourcePreview preview;
        private GraphicOverlay graphicOverlay;
        
        private static readonly int gms_code = 9001;
        private static readonly int camera_code = 10;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            preview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                CreateCameraSource();                
            }
            else { RequestCameraPermission(); }

        }

        private void RequestCameraPermission()
        {
            Log.Warn(TAG, "Camera permission is not granted. Requesting permission");

            var permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, camera_code);
                return;
            }

            Snackbar.Make(graphicOverlay, "Camera Permission", Snackbar.LengthIndefinite)
                .SetAction("Ok", (o) => { ActivityCompat.RequestPermissions(this, permissions, camera_code); })
                .Show();
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartCameraSource();
        }

        protected override void OnPause()
        {
            base.OnPause();
            preview.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (cameraSource != null)
            {
                cameraSource.Release();
            }
        }

        private void CreateCameraSource()
        {

            FaceDetector detector = new FaceDetector.Builder(this).Build();

           //detector.SetProcessor(new MultiProcessor.Builder(this).Build());

            if (!detector.IsOperational)
            {
                Log.Warn(TAG, "Face detector isn't configured yet");
            }

            //var metrics = DeviceDisplay.ScreenMetrics;
            //int width = (int)metrics.Width;
            //int height = (int)metrics.Height;

            cameraSource = new CameraSource.Builder(this, detector)
                .SetRequestedPreviewSize(640,480)
                .SetFacing(CameraFacing.Back)
                .SetRequestedFps(30.0f)
                .Build();

            
        }

        private void StartCameraSource()
        {

            // check that the device has play services available.
            int code = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (code != ConnectionResult.Success)
            {
                Dialog dlg = GoogleApiAvailability.Instance.GetErrorDialog(this, code, gms_code);
                dlg.Show();
            }

            if (cameraSource != null)
            {
                try
                {
                    preview.Start(cameraSource, graphicOverlay);
                }
                catch (System.Exception e)
                {
                    Log.Error(TAG, "Unable to start camera", e);
                    cameraSource.Release();
                    cameraSource = null;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode != camera_code)
            {
                Log.Debug(TAG, "Got unexpected permission result: " + requestCode);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                return;
            }

            if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
            {
                Log.Debug(TAG, "Camera permission granted - initialize the camera source");
                CreateCameraSource();
                return;
            }

            Log.Error(TAG, "Permission not granted: results len = " + grantResults.Length +
                    " Result code = " + (grantResults.Length > 0 ? grantResults[0].ToString() : "(empty)"));

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("friendcognition")
                .SetMessage(Resource.String.no_camera_permission)
                .SetPositiveButton(Resource.String.ok, (o, e) => Finish())
                .Show();

        }
    }
}