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
using static System.Net.Mime.MediaTypeNames;

namespace friendcognition.Droid.Recognition
{
    class RecognitionController
    {
        public static void TrainAlbum(byte[] pic, string id)
        {

            var httpWebRequest = Sender.createRequestHandler("POST", "train");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                
                streamWriter.Write("entryid=" + id +
                    "&files=" + Convert.ToBase64String(pic));
            }

            var response = Sender.getResponse(httpWebRequest);

        }

        public static string RecognisePic(byte[] pic)
        {
            var httpWebRequest = Sender.createRequestHandler("POST", "rec");
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("files=" + Convert.ToBase64String(pic));
            }

            string response = Sender.getResponse(httpWebRequest);

            return response;

        }
}
}