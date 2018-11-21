using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using friendcognition.Droid.HTTP;

namespace friendcognition.Droid.Recognition
{
    class RecognitionController
    {
        public static void TrainAlbum(Android.Graphics.Bitmap bmp)
        {

            var httpWebRequest = Sender.createRequestHandler("POST");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(bmp);
            }

            var response = Sender.getResponse(httpWebRequest);

        }

        public static string RecognisePic(Android.Graphics.Bitmap bmp)
        {
            var httpWebRequest = Sender.createRequestHandler("POST");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("RecognisePic", bmp);
            }

            var response = Sender.getResponse(httpWebRequest);

            return response.ToString();

        }
}
}