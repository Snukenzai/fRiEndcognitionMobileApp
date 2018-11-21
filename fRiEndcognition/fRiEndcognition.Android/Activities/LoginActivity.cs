using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace friendcognition.Droid
{
    [Activity(Label = "LoginActivity", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class LoginActivity : Activity
    {
        private EditText email;
        private EditText password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            Button loggedIn = FindViewById<Button>(Resource.Id.FinalLogin);
            email = FindViewById<EditText>(Resource.Id.LoginEmail);
            password = FindViewById<EditText>(Resource.Id.LoginPassword);
            loggedIn.Click += delegate (object sender, EventArgs e)
            {
                if (DataController.Instance.Login(email.Text, password.Text))
                {
                    Intent i = new Intent(this, typeof(CameraActivity));

                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Denied)
                    {
                        ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 10);
                    }
                    else
                    {
                        StartActivity(i);
                    }
                }
                else
                {
                    Toast.MakeText(ApplicationContext, Resource.String.WRONG_EMAIL_OR_PASSWORD, ToastLength.Long).Show();
                }
            };
        }

        public void onRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == 10)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    Intent i = new Intent(this, typeof(CameraActivity));
                    StartActivity(i);
                }
            }
        }
    }
}