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

using Facebook;
using Facebook.Client;
using IntegratedSocialNetwork.Model;
using LinqToTwitter;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace IntegratedSocialNetwork.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainView : Page
	{
		public ObservableCollection<ISNPost> itemsList { get; set; }
		private int currentPivot;

		public MainView()
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
			currentPivot = 1;
			OnQueryButtonClick(null, null);
			
		}

// 		private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
// 		{
// 			this.myProgressRing.Visibility = (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Opened) ? Visibility.Visible : Visibility.Collapsed;
// 			//loginButton.Visibility = Visibility.Collapsed;
// 
// 			if (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Opened)
// 			{
// 				this.userInfo.Visibility = Visibility.Visible;
// 				this.RetriveUserInfo();
// 				this.OnQueryButtonClick(null, null);
// 			}
// 			else if (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Closed)
// 			{
// 				this.userInfo.Visibility = Visibility.Collapsed;
// 			}
// 		}

		private async void OnQueryButtonClick(object sender, RoutedEventArgs e)
		{
			this.myProgressRing.Visibility = Visibility.Visible;
			itemsList = new ObservableCollection<ISNPost>();
			newFeedList.ItemsSource = itemsList;
			newFeedList2.ItemsSource = itemsList;

			if (currentPivot == 1)
			{
				FetchFacebookNewFeed("/me/home");
				FetchTwitterNewFeed(StatusType.Home);
			}
			else if (currentPivot == 2)
			{
				FetchFacebookNewFeed("/me/feed");
				FetchTwitterNewFeed(StatusType.User);
			}

			
			
			//this.myProgressRing.Visibility = Visibility.Collapsed;
		}

		private async void FetchFacebookNewFeed(string feedType)
		{
			if (Session.ActiveSession.CurrentAccessTokenData.AccessToken == "") return;

			try
			{
				var fb = new Facebook.FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);

				var parameters = new Dictionary<string, object>();
				parameters[""] = "";

				dynamic result = await fb.GetTaskAsync(feedType, parameters);

				foreach (var data in result[0])
				{
					ISNPost tmp = new ISNPost();
					ISNUser usr = new ISNUser();
					try
					{
						tmp.id = data["id"];
						tmp.postSource = "Facebook";
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
				this.myProgressRing.Visibility = Visibility.Collapsed;
				this.myProgressRing2.Visibility = Visibility.Collapsed;
			}
			catch (Exception exc)
			{
				MessageDialogHelper.Show(exc.Message);
			}
		}

		private async void FetchTwitterNewFeed(StatusType statusType)
		{
			// get tw new feed
			if (SharedState.Authorizer == null) return;
			try
			{
				var twitterCtx = new TwitterContext(SharedState.Authorizer);

				var timelineResponse =
					await
					(from tweet in twitterCtx.Status
					 where tweet.Type == statusType
					 select tweet)
					.ToListAsync();


				List<ISNPost> Tweets =
					(from tweet in timelineResponse
					 select new ISNPost
					 {
						 user = new ISNUser("", tweet.User.ScreenNameResponse, tweet.User.ProfileImageUrl),
						 postSource = "Tweeter",
						 message = tweet.Text,
					 })
					.ToList();

				foreach (ISNPost isnp in Tweets)
				{
					itemsList.Add(isnp);
				}
				this.myProgressRing.Visibility = Visibility.Collapsed;
				this.myProgressRing2.Visibility = Visibility.Collapsed;
			}
			catch (Exception exc)
			{
				MessageDialogHelper.Show(exc.Message);
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
			this.userInfo.Text = this.BuildUserInfoDisplay(currentUser);
		}

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}

		private void ListView_SelectionChanged2(object sender, SelectionChangedEventArgs e)
		{
			
		}

		private void SecondPivot_Loaded(object sender, RoutedEventArgs e)
		{

			this.myProgressRing2.Visibility = Visibility.Visible;
			currentPivot = 2;
			if (itemsList != null)
			{
				itemsList.Clear();
				
			}
			FetchFacebookNewFeed("/me/feed");
			FetchTwitterNewFeed(StatusType.User);
		}
	}
}
