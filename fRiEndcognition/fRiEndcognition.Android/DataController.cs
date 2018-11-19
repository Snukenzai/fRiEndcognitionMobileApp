using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public enum RegistrationCallbacks { INVALID_NAME, INVALID_SURNAME, INVALID_EMAIL, INVALID_PASSWORD, EMAIL_EXISTS, PASSED}

        public const string REGEX_ONLY_LETTERS = @"^[a-zA-Z]+$";
        public const string REGEX_EMAIL = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        private static DataController instance = null;
        private static readonly object padlock = new object();

        private Dictionary<string, string> loginInfo = new Dictionary<string, string>();

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
            Recognition.RecognitionController.UpdateAlbum(bitmapPicture);

            return true;
        }


        public bool Login(string email, string password)
        {
            if (!loginInfo.ContainsKey(email))
            {
                return false;
            }

            if (loginInfo[email].Equals(password))
            {
                return true;
            }

            return false;

        }

        public RegistrationCallbacks Register(string name, string surname, string email, string password, string repeatPassword)
        {
            if (!ValidateEmail(email))
            {
                //MessageBox.Show(Constants.WRONG_EMAIL);
                return RegistrationCallbacks.INVALID_EMAIL;
            }

            if (loginInfo.ContainsKey(email))
            {
                //MessageBox.Show(Constants.EMAIL_ALREADY_EXISTS);
                return RegistrationCallbacks.EMAIL_EXISTS;
            }

            if (!ValidateStringOnlyLetters(name))
            {
                //MessageBox.Show(Constants.INVALID_NAME);
                return RegistrationCallbacks.INVALID_NAME;
            }
            if (!ValidateStringOnlyLetters(name))
            {
                //MessageBox.Show(Constants.INVALID_SURNAME);
                return RegistrationCallbacks.INVALID_SURNAME;
            }

            if (password.Length == 0 || !password.Equals(repeatPassword))
            {
                return RegistrationCallbacks.INVALID_PASSWORD;
            }

            loginInfo.Add(email, password);

            return RegistrationCallbacks.PASSED;

        }

        private bool ValidateEmail(string email)
        {
            if (Regex.IsMatch(email, REGEX_EMAIL))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ValidateStringOnlyLetters(string input)
        {
            if (Regex.IsMatch(input, REGEX_ONLY_LETTERS))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}