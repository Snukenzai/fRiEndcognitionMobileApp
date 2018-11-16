using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Vision;
using Android.Gms.Vision.Faces;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Resource.Id;
using static Android.Gms.Vision.MultiProcessor;
using static friendcognition.Droid.StateController;

namespace friendcognition.Droid
{
    [Activity(Label = "RegisterCamera", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegisterCamera : Activity, IFactory
    {
        private static readonly string TAG = "friendcognition";
        private CameraSource cameraSource = null;
        private CameraSourcePreview preview;
        private GraphicOverlay graphicOverlay;
        private static CameraType cameraType = CameraType.RegisterCamera;

        private static readonly int gms_code = 9001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegisterCamera);

            ImageButton menu = FindViewById<ImageButton>(Resource.Id.Menu);
            ImageButton changeCamera = FindViewById<ImageButton>(Resource.Id.ChangeCamera);
            ImageButton takePhoto = FindViewById<ImageButton>(Resource.Id.TakePhoto);
            //menu.Click += OpenMenu; <--- for now, let's stay without menu for Register Camera
            changeCamera.Click += ChangeCameraFacing;
            takePhoto.Click += TakePhoto;

            preview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            CreateCameraSource(CameraFacing.Back);

            StateController.StateControllerInstance.SetCameraType(cameraType);

        }

        private void TakePhoto(object sender, EventArgs e)
        {
            cameraSource.TakePicture(null, null);
        }

        /*
        private void OpenMenu(object sender, EventArgs e)
        {
            StateControllerInstance.SetCameraType(cameraType);
            Intent i = new Intent(this, typeof(Menu));
            StartActivity(i);
        }
        */

        private void ChangeCameraFacing(object sender, EventArgs e)
        {
            if (cameraSource != null)
            {
                cameraSource.Release();
                if (cameraSource.CameraFacing == CameraFacing.Back)
                {
                    CreateCameraSource(CameraFacing.Front);
                }
                else
                {
                    CreateCameraSource(CameraFacing.Back);
                }
                StartCameraSource();
            }
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

        private void CreateCameraSource(CameraFacing cameraFacing)
        {

            FaceDetector detector = new FaceDetector.Builder(this).Build();

            detector.SetProcessor(new MultiProcessor.Builder(this).Build());

            if (!detector.IsOperational)
            {
                Log.Warn(TAG, "Face detector isn't configured yet");
            }

            cameraSource = new CameraSource.Builder(this, detector)
                .SetRequestedPreviewSize(640, 480)
                .SetFacing(cameraFacing)
                .SetAutoFocusEnabled(true)
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

        public Tracker Create(Java.Lang.Object item)
        {
            return new friendcognition.Droid.FaceDetection(graphicOverlay, cameraSource);
        }
    }
}