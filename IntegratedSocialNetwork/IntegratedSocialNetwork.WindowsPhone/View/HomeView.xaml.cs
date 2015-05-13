using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// 
// using Facebook;
// using Facebook.Client;
// using Facebook.Client.Controls.WebDialog;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace IntegratedSocialNetwork.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	/// 


	public sealed partial class HomeView : Page
	{
		public HomeView()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			
		}

		private void Button_Facebook_Click(object sender, RoutedEventArgs e)
		{
			if (!((Frame)Window.Current.Content).Navigate(typeof(MainView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}

		private async void Btn_Twitter_Login_Click(object sender, RoutedEventArgs e)
		{
			if (SharedState.Authorizer == null)
			{
				var auth = new SingleUserAuthorizer
				{
					CredentialStore = new InMemoryCredentialStore
					{
						ConsumerKey = "MsLLdIN5d4tGig1mIOd544sre",
						ConsumerSecret = "ZPHLC05kp0wt2ZZMYPsCPMrLlhe7bXIPgHP2BaK2PHoMSl8OWR",
						OAuthToken = "730162950-GA2MJy8n6vqwstq2ZgCaU3fe5JzZjAi8bOKXN9sN",
						OAuthTokenSecret = "ss5yUNU1RwXws2iuQMFZ77pC97r6smuoNO5e0pm8d6xux"
					},
				};

				await auth.AuthorizeAsync();

				SharedState.Authorizer = auth;

				this.Btn_Twitter_Login.Content = "Logout";
				MessageDialogHelper.Show("Twitter Login successfully");
			}
		}

		private void Btn_FB_OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
		{

		}
	}
}
