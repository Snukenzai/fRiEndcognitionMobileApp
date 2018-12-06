using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using SQLite;
using Environment = System.Environment;

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
        public int id;

        public string name { get; set; }


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
                Instance().UploadToLocalDatabase();
                return Instance().UploadToDatabase();
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
            if(CheckingLocalDatabase(email, password))
                return true;
            else
            {
                if (CheckingLogin(email, password))
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

            //this line freezes the app if there's no response
            var response = Sender.getResponse(httpWebRequest);
            if (response == "200")
                return false;
            else
                return true;
        }

        public bool UploadToDatabase()
        {

            var httpWebRequest = Sender.createRequestHandler("POST", "register");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(currentPerson.ToJSON());
            }
            
            //no database, so no responses, thus freezes vvvvv
            var response = Sender.getResponse(httpWebRequest);
            if (response == "200")
                return true;
            return false;
        }

        private bool CheckingLogin(string email, string password)
        {
            var httpWebRequest = Sender.createRequestHandler("POST", "login");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}");
            }

            //this line freezes the app if there's no response
            var response = Sender.getResponse(httpWebRequest);

            if (response == "200")
                return true;
            else
                return false;
        }

        private void UploadToLocalDatabase()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath
            (System.Environment.SpecialFolder.Personal),
            "database.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<PersonStock>();
            var newPerson = new PersonStock();
            newPerson.Name = currentPerson.Name;
            newPerson.Surname = currentPerson.Surname;
            newPerson.Email = currentPerson.Email;
            newPerson.Password = currentPerson.Password;
            newPerson.Picture = currentPerson.Picture;

            db.Insert(newPerson);
        }

        private bool CheckingLocalDatabase(string email, string password)
        {
            string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "database.db3");
            var db = new SQLiteConnection(dbPath);
            try
            {
                var person = (from people in db.Table<PersonStock>()
                              where people.Email.Equals(email)
                              select people).First();

                if (person.Password == password)
                {
                    currentPerson = new Person(person.Name, person.Surname, person.Email, person.Password, person.Picture);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}