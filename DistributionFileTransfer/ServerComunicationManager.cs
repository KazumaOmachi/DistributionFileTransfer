using System;
using DistributionFileTrasfer;

using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;

namespace DistributionFileTransfer
{
	public class ServerComunicationManager :AComunicationManager
	{
		private NetWorkContoroller tcpClient;
		private System.Threading.Thread dataReceiveThread;

		public ServerComunicationManager(DataReceiverController receiver, string ip,int port)
		{
			
			this.isAct = true;
			this.receiver = receiver;
			for (int i = 0; i < 10;i++)
			{
				try
				{
					Console.WriteLine("conntect to " + ip + ":" + port);
					TcpClient tcp = new TcpClient(ip, port);
					this.tcpClient = new NetWorkContoroller(tcp);
					this.dataReceiveThread = new System.Threading.Thread(dataReceivThreadAction);
					this.dataReceiveThread.Start();
				}
				catch
				{
					this.isAct = false;
					System.Threading.Thread.Sleep(1000);
					Console.WriteLine("connction filed");
				}
			}
		}

		// データの受信と送信
		public new void dataReceivThreadAction(object e)
		{
			Console.WriteLine("ServerComunicationManager");

			while (this.isAct)
			{

				DataObject data = this.tcpClient.getRcvMessage();

				if (data != null)
				{
					// データの受信
					this.receiver.setSendData(data);
				}
				System.Threading.Thread.Sleep(10);
			}
		}


	}
}
