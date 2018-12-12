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
    class Person
    {

        private string name;
        private string surname;
        private string email;
        private string password;
        private string picture;

        public string Name { get => name; set => name = value; }
        public string Surname { get => surname; set => surname = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public string Picture { get => picture; set => picture = value; }

        public Person(string email, string name, string surname, string password, string picture)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            Picture = picture;
        }

        public string ToJSON()
        {
            return "{\"name\": \"" + name +
                    "\", \"surname\": \"" + surname +
                    "\", \"email\": \"" + email +
                    "\", \"password\": \"" + password +
                    "\", \"picture\": \"" + picture + "\"}";
        }
    }
}