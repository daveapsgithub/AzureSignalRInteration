using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;

namespace Chat.Models
{
	public class Storage
	{
		static CloudStorageAccount storageAccount = null;

		static CloudTable chatClientTable = null;

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public static void Start()
		{
			storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the table client.
			CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

			// Create the table if it doesn't exist.
			chatClientTable = tableClient.GetTableReference("ChatClient");
			chatClientTable.CreateIfNotExists();
		}


		/// <summary>
		/// Registers the viewer.
		/// </summary>
		/// <param name="viewerId">The viewer id.</param>
		public static void RegisterChatEndPoint()
		{
			TableOperation retrieveOperation = TableOperation.Retrieve<ChatClientEntity>("x", "x");
			TableResult retrievedResult = chatClientTable.Execute(retrieveOperation);
			ChatClientEntity updateEntity = (ChatClientEntity)retrievedResult.Result;
			if (updateEntity != null)
			{
				updateEntity.WebRoleEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["WebApi"].IPEndpoint.ToString();

				// Create the InsertOrReplace TableOperation
				TableOperation insertOrReplaceOperation = TableOperation.Replace(updateEntity);

				// Execute the operation.
				chatClientTable.Execute(insertOrReplaceOperation);
			}
			else
			{
				ChatClientEntity insertEntity = new ChatClientEntity(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["WebApi"].IPEndpoint.ToString());

				// Create the InsertOrReplace TableOperation
				TableOperation insertOrReplaceOperation = TableOperation.Insert(insertEntity);

				// Execute the operation.
				chatClientTable.Execute(insertOrReplaceOperation);
			}
		}


		/// <summary>
		/// Gets the end point for the chat client
		/// </summary>
		/// <param name="agentId">The chat client id.</param>
		/// <returns></returns>
		public static string GetChatEndpoint()
		{
			// Create a retrieve operation that takes a customer entity.
			TableOperation retrieveOperation = TableOperation.Retrieve<ChatClientEntity>("x", "x");

			// Execute the retrieve operation.
			TableResult retrievedResult = chatClientTable.Execute(retrieveOperation);

			// Print the phone number of the result.
			if (retrievedResult.Result != null)
				return ((ChatClientEntity)retrievedResult.Result).WebRoleEndPoint;
			else
				throw new InvalidOperationException("Could not find the end point");
		}
	}

	/// <summary>
	/// Represents a chat client end point
	/// </summary>
	public class ChatClientEntity : TableEntity
	{
		public ChatClientEntity(string endPoint)
		{
			this.PartitionKey = "x";
			this.RowKey = "x";
			this.WebRoleEndPoint = endPoint;
		}

		public ChatClientEntity()
		{
		}

		public string WebRoleEndPoint { get; set; }

	}

}