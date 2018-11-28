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
using Plugin.Connectivity;

namespace friendcognition.Droid
{
    [Activity(Label = "RegisterActivity", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class RegisterActivity : Activity
    {
        private EditText name;
        private EditText surname;
        private EditText email;
        private EditText password;
        private EditText repeatPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            name = FindViewById<EditText>(Resource.Id.RegisterName);
            surname = FindViewById<EditText>(Resource.Id.RegisterSurname);
            email = FindViewById<EditText>(Resource.Id.RegisterEmail);
            password = FindViewById<EditText>(Resource.Id.RegisterPassword);
            repeatPassword = FindViewById<EditText>(Resource.Id.RegisterPassword2);

            Button registered = FindViewById<Button>(Resource.Id.FinalRegister);
            registered.Click += delegate (object sender, EventArgs e)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    DataController.RegistrationCallbacks callback = DataController.Instance().Register(name.Text, surname.Text, email.Text, password.Text, repeatPassword.Text);

                    if (callback == DataController.RegistrationCallbacks.PASSED)
                    {
                        Intent i = new Intent(this, typeof(RegisterCamera));

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
                        switch (callback)
                        {
                            case DataController.RegistrationCallbacks.EMAIL_EXISTS:
                                Toast.MakeText(ApplicationContext, Resource.String.EMAIL_ALREADY_EXISTS, ToastLength.Long).Show();
                                break;
                            case DataController.RegistrationCallbacks.INVALID_NAME:
                                Toast.MakeText(ApplicationContext, Resource.String.INVALID_NAME, ToastLength.Long).Show();
                                break;
                            case DataController.RegistrationCallbacks.INVALID_SURNAME:
                                Toast.MakeText(ApplicationContext, Resource.String.INVALID_SURNAME, ToastLength.Long).Show();
                                break;
                            case DataController.RegistrationCallbacks.INVALID_EMAIL:
                                Toast.MakeText(ApplicationContext, Resource.String.INVALID_EMAIL, ToastLength.Long).Show();
                                break;
                            case DataController.RegistrationCallbacks.INVALID_PASSWORD:
                                Toast.MakeText(ApplicationContext, Resource.String.INVALID_PASSWORD, ToastLength.Long).Show();
                                break;
                            case DataController.RegistrationCallbacks.USER_EXISTS:
                                Toast.MakeText(ApplicationContext, Resource.String.INVALID_EMAIL, ToastLength.Long).Show();
                                break;
                            default:
                                Toast.MakeText(ApplicationContext, Resource.String.SOMETHING_WENT_WRONG, ToastLength.Long).Show();
                                break;
                        }
                    }
                }
                else
                    Toast.MakeText(ApplicationContext, Resource.String.NO_INTERNET_CONNECTION, ToastLength.Long).Show();
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