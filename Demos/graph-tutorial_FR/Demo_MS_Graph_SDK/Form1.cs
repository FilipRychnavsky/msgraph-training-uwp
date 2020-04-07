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

//FR Namespaces für die Anbindung an Office 365
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
			m_rTextBoxResult.Text += System.Environment.NewLine + "m_rButton_OAuth20_Click Start" + System.Environment.NewLine;
			try {
				// Build a client application.
				var appId = OAuth_ApplicationPermissions.AppId;
				// https://aad.portal.azure.com
				string sClientSecret = OAuth_ApplicationPermissions.Secret;
				string sInstanceOfAzure = "https://login.microsoftonline.com/{0}";
				string sAuthority = String.Format(System.Globalization.CultureInfo.InvariantCulture, sInstanceOfAzure, OAuth_ApplicationPermissions.Tenant);
				IConfidentialClientApplication rConfidentialClientApplication = ConfidentialClientApplicationBuilder
										.Create(appId)
										.WithClientSecret(sClientSecret)
										.WithAuthority(new Uri(sAuthority))
										.Build();
				// https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS#authorization-code-provider
				// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
				// Demo für daemon in console un OAuth 2.0 https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/blob/master/1-Call-MSGraph/README.md

				// With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
				// application permissions need to be set statically (in the portal or by PowerShell), and then granted by
				// a tenant administrator. 
				string[] scopes = new string[] { $"{OAuth_ApplicationPermissions.ApiUrl}.default" };

				//AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(rConfidentialClientApplication, GetGraphScopes());
				AuthenticationResult rAuthenticationResult = null;
				try {
					rAuthenticationResult = await rConfidentialClientApplication.AcquireTokenForClient(scopes)
							.ExecuteAsync();
					m_rTextBoxResult.Text += "Token acquired" + System.Environment.NewLine;
				} catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011")) {
					// Invalid scope. The scope has to be of the form "https://resourceurl/.default"
					// Mitigation: change the scope to be as expected
					m_rTextBoxResult.Text += System.String.Format("Scope provided is not supported.\n");
				}
				if (rAuthenticationResult != null) {
					var rHttpClient = new HttpClient();
					string sWebApiUrl = $"{OAuth_ApplicationPermissions.ApiUrl}v1.0/users";
					var defaultRequestHeaders = rHttpClient.DefaultRequestHeaders;
					// Test: wenn ich kein QualityHeaderValue hinzugefügt habe, habe ich trotzdem gleiches Ergebnis bekommen.
					if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json")) {
						rHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					}
					defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", rAuthenticationResult.AccessToken);
					HttpResponseMessage rHttpResponseMessage = await rHttpClient.GetAsync(sWebApiUrl);
					if (rHttpResponseMessage.IsSuccessStatusCode) {
						string sResponseAsString = await rHttpResponseMessage.Content.ReadAsStringAsync();
						JObject rJObectResponse = JsonConvert.DeserializeObject(sResponseAsString) as JObject;
						foreach (JProperty child in rJObectResponse.Properties().Where(p => !p.Name.StartsWith("@"))) {
							m_rTextBoxResult.Text += System.Environment.NewLine + $"{child.Name} = {child.Value}";
						}
					} else {
						m_rTextBoxResult.Text += System.Environment.NewLine + $"Failed to call the Web Api: {rHttpResponseMessage.StatusCode}";
						string content = await rHttpResponseMessage.Content.ReadAsStringAsync();
						// Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
						// this is because the tenant admin as not granted consent for the application to call the Web API
						m_rTextBoxResult.Text += System.Environment.NewLine + $"Content: {content}";
					}
				}
			} catch (Microsoft.Graph.ServiceException rException) {
				m_rTextBoxResult.Text += System.String.Format("\nException in m_rButton_OAuth20_Click:\n{0}", rException.Message);
				if (rException.InnerException != null) {
					m_rTextBoxResult.Text += System.String.Format("\nInner Exception:\n{0}", rException.InnerException.Message);
				}
			} catch (System.Exception rException) {
				m_rTextBoxResult.Text += System.String.Format("\nException in m_rButton_OAuth20_Click:\n{0}", rException.Message);
			}
		}

		private async void m_rButtonExcel_Click(object sender, EventArgs e)
		{
			m_rTextBoxResult.Text += System.Environment.NewLine + "m_rButtonExcel_Click Start" + System.Environment.NewLine;
			try {
				// Build a client application.
				var appId = OAuth_ApplicationPermissions.AppId;
				// https://aad.portal.azure.com
				string sClientSecret = OAuth_ApplicationPermissions.Secret;
				string sInstanceOfAzure = "https://login.microsoftonline.com/{0}";
				string sAuthority = String.Format(System.Globalization.CultureInfo.InvariantCulture, sInstanceOfAzure, OAuth_ApplicationPermissions.Tenant);
				IConfidentialClientApplication rConfidentialClientApplication = ConfidentialClientApplicationBuilder
										.Create(appId)
										.WithClientSecret(sClientSecret)
										.WithAuthority(new Uri(sAuthority))
										.Build();
				// https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS#authorization-code-provider
				// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
				// Demo für daemon in console un OAuth 2.0 https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/blob/master/1-Call-MSGraph/README.md

				// With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
				// application permissions need to be set statically (in the portal or by PowerShell), and then granted by
				// a tenant administrator. 
				string[] scopes = new string[] { $"{OAuth_ApplicationPermissions.ApiUrl}.default" };

				//TODO_FR 190 experiment AuthorizationCodeProvider
				// https://docs.microsoft.com/en-us/graph/sdks/create-requests?tabs=CS
				AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(rConfidentialClientApplication);
// GET https://graph.microsoft.com/v1.0/me

/*
     User user = await rConfidentialClientApplication.Me  
                .Request()
                .GetAsync();
     User user = await authProvider.Me  
                .Request()
                .GetAsync();
*/
				AuthenticationResult rAuthenticationResult = null;
				try {
					rAuthenticationResult = await rConfidentialClientApplication.AcquireTokenForClient(scopes)
							.ExecuteAsync();
					m_rTextBoxResult.Text += "Token acquired" + System.Environment.NewLine;
				} catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011")) {
					// Invalid scope. The scope has to be of the form "https://resourceurl/.default"
					// Mitigation: change the scope to be as expected
					m_rTextBoxResult.Text += System.String.Format("Scope provided is not supported.\n");
				}
				if (rAuthenticationResult != null) {
					var rHttpClient = new HttpClient();
					//TODO_FR 299 Neuanlage eine Datei in Office 365
					//TODO_FR 210 WebApiUrl für Neuanlage eines Excel Sheet; Kann man sich in Graph oder Postman inspirieren?
					string sWebApiUrl = $"{OAuth_ApplicationPermissions.ApiUrl}v1.0/users";
					//string sWebApiUrl = $"{OAuth_ApplicationPermissions.ApiUrl}v1.0/me";
					//TODO_FR 220 UpStreamen in Office
					// Test: wenn ich kein QualityHeaderValue hinzugefügt habe, habe ich trotzdem gleiches Ergebnis bekommen.
					if (rHttpClient.DefaultRequestHeaders.Accept == null || !rHttpClient.DefaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json")) {
						rHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					}
					rHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", rAuthenticationResult.AccessToken);
					HttpResponseMessage rHttpResponseMessage = await rHttpClient.GetAsync(sWebApiUrl);
					if (rHttpResponseMessage.IsSuccessStatusCode) {
						string sResponseAsString = await rHttpResponseMessage.Content.ReadAsStringAsync();
						JObject rJObectResponse = JsonConvert.DeserializeObject(sResponseAsString) as JObject;
						foreach (JProperty child in rJObectResponse.Properties().Where(p => !p.Name.StartsWith("@"))) {
							m_rTextBoxResult.Text += System.Environment.NewLine + $"{child.Name} = {child.Value}";
						}
						//TODO_FR 230 Downstream (oder bekommen wir einen Link auf neu angelegte Datei zurück)
					} else {
						m_rTextBoxResult.Text += System.Environment.NewLine + $"Failed to call the Web Api: {rHttpResponseMessage.StatusCode}";
						string content = await rHttpResponseMessage.Content.ReadAsStringAsync();
						// Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
						// this is because the tenant admin as not granted consent for the application to call the Web API
						m_rTextBoxResult.Text += System.Environment.NewLine + $"Content: {content}";
					}
				}
			} catch (Microsoft.Graph.ServiceException rException) {
				m_rTextBoxResult.Text += System.String.Format("\nException in m_rButton_OAuth20_Click:\n{0}", rException.Message);
				if (rException.InnerException != null) {
					m_rTextBoxResult.Text += System.String.Format("\nInner Exception:\n{0}", rException.InnerException.Message);
				}
			} catch (System.Exception rException) {
				m_rTextBoxResult.Text += System.String.Format("\nException in m_rButton_OAuth20_Click:\n{0}", rException.Message);
			}
		}
	}
}
