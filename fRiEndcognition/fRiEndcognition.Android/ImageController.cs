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
using System.IO;
using Android.Graphics;
using Plugin.Screenshot;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace friendcognition.Droid
{
    class ImageController
    {

        public static byte[] BitmapToByteArray(Android.Graphics.Bitmap bitmapData)
        {
            byte[] byteArrayData;
            using (var stream = new MemoryStream())
            {
                bitmapData.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 30, stream);
                byteArrayData = stream.ToArray();
            }
            return byteArrayData;
        }

        public static Bitmap ByteArrayToBitmap(byte[] byteArrayData)
        {
            return BitmapFactory.DecodeByteArray(byteArrayData, 0, byteArrayData.Length);
        }

        public static Bitmap LoadBitmapFromView(Android.Views.View v)
        {
            Bitmap b = Bitmap.CreateBitmap(v.Width, v.Height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(b);
            v.Layout(0, 0, v.Width, v.Height);
            v.Draw(c);
            return b;
        }

        public static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            var bitmapScalled = Bitmap.CreateScaledBitmap(bitmap, 400, 700, true);
            bitmap.Recycle();
            return bitmapScalled;
        }

    }
}