using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;

namespace IntegratedSocialNetwork
{
	public class GlobalUtil
	{

	}

	public class MessageDialogHelper
	{
		public static async void Show(string content, string title = "")
		{
			try
			{
				MessageDialog messageDialog = new MessageDialog(content, title);
				await messageDialog.ShowAsync();
			}
			catch (Exception exc)
			{
				string excmsg = exc.Message;
			}
		}
	}
}
