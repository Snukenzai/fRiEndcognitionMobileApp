using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
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
        private int id, percentage;
        private Person currentPerson;
        public static readonly int PickImageId = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Profile);

            Button changePicture = FindViewById<Button>(Resource.Id.ChangeProfilePicture);
            changePicture.Click += delegate (object sender, EventArgs e)
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
            };

            profileName = FindViewById<TextView>(Resource.Id.ProfileName);
            profileImage = FindViewById<ImageView>(Resource.Id.ProfileImage);
            currentPerson = DataController.Instance().currentPerson;
            setId();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                profileImage.SetImageURI(uri);
                byte[] picture = convertImageToByte(uri);
                DataController.Instance().UpdateLocalDatabase(picture);
            }
        }

        private byte[] convertImageToByte(Android.Net.Uri uri)
        {
            Stream stream = ContentResolver.OpenInputStream(uri);

            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}