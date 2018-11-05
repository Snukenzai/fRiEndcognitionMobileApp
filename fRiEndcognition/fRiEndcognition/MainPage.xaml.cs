using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fRiEndcognition
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public async void Register_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterForm());
        }
        public async void Login_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginForm());
        }
    }
}
