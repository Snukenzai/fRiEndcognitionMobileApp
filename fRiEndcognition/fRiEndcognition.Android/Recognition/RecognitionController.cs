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
        public static void TrainAlbum(byte[] pic)
        {

            var httpWebRequest = Sender.createRequestHandler("POST", "train");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("files: ", pic, "entryid: ", DataController.Instance().name);
            }

            var response = Sender.getResponse(httpWebRequest);

        }

        public static string RecognisePic(byte[] pic)
        {
            var httpWebRequest = Sender.createRequestHandler("POST", "rec");
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("files: ", pic);
            }

            var response = Sender.getResponse(httpWebRequest);

            return response.ToString();

        }
}
}