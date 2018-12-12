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
using friendcognition.Droid.Camera;
using Java.Lang;
using static Android.Gms.Vision.MultiProcessor;

namespace friendcognition.Droid
{
    [Activity(Label = "CameraActivity", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CameraActivity : Activity, ICameraStreaming, IFactory, View.IOnTouchListener, CameraSource.IPictureCallback
    {

        private static readonly string TAG = "friendcognition";
        private CameraSource cameraSource = null;
        private CameraSourcePreview preview;
        private GraphicOverlay graphicOverlay;
        private ImageView imageview;
        private RelativeLayout layout;
        private Person person;

        private byte[] byteArrayPicture;
        private static readonly int gms_code = 9001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Camera);

            ImageButton menu = FindViewById<ImageButton>(Resource.Id.Menu);
            ImageButton changeCamera = FindViewById<ImageButton>(Resource.Id.ChangeCamera);
            imageview = FindViewById<ImageView>(Resource.Id.ImageViewCamera);
            layout = FindViewById<RelativeLayout>(Resource.Id.layoutcamera);
            preview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);
            person = DataController.Instance().currentPerson;

            imageview.Visibility = ViewStates.Invisible;

            menu.Click += delegate(object sender, EventArgs e) 
            {
                Intent i = new Intent(this, typeof(MenuActivity));
                StartActivity(i);
            };

            changeCamera.Click += delegate(object sender, EventArgs e) 
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
            };

            CreateCameraSource(CameraFacing.Back);

            DataController.Instance().openProfile = false;
            graphicOverlay.SetOnTouchListener(this);
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

        public bool OnTouch(View v, MotionEvent e)
        {
            float x = e.GetX();
            float y = e.GetY();

            if (e.Action == MotionEventActions.Down)
            {
                DataController.Instance().TouchEventDown(x, y);
            }
            else if (e.Action == MotionEventActions.Up)
            {
                DataController.Instance().TouchEventUp();
                if (DataController.Instance().openProfile == true)
                {
                    cameraSource.TakePicture(null, this);
                }
            }

            return true;
        }

        public void CreateCameraSource(CameraFacing cameraFacing)
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

        public void StartCameraSource()
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

        public void OnPictureTaken(byte[] data)
        {
            DataController.Instance().openProfile = false;

            Android.Graphics.Bitmap bm = ImageController.ByteArrayToBitmap(data);
            imageview.SetImageBitmap(bm);
            bm = ImageController.LoadBitmapFromView(imageview);
            bm = ImageController.ResizeBitmap(bm);
            byteArrayPicture = ImageController.BitmapToByteArray(bm);

            string result = Recognition.RecognitionController.RecognisePic(byteArrayPicture);
            person.Name = result;
            Intent i = new Intent(this, typeof(ProfileActivity));
            StartActivity(i);
        }
    }
}