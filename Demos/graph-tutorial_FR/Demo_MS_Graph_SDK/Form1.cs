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

		private void m_rButtonConnect_Click(object sender, EventArgs e)
		{
			const string resxFile = @".\OAuth.resx";
			//System.Resources.ResourceManager oauthSettings = new System.Resources.ResourceManager("OAuth", typeof(Form1).Assembly);
			// statt ResourceLoader von UWP benutze ich C# App Resoucen
			//ResXResourceSet oauthSettings = new ResXResourceSet(resxFile);
			// Build a client application.
			var appId = oauthSettings.GetString("AppId");
			IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
									.Create(appId)
									.Build();
			// Create an authentication provider by passing in a client application and graph scopes.
			// Load OAuth settings
			//var oauthSettings = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("OAuth");
			string sScopes = oauthSettings.GetString("Scopes");
			//var graphScopes = oauthSettings.GetString("Scopes");
			System.Collections.Generic.IEnumerable<string> rIEnumerableGraphScopes = new System.Collections.Generic.List<string>();
			rIEnumerableGraphScopes.Append(sScopes);
			DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, rIEnumerableGraphScopes);
			// Create a new instance of GraphServiceClient with the authentication provider.
			GraphServiceClient graphClient = new GraphServiceClient(authProvider);
		}
	}
}
