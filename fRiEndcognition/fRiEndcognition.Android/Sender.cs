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
using System.Net.Http;
using System.Net;
using System.IO;

namespace friendcognition.Droid
{
    class Sender
    {       
        
        public static HttpWebRequest createRequestHandler(string requestType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://url");
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