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

        private CameraType currentType = CameraType.Camera;

        StateController()
        {
        }

        public static StateController StateControllerInstance
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

        public CameraType GetCameraType()
        {
            return StateControllerInstance.currentType;
        }

        public void SetCameraType(CameraType cameraType)
        {
            StateControllerInstance.currentType = cameraType;
        }

        
    }
}