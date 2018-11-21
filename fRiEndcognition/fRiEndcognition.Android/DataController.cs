﻿using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly Lazy<DataController> lazy = new Lazy<DataController>(() => new DataController());

        private Dictionary<string, string> loginInfo = new Dictionary<string, string>();

        private bool touching = false;
        private float x, y;
        public int id { get; set; }

        DataController()
        {
        }

        public static DataController Instance()
        {
            return lazy.Value;
        }

        public bool SavePicture(Android.Graphics.Bitmap bitmapPicture)
        {
           // byte[] byteArrayPicture = BitmapToByteArray(bitmapPicture);
            // TO BE IMPLEMENTED, THE DATABASE LOGIC

            return true;
        }

        
        //Converts Bitmap picture to Byte Array using 
        public byte[] BitmapToByteArray(Android.Graphics.Bitmap bitmapPicture)
        {
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmapPicture.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                bitmapData = stream.ToArray();
            }
            return bitmapData;
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

        public void TouchEventDown(float x, float y)
        {
            this.x = x;
            this.y = y;
            touching = true;
        }

        public void TouchEventUp()
        {
            touching = false;
        }

        public float getX()
        {
            return x;
        }
        public float getY()
        {
            return y;
        }
        public bool getTouching()
        {
            return touching;
        }
    }
}