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
    [Activity(Label = "ProfileActivity", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class ProfileActivity : Activity
    {

        private TextView profileName;
        private ImageView profileImage;
        private Button changePicture;
        private int id, percentage;
        private Person currentPerson;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Profile);

            profileName = FindViewById<TextView>(Resource.Id.ProfileName);
            profileImage = FindViewById<ImageView>(Resource.Id.ProfileImage);
            currentPerson = DataController.Instance().currentPerson;
            setId();
        }

        public void setId()
        {
            profileName.Text = currentPerson.Name + " " + currentPerson.Surname;
            profileImage.SetImageBitmap(DataController.Instance().ByteArrayToBitmap(currentPerson.Picture));
        }
    }
}