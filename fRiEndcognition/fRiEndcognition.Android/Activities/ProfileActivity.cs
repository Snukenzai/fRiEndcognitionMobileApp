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

            Button deleteAccount = FindViewById<Button>(Resource.Id.DeleteAccount);
            deleteAccount.Click += delegate (object sender, EventArgs e)
            {
                bool answer = Delete();
                if (answer)
                {
                    Intent i = new Intent(this, typeof(MainActivity));
                    StartActivity(i);
                }
            };

            profileName = FindViewById<TextView>(Resource.Id.ProfileName);
            profileImage = FindViewById<ImageView>(Resource.Id.ProfileImage);
            currentPerson = DataController.Instance().currentPerson;

            if (DataController.Instance().isShowingRecognition)
            {
                currentPerson = DataController.Instance().recognizedPerson;
                DataController.Instance().isShowingRecognition = false;
                changePicture.Visibility = ViewStates.Gone;
                deleteAccount.Visibility = ViewStates.Gone;
            }
            AddPerson();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                profileImage.SetImageURI(uri);
                byte[] picture = convertImageToByte(uri);
                DataController.Instance().UpdateLocalDatabase(picture);
                DataController.Instance().UpdateDatabase(picture);
            }
        }

        private bool Delete()
        {
           bool local = DataController.Instance().DeleteFromLocalDatabase();
           bool notlocal = DataController.Instance().DeleteFromDatabase();

           return local && notlocal;
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

        public void AddPerson()
        {
            profileName.Text = currentPerson.Name + " " + currentPerson.Surname;
            profileImage.SetImageBitmap(DataController.Instance().ByteArrayToBitmap(DataController.Instance().Base64StringToByteArray(currentPerson.Picture)));
        }

    }
}