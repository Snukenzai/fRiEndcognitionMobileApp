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

namespace friendcognition.Droid.Recognition
{
    class RecognitionController
    { 

        public static void UpdateAlbum(Android.Graphics.Bitmap bmp)
        {

            var httpWebRequest = Sender.createRequestHandler("POST");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("Update album", bmp);
            }

            var response = Sender.getResponse(httpWebRequest);

        }

        public void TrainAlbum()
        {
            //Could be implemented if we add more than one pic
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