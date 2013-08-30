AzureSignalRInteration
======================

Demonstration of Azure and SignalR integration

This application will open a SignalR connection to an Azure running with multiple instances. Azure will record in an Azure table storage the end point on which this connection was established. 

Four things then happen
 1. The browser will periodically send a message to Azure which the Azure instance will reply to
 2. The Azure instance that received the connection request will periodically send a message to the client
 3. The user can elect to send a message from the browser to the Azure instance which will then reply.
 4. The user can elect to send a message from the browser to the Azure instance which will then use WebApi to send the message onto the instance that the SignalR connection was established on, and then have the message sent back to the client.

What do you think will happen?

SignalR may work differently to what you think....  Download to find out.
