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

		IPublicClientApplication GetPublicClientApplication()
		{
			IPublicClientApplication rIPublicClientApplication = null;
			var appId = OAuth.AppId;
			rIPublicClientApplication = PublicClientApplicationBuilder
									.Create(appId)
									.Build();
			return rIPublicClientApplication;
		}

		private static System.Collections.Generic.IEnumerable<string> GetGraphScopes()
		{
			System.Collections.Generic.IEnumerable<string> rIEnumerableGraphScopes_result = new System.Collections.Generic.List<string>();
			string sScopes = OAuth.Scopes;
			rIEnumerableGraphScopes_result = rIEnumerableGraphScopes_result.Append(sScopes);
			return rIEnumerableGraphScopes_result;
		}
		private async Task CreateClientAndCallGraph(IAuthenticationProvider authProvider)
		{
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

		//Device CodeProvide
		// Device code provider führt zur folgenden exception
		// Original exception: AADSTS70002: The provided client is not supported for this feature. The client application must be marked as 'mobile.'
		//DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, rIEnumerableGraphScopes);

		private async void m_rButtonConnect_InteractiveAuthenticationProvider_Click(object sender, EventArgs e)
		{
			// Interactive provider - The interactive flow is used by mobile applications (Xamarin and UWP) and desktops applications to call Microsoft Graph in the name of a user. 
			// Create an authentication provider by passing in a client application and graph scopes.
			// https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively
			InteractiveAuthenticationProvider authProvider = new InteractiveAuthenticationProvider(GetPublicClientApplication(), GetGraphScopes());
			// Create a new instance of GraphServiceClient with the authentication provider.
			await CreateClientAndCallGraph(authProvider);
		}


		private async void m_rButton_OAuth20_Click(object sender, EventArgs e)
		{
			try {
				// Build a client application.
				var appId = OAuth_ApplicationPermissions.AppId;
				// https://aad.portal.azure.com
				string sClientSecret = OAuth_ApplicationPermissions.Secret;
				string sInstanceOfAzure = "https://login.microsoftonline.com/{0}";
				// AppId ist wie Tenant
				string sAuthority = String.Format(System.Globalization.CultureInfo.InvariantCulture, sInstanceOfAzure, appId);
				IConfidentialClientApplication rConfidentialClientApplication = ConfidentialClientApplicationBuilder
										.Create(appId)
										.WithClientSecret(sClientSecret)
										.WithAuthority(new Uri(sAuthority))
										.Build();
				// https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS#authorization-code-provider
				// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
				// Demo für daemon in console un OAuth 2.0 https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/blob/master/1-Call-MSGraph/README.md
				//TODO_FR 130 Brauche ich den AuthorizationCodeProvider? - in daemon demo ist er nicht notwendig
				//TODO_FR 140 scopes daemon demo "https://graph.microsoft.com/.default"
				// With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
				// application permissions need to be set statically (in the portal or by PowerShell), and then granted by
				// a tenant administrator. 
				string[] scopes = new string[] { $"{OAuth_ApplicationPermissions.ApiUrl}.default" };

				//AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(rConfidentialClientApplication, GetGraphScopes());
				//TODO_FR 140 application - AcquireTokenForClient(scopes)
				/*
												result = await app.AcquireTokenForClient(scopes)
														.ExecuteAsync();
				*/
				//await CreateClientAndCallGraph(authProvider);
				//TODO_FR 150 Api caller
				/*
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
				*/
			} catch (Microsoft.Graph.ServiceException rException) {
				m_rTextBoxResult.Text += System.String.Format("\nException in m_rButton_OAuth20_Click:\n{0}", rException.Message);
				if (rException.InnerException != null) {
				//TODO_FR 550 ApplicationPermission HACK
					m_rTextBoxResult.Text += System.String.Format("\nInner Exception:\n{0}", rException.InnerException.Message);
				}
			} catch (System.Exception rException) {
				m_rTextBoxResult.Text += System.String.Format("\nException in m_rButton_OAuth20_Click:\n{0}", rException.Message);
			}
		}
	}
}
