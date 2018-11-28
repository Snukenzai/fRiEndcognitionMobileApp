using System;
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
using friendcognition.Droid.HTTP;

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

        private byte[] byteArrayCurrent;

        private bool touching = false;
        private float x, y;
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string password { get; set; }

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
            //if (!loginInfo.ContainsKey(email))
            //{
            //    return false;
            //}

            //if (loginInfo[email].Equals(password))
            //{
            //    return true;
            //}

            return true;
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

            var response = Sender.getResponse(httpWebRequest);

            System.IO.File.WriteAllText(@"C:\Users\Gytis\Desktop\Response.txt", response.ToString());

            return true;
        }

        public void UploadToDatabase()
        {

            var httpWebRequest = Sender.createRequestHandler("POST", "database");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(currentPerson.ToJSON());
            }

            var response = Sender.getResponse(httpWebRequest);

            //need to handle response
        }


    }
}