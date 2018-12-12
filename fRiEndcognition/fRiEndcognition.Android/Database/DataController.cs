﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using friendcognition.Droid.HTTP;
using Plugin.Connectivity;

namespace friendcognition.Droid
{
    class DataController
    {

        public enum RegistrationCallbacks { INVALID_NAME, INVALID_SURNAME, INVALID_EMAIL, INVALID_PASSWORD, EMAIL_EXISTS, PASSED, USER_EXISTS}

        public const string REGEX_ONLY_LETTERS = @"^[a-zA-Z]+$";
        public const string REGEX_EMAIL = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public Person currentPerson;

        private static readonly Lazy<DataController> lazy = new Lazy<DataController>(() => new DataController());

        private Dictionary<string, string> loginInfo = new Dictionary<string, string>();

        public Dictionary<string, Person> personList = new Dictionary<string, Person>();

        private byte[] byteArrayCurrent;

        private bool touching = false;
        private float x, y;
        public bool openProfile { get; set; }

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
                currentPerson.Picture = byteArrayCurrent;
                Instance().UploadToDatabase();

                return true;
            }
            else
            {
                return false;
            }
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

            

            // If all the checks are passed, create a new Person object

            currentPerson = new Person(name, surname, email, password);

            personList.Add(email, currentPerson);

            return RegistrationCallbacks.PASSED;

        }

        private bool ValidateEmail(string email)
        {
            if (!Regex.IsMatch(email, REGEX_EMAIL))
            {
                return false;
            }
            else if (EmailExists(email))
            {
                return false;
            }
            else
            {
                return true;
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

        private bool EmailExists(string email)
        {

            var httpWebRequest = Sender.createRequestHandler("POST", "email");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("{\"email\": \"" + email + "\"}");
            }


            // this line freezes the app if there's no response
            var response = Sender.getResponse(httpWebRequest);

            //System.IO.File.WriteAllText(@"C:\Users\Gytis\Desktop\Response.txt", response.ToString());

            return false;
        }

        public void UploadToDatabase()
        {

            var httpWebRequest = Sender.createRequestHandler("POST", "database");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(currentPerson.ToJSON());
            }


            // no database, so no responses, thus freezes vvvvv
            //var response = Sender.getResponse(httpWebRequest);

            //need to handle response
        }
    }
}