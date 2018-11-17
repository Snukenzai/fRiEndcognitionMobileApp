using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace friendcognition.Droid
{
    class Constants
    {
        public const string WRONG_EMAIL_OR_PASSWORD = "Wrong email or password...";
        public const string WRONG_PASSWORD = "Wrong password";

        public const string INVALID_NAME = "Invalid name";
        public const string INVALID_SURNAME = "Invalid surname";

        public const string EMAIL_ALREADY_EXISTS = "Such email already exists";

        public const string REGEX_ONLY_LETTERS = @"^[a-zA-Z]+$";
        public const string REGEX_EMAIL = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
    }
}