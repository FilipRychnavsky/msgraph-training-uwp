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

namespace Demo_MS_Graph_SDK
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private async void m_rButtonConnect_Click(object sender, EventArgs e)
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
			// Device code provider
			// DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, rIEnumerableGraphScopes);
			//TODO Interactive provider
			// https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively
			InteractiveAuthenticationProvider authProvider = new InteractiveAuthenticationProvider(publicClientApplication, rIEnumerableGraphScopes);
			// Create a new instance of GraphServiceClient with the authentication provider.
			GraphServiceClient graphClient = new GraphServiceClient(authProvider);
			//var user = await graphClient.Me.Request();
			//int n = 0;
			User user = await graphClient.Me
										.Request()
										.GetAsync();
		}
	}
}
