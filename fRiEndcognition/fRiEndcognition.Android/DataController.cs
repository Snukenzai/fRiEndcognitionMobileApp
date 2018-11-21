﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        private static readonly Lazy<DataController> lazy = new Lazy<DataController>(() => new DataController());

        private byte[] byteArrayCurrent;

        private Dictionary<string, string> loginInfo = new Dictionary<string, string>();

        DataController()
        {
        }

        public static DataController Instance()
        {
            return lazy.Value;
        }

        public bool SavePicture(byte[] byteArrayPicture)
        {

            if (byteArrayPicture != null)
            {
                byteArrayCurrent = byteArrayPicture;

                return true;
            }
            else
            {
                return false;
            }
            
        }

        
        //Converts Bitmap picture to Byte Array using 
        public byte[] BitmapToByteArray(Android.Graphics.Bitmap bitmapData)
        {
            byte[] byteArrayData;
            using (var stream = new MemoryStream())
            {
                bitmapData.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                byteArrayData = stream.ToArray();
            }
            return byteArrayData;
        }

        public Android.Graphics.Bitmap ByteArrayToBitmap(byte[] byteArrayData)
        {
            return Android.Graphics.BitmapFactory.DecodeByteArray(byteArrayData, 0, byteArrayData.Length);
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
                return RegistrationCallbacks.INVALID_EMAIL;
            }

            if (loginInfo.ContainsKey(email))
            {
                return RegistrationCallbacks.EMAIL_EXISTS;
            }

            if (!ValidateStringOnlyLetters(name))
            {
                return RegistrationCallbacks.INVALID_NAME;
            }
            if (!ValidateStringOnlyLetters(name))
            {
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