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

        private TextView profileName, profileSurname;
        private ImageView profileImage;
        private Button changePicture;
        private int id, percentage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Profile);

            profileName = FindViewById<TextView>(Resource.Id.ProfileName);
            profileSurname = FindViewById<TextView>(Resource.Id.ProfileSurname);
            profileImage = FindViewById<ImageView>(Resource.Id.ProfileImage);
            changePicture = FindViewById<Button>(Resource.Id.changePicture);
            setId();
        }

        public void setId()
        {
            profileSurname.Visibility = ViewStates.Gone;

            profileName.Text = DataController.Instance().name;
        }
    }
}