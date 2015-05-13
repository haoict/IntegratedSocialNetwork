using Facebook.Client;
using IntegratedSocialNetwork.Model;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace IntegratedSocialNetwork.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class TestView : Page
	{
		public string headern { get; private set; }
		public ObservableCollection<ISNPost> itemsList { get; set; }

		public TestView()
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
			OnQueryButtonClick(null, null);

			headern = "asdfadsf";
		}

		private async void OnQueryButtonClick(object sender, RoutedEventArgs e)
		{
			//this.myProgressRing.Visibility = Visibility.Visible;
			itemsList = new ObservableCollection<ISNPost>();
			//newFeedList.ItemsSource = itemsList;

			FetchFacebookNewFeed();
			FetchTwitterNewFeed();

			

			//this.myProgressRing.Visibility = Visibility.Collapsed;
		}

		private async void FetchFacebookNewFeed()
		{
			// get facebook new feed
			var fb = new Facebook.FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);

			var parameters = new Dictionary<string, object>();
			parameters[""] = "";

			dynamic result = await fb.GetTaskAsync("/me/home", parameters);

			foreach (var data in result[0])
			{
				ISNPost tmp = new ISNPost();
				ISNUser usr = new ISNUser();
				try
				{
					tmp.id = data["id"];
					tmp.message = data["message"];
					usr.id = data["from"]["id"];
					usr.name = data["from"]["name"];
					usr.picture = "http://graph.facebook.com/" + data["from"]["id"] + "/picture";
					tmp.user = usr;
					itemsList.Add(tmp);
				}
				catch (Exception exc)
				{
					continue;
				}

			}
			//this.myProgressRing.Visibility = Visibility.Collapsed;
			MessageDialogHelper.Show("adsf");
		}

		private async void FetchTwitterNewFeed()
		{
			// get tw new feed
			if (SharedState.Authorizer != null)
			{
				var twitterCtx = new TwitterContext(SharedState.Authorizer);

				var timelineResponse =
					await
					(from tweet in twitterCtx.Status
					 where tweet.Type == StatusType.Home
					 select tweet)
					.ToListAsync();


				List<ISNPost> Tweets =
					(from tweet in timelineResponse
					 select new ISNPost
					 {
						 user = new ISNUser("", tweet.User.ScreenNameResponse, tweet.User.ProfileImageUrl),
						 message = tweet.Text,
					 })
					.ToList();

				foreach (ISNPost isnp in Tweets)
				{
					itemsList.Add(isnp);
				}
				//this.myProgressRing.Visibility = Visibility.Collapsed;
			}
		}

		private async void PublishStory()
		{
			var fb = new Facebook.FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);

			var postParams = new
			{
				name = "Facebook SDK for .NET",
				caption = "Build great social apps and get more installs.",
				description = "The Facebook SDK for .NET makes it easier and faster to develop Facebook integrated .NET apps.",
				link = "http://facebooksdk.net/",
				picture = "http://facebooksdk.net/assets/img/logo75x75.png"
			};

			try
			{
				dynamic fbPostTaskResult = await fb.PostTaskAsync("/me/feed", postParams);
				var result2 = (IDictionary<string, object>)fbPostTaskResult;

				var successMessageDialog = new Windows.UI.Popups.MessageDialog("Posted Open Graph Action, id: " + (string)result2["id"]);
				await successMessageDialog.ShowAsync();
			}
			catch (Exception ex)
			{
				MessageDialogHelper.Show("Exception during post: " + ex.Message);
			}
		}

		private string BuildUserInfoDisplay(GraphUser user)
		{
			var userInfoname = new StringWriter();
			userInfoname.WriteLine(string.Format("{0}", user.FirstName));
			//userInfoname.WriteLine(string.Format("Name:  {0} {1} {2} {3}", user.FirstName, user.MiddleName, user.LastName, user.ProfilePictureUrl));
			userInfoname.WriteLine();
			return userInfoname.ToString();
		}

		private async void RetriveUserInfo()
		{
			var token = Session.ActiveSession.CurrentAccessTokenData.AccessToken;
			var client = new Facebook.FacebookClient(token);
			dynamic result = await client.GetTaskAsync("me");
			var currentUser = new Facebook.Client.GraphUser(result);
			//this.userInfo.Text = this.BuildUserInfoDisplay(currentUser);
		}

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private async void Test_Click(object sender, RoutedEventArgs e)
		{
			var fb = new Facebook.FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);
			var parameters = new Dictionary<string, object>();
			parameters[""] = "";

			dynamic result = await fb.GetTaskAsync("/565811000225586", parameters);


		}
	}
}
