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
    partial class StateController
    {

        private static StateController instance = null;
        private static readonly object padlock = new object();

        private CameraType currentType;

        StateController()
        {
        }

        public static StateController Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new StateController();
                    }
                    return instance;
                }
            }
        }

        
    }
}