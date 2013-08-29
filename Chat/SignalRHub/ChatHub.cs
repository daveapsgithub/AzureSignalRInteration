using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Timers;

namespace Chat.SignalRHub
{
	public class ChatHub : Hub
	{
		static Timer t = null;
		static string connectionId;


		public override System.Threading.Tasks.Task OnConnected()
		{
			connectionId = this.Context.ConnectionId;

			Chat.Models.Storage.RegisterChatEndPoint();

			t = new Timer(5000);
			t.Elapsed += new ElapsedEventHandler(t_Elapsed);
			t.Start(); 
			
			return base.OnConnected();
		}

		static void t_Elapsed(object sender, ElapsedEventArgs e)
		{
			SendMessageToClient("Periodic call to client: " + DateTime.Now.ToString(), connectionId);
		}

		/// <summary>
		/// Receives the message and sends it to the SignalR client.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="connectionId">The connection id.</param>
		public static void SendMessageToClient(string message, string connectionId)
		{
			GlobalHost.ConnectionManager.GetHubContext<ChatHub>().Clients.Client(connectionId).SendMessageToClient(message);

			Trace.TraceInformation("Sending: '" + message + "' on " + connectionId + " on " + RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["WebApi"].IPEndpoint.ToString());
		}

				
		/// <summary>
		/// Sends the message to other instance.
		/// </summary>
		/// <param name="chatClientId">The chat client id.</param>
		/// <param name="message">The message.</param>
		public void SendMessageToServer()
		{
			Trace.TraceInformation("Received a 'SendMessageToServer': " + this.Context.ConnectionId);

			SendMessageToClient("Reply to 'SendMessageToServer'", this.Context.ConnectionId);
		}
		
				/// <summary>
		/// Sends the message to other instance.
		/// </summary>
		/// <param name="chatClientId">The chat client id.</param>
		/// <param name="message">The message.</param>
		public void SendMessageToServerWithWebApi()
		{
			Trace.TraceInformation("Received a message: " + this.Context.ConnectionId);

			string endpoint = Chat.Models.Storage.GetChatEndpoint();

			Chat.WebApi.ChatWebApiController.SendMessage(endpoint, this.Context.ConnectionId, "Received a 'SendMessageToServerWithWebApi': " + this.Context.ConnectionId);
		}


		/// <summary>
		/// Sends the message to other instance.
		/// </summary>
		/// <param name="chatClientId">The chat client id.</param>
		/// <param name="message">The message.</param>
		public void SendPeriodicMessageToServer()
		{
			Trace.TraceInformation("Received a 'SendPeriodicMessageToServer': " + this.Context.ConnectionId);

			SendMessageToClient("Reply to 'SendPeriodicMessageToServer'", this.Context.ConnectionId);
		}
	}
}