﻿using System;
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
using Android.Support.V4.App;
using Android;
using Android.Content.PM;

namespace friendcognition.Droid
{
    [Activity(Label = "RegisterCamera", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegisterCamera : Activity, IFactory, CameraSource.IPictureCallback
    {
        private static readonly string TAG = "friendcognition";
        private CameraSource cameraSource = null;
        private CameraSourcePreview preview;
        private GraphicOverlay graphicOverlay;
        private ImageButton changeCamera;
        private ImageButton takePhoto;
        private ImageButton declinePhoto;
        private ImageButton confirmPhoto;
        Android.Graphics.Bitmap bitmapPicture;

        private static readonly int gms_code = 9001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegisterCamera);

            //ImageButton menu = FindViewById<ImageButton>(Resource.Id.Menu);
            changeCamera = FindViewById<ImageButton>(Resource.Id.ChangeCamera);
            takePhoto = FindViewById<ImageButton>(Resource.Id.TakePhoto);
            declinePhoto = FindViewById<ImageButton>(Resource.Id.DeclinePhoto);
            confirmPhoto = FindViewById<ImageButton>(Resource.Id.ConfirmPhoto);
            //menu.Click += OpenMenu; <--- for now, let's stay without menu for Register Camera
            changeCamera.Click += delegate (object sender, EventArgs e)
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
            takePhoto.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    cameraSource.TakePicture(null, this);
                    //cameraSource.Stop(); <--- just dont do it, dont stop it too early, he's to young to die and the whole program just goes nuts
                    takePhoto.Visibility = ViewStates.Gone;
                    changeCamera.Visibility = ViewStates.Gone;
                    declinePhoto.Visibility = ViewStates.Visible;
                    confirmPhoto.Visibility = ViewStates.Visible;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext, "Error: " + ex.ToString(), ToastLength.Long).Show();
                }
            };
            declinePhoto.Click += DeclinePhoto;
            confirmPhoto.Click += delegate (object sender, EventArgs e)
            {
                if (DataController.Instance.SavePicture(bitmapPicture))
                {
                    Intent i = new Intent(this, typeof(CameraActivity));

                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Denied)
                    {
                        ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 10);
                    }
                    else
                    {
                        StartActivity(i);
                    }
                }
                else
                {
                    DeclinePhoto(null, null);
                    Toast.MakeText(ApplicationContext, "Error: picture failed to be saved...", ToastLength.Long).Show();
                }
            };


            // vvv hide the unwanted buttons
            declinePhoto.Visibility = ViewStates.Gone;
            confirmPhoto.Visibility = ViewStates.Gone;


            preview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            CreateCameraSource(CameraFacing.Back);

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

        private void DeclinePhoto(object sender, EventArgs e)
        {
            takePhoto.Visibility = ViewStates.Visible;
            changeCamera.Visibility = ViewStates.Visible;
            declinePhoto.Visibility = ViewStates.Gone;
            confirmPhoto.Visibility = ViewStates.Gone;
            StartCameraSource();
        }

        public void OnPictureTaken(byte[] data)
        {
            bitmapPicture = Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length);
            cameraSource.Stop();
            // Data is saved in the bitmapPicture variable, we should store it in the database now
        }
    }
}