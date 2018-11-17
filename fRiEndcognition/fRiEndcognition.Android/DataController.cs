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

namespace friendcognition.Droid
{
    class DataController
    {
        private static DataController instance = null;
        private static readonly object padlock = new object();

        DataController()
        {
        }

        public static DataController Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DataController();
                    }
                    return instance;
                }
            }
        }

        public bool SavePicture(Android.Graphics.Bitmap bitmapPicture)
        {
            // TO BE IMPLEMENTED, THE DATABASE LOGIC

            return true;
        }
    }
}