using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Resources;
using System.Diagnostics;

namespace Demo_MS_Graph_SDK
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private async void m_rButtonConnect_InteractiveAuthenticationProvider_Click(object sender, EventArgs e)
		{
			// Build a client application.
			var appId = OAuth.AppId;
			IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
									.Create(appId)
									.Build();
			string sScopes = OAuth.Scopes;
			// Create an authentication provider by passing in a client application and graph scopes.
			System.Collections.Generic.IEnumerable<string> rIEnumerableGraphScopes = new System.Collections.Generic.List<string>();
			rIEnumerableGraphScopes = rIEnumerableGraphScopes.Append(sScopes);

			// Device code provider führt zur folgenden exception
			// Original exception: AADSTS70002: The provided client is not supported for this feature. The client application must be marked as 'mobile.'
			//DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, rIEnumerableGraphScopes);

			// Interactive provider
			// https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively
			InteractiveAuthenticationProvider authProvider = new InteractiveAuthenticationProvider(publicClientApplication, rIEnumerableGraphScopes);
			// Create a new instance of GraphServiceClient with the authentication provider.
			GraphServiceClient graphClient = new GraphServiceClient(authProvider);

			User user = await graphClient.Me
										.Request()
										.Select(u => new
										{
											u.DisplayName,
											u.JobTitle
										})
										.GetAsync();
			Debug.WriteLine("after calling");
			m_rTextBoxResult.Text += System.String.Format("\nName: {0} JobTitle: {1}", user.DisplayName, user.JobTitle);
		}

		private void m_rButton_OAuth20_Click(object sender, EventArgs e)
		{
			// Build a client application.
			var appId = OAuth.AppId;
//TODO Redirect URI und Secret
			IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
									.Create(appId)

									.Build();
			string sScopes = OAuth.Scopes;
			// Create an authentication provider by passing in a client application and graph scopes.
			System.Collections.Generic.IEnumerable<string> rIEnumerableGraphScopes = new System.Collections.Generic.List<string>();
			rIEnumerableGraphScopes = rIEnumerableGraphScopes.Append(sScopes);

			// https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS#authorization-code-provider
			//TODO InteractiveAuthenticationProvider authProvider = new InteractiveAuthenticationProvider(publicClientApplication, rIEnumerableGraphScopes);
			// Create a new instance of GraphServiceClient with the authentication provider.
			GraphServiceClient graphClient = new GraphServiceClient(authProvider);

			User user = await graphClient.Me
										.Request()
										.Select(u => new
										{
											u.DisplayName,
											u.JobTitle
										})
										.GetAsync();
			Debug.WriteLine("after calling");
			m_rTextBoxResult.Text += System.String.Format("\nName: {0} JobTitle: {1}", user.DisplayName, user.JobTitle);
		}
	}
}
