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
using Android.Util;
using Android.Gms.Vision;
using Android.Graphics;
using Android.Content.Res;
using Android.Hardware;

namespace friendcognition.Droid
{
    public class CameraSourcePreview : ViewGroup, ISurfaceHolderCallback
    {
        private static readonly String TAG = "CameraSourcePreview";

        private Context context;
        private SurfaceView surfaceView;
        private bool startRequested;
        private bool surfaceAvailable;
        private CameraSource cameraSource;
        private GraphicOverlay overlay;


        public CameraSourcePreview(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.context = context;
            startRequested = false;
            surfaceAvailable = false;

            surfaceView = new SurfaceView(context);
            surfaceView.Holder.AddCallback(this);

            AddView(surfaceView);
        }


        public void Start(CameraSource cameraSource)
        {
            if (cameraSource == null)
            {
                Stop();
            }

            this.cameraSource = cameraSource;

            if (cameraSource != null)
            {
                startRequested = true;
                StartIfReady();
            }
        }

        public void Start(CameraSource cameraSource, GraphicOverlay overlay)
        {
            this.overlay = overlay;
            Start(cameraSource);
        }

        public void Stop()
        {
            if (cameraSource != null)
            {
                cameraSource.Stop();
            }
        }

        public void Release()
        {
            if (cameraSource != null)
            {
                cameraSource.Release();
                cameraSource = null;
            }
        }
        private void StartIfReady()
        {
            if (startRequested && surfaceAvailable)
            {
                cameraSource.Start(surfaceView.Holder);
                if (overlay != null)
                {
                    var size = cameraSource.PreviewSize;
                    var min = Math.Min(size.Width, size.Height);
                    var max = Math.Max(size.Width, size.Height);
                    if (IsPortraitMode())
                    {
                        overlay.SetCameraInfo(min, max, cameraSource.CameraFacing);
                    }
                    else
                    {
                        overlay.SetCameraInfo(max, min, cameraSource.CameraFacing);
                    }
                    overlay.Clear();
                }
                startRequested = false;
            }
        }

        private bool IsPortraitMode()
        {
            var orientation = context.Resources.Configuration.Orientation;
            if (orientation == Android.Content.Res.Orientation.Landscape)
            {
                return false;
            }
            if (orientation == Android.Content.Res.Orientation.Portrait)
            {
                return true;
            }

            Log.Debug(TAG, "isPortraitMode returning false by default");
            return false;
        }



        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            surfaceAvailable = true;


            try
            {
                StartIfReady();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Could not start camera source.", e);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            surfaceAvailable = false;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int width = 320;
            int height = 240;
            if (cameraSource != null)
            {
                var size = cameraSource.PreviewSize;
                if (size != null)
                {
                    width = size.Width;
                    height = size.Height;
                }
            }

            // Swap width and height sizes when in portrait, since it will be rotated 90 degrees
            if (IsPortraitMode())
            {
                int tmp = width;
                width = height;
                height = tmp;
            }

            int layoutWidth = r - l;
            int layoutHeight = b - t;

            // Computes height and width for potentially doing fit width.
            int childWidth = layoutWidth;
            int childHeight = (int)(((float)layoutWidth / (float)width) * height);

            // If height is too tall using fit width, does fit height instead.
            if (childHeight < layoutHeight)
            {
                childHeight = layoutHeight;
                childWidth = (int)(((float)layoutHeight / (float)height) * width);
            }

            for (int i = 0; i < ChildCount; ++i)
            {

                GetChildAt(i).Layout(0, 0, childWidth, childHeight);
            }

            try
            {
                StartIfReady();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Could not start camera source.", e);
            }
        }
    }

}