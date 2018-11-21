using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace friendcognition.Droid.Camera
{
    interface ICameraStreaming
    {

        void CreateCameraSource(CameraFacing cameraFacing);

        void StartCameraSource();

    }
}