﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace friendcognition.Droid.HTTP
{
    class Sender
    {
        public static HttpWebRequest createRequestHandler(string requestType, string uriend)
        {
            //change URI to our web api ip
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://35.228.182.137/" + uriend);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = requestType;

            return httpWebRequest;
        }

        public static HttpWebResponse getResponse(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
            return httpResponse;
        }
    }
}