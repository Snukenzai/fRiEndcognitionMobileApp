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
using SQLite;

namespace friendcognition.Droid
{
    [Table("Users")]
    public class PersonStock
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        [Column("_name")]
        public string Name { get; set; }
        [Column("_surname")]
        public string Surname { get; set; }
        [Column("_email")]
        public string Email { get; set; }
        [Column("_password")]
        public string Password { get; set; }
        [Column("_picture")]
        public byte[] Picture { get; set; }
    }
}