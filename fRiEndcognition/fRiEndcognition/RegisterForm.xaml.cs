﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace fRiEndcognition
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterForm : ContentPage
	{
		public RegisterForm ()
		{
			InitializeComponent ();
		}
        public async void Register(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Camera());
        }
    }
}