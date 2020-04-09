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
using System.IO;

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
			// Doku in SharePoint: https://alphaplan.sharepoint.com/sites/Entwicklung/_layouts/15/Doc.aspx?sourcedoc={ecab1635-5b06-4767-8259-a963bcc3e8f7}&action=edit&wd=target%28Brainstorming.one%7C72cf04d9-f05e-4b91-afc5-be7625335627%2FDemo%20-%20Datei%20hochladen%7Cd5108952-e36f-49ef-be5c-9015928bbaf1%2F%29&wdorigin=703
			m_rTextBoxResult.Text += System.Environment.NewLine + "m_rButtonExcel_Click Start" + System.Environment.NewLine;
			//TODO_FR 650 Den Code auch in einer VM in der cloud testen (wegen Packages; was Borja noch installieren muss)
			try {
				string sAppId					= OAuth_ApplicationPermissions.AppId; //Dient als ClientID Parameter
				string sClientSecret	= OAuth_ApplicationPermissions.Secret;
				//Redirect url habe ich von dem Blog - Day 25 geschrieben; ich weiß nicht, wie weit wichtig es ist
				string sRedirectUri		= "https://localhost:8080";
				string sInstanceOfAzure = "https://login.microsoftonline.com/{0}";
				string sAuthority			=	String.Format(System.Globalization.CultureInfo.InvariantCulture, sInstanceOfAzure, OAuth_ApplicationPermissions.Tenant);
        var rConfidentialClientApplicationBuilder = ConfidentialClientApplicationBuilder.Create(sAppId)
                                                    .WithAuthority(sAuthority)
                                                    .WithRedirectUri(sRedirectUri)
                                                    .WithClientSecret(sClientSecret)
                                                    .Build();
				// With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
				// application permissions need to be set statically (in the portal or by PowerShell), and then granted by
				// a tenant administrator. 
				string[] scopes = new string[] { $"{OAuth_ApplicationPermissions.ApiUrl}.default" };
				// ConsoleGraphTest.MsalAuthenticationProvider wurde von https://developer.microsoft.com/en-us/graph/blogs/30daysmsgraph-day-15-microsoft-graph-in-dotnet-core-application/
				IAuthenticationProvider rAuthenticationProvider = new ConsoleGraphTest.MsalAuthenticationProvider(rConfidentialClientApplicationBuilder, scopes.ToArray());
				GraphServiceClient rGraphServiceClient = new  GraphServiceClient(rAuthenticationProvider);
				const string sUserPrincipalName = "Babsi05@CVSDemo05.onmicrosoft.com";
				// Beispiel Day29 OneDriveHelperCall
				const string sSmallFilePath = @"SampleFiles\SmallFile.txt";
				const string rLargeFilePath = @"SampleFiles\LargeFile.txt";
				// Wegen der Freigabe von Instanzen von FileStream habe ich sie in code Blöcke verschoben
				{
					DriveItem rDriveItem_uploadedFile = null;
					FileStream rFileStream = new FileStream(sSmallFilePath, FileMode.Open);
					//uploadedFile = (await rGraphServiceClient.Me.Drive.Root.ItemWithPath(smallFilePath).Content.Request().PutAsync<DriveItem>(fileStream ));
					// Im Vergleich zum Beispiel Day 29 (Me.Drive.Root) verwende ich Users[sUserPrincipalName].Drive.Root
					rDriveItem_uploadedFile = (await rGraphServiceClient.Users[sUserPrincipalName].Drive.Root.ItemWithPath(sSmallFilePath).Content.Request().PutAsync<DriveItem>(rFileStream));
					rFileStream.Close();
					rFileStream.Dispose();
					if (rDriveItem_uploadedFile != null) {
						Console.WriteLine($"Uploaded file {sSmallFilePath} to {rDriveItem_uploadedFile.WebUrl}.");
					} else {
						Console.WriteLine($"Failure uploading {sSmallFilePath}");
					}
				}
				//	large file upload in session
				{
					DriveItem rDriveItem_uploadedFile = null;
					FileStream rFileStream = new FileStream(rLargeFilePath, FileMode.Open);
					UploadSession rUploadSession = await rGraphServiceClient.Users[sUserPrincipalName].Drive.Root.ItemWithPath(rLargeFilePath).CreateUploadSession().Request().PostAsync();
					if (rUploadSession != null) {
						// Chunk size must be divisible by 320KiB, our chunk size will be slightly more than 1MB
						int maxSizeChunk = (320 * 1024) * 4;
						ChunkedUploadProvider rChunkedUploadProvider = new ChunkedUploadProvider(rUploadSession, rGraphServiceClient, rFileStream, maxSizeChunk);
						var rvUploadChunkRequests = rChunkedUploadProvider.GetUploadChunkRequests();
						var rvExceptions = new List<Exception>();
						var abyteReadBuffer = new byte[maxSizeChunk];
						foreach (var rUploadChungRequest in rvUploadChunkRequests) {
							var rUploadChunkResult = await rChunkedUploadProvider.GetChunkRequestResponseAsync(rUploadChungRequest, rvExceptions);

							if (rUploadChunkResult.UploadSucceeded) {
								rDriveItem_uploadedFile = rUploadChunkResult.ItemResponse;
							}
						}
					}
					rFileStream.Close();
					rFileStream.Dispose();
					if (rDriveItem_uploadedFile != null) {
						Console.WriteLine($"Uploaded file {rLargeFilePath} to {rDriveItem_uploadedFile.WebUrl}.");
					} else {
						Console.WriteLine($"Failure uploading {rLargeFilePath}");
					}
				}
				//TODO_FR 430 Downstream (oder bekommen wir einen Link auf neu angelegte Datei zurück)
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
