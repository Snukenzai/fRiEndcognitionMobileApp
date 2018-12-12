using System;
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
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://35.228.22.247/" + uriend);
          
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = requestType;

            return httpWebRequest;
        }

        public static string getResponse(HttpWebRequest httpWebRequest)
        {
            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return reader.ReadToEnd();
            }
        }
    }
}