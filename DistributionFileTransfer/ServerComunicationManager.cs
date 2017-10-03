using System;
using DistributionFileTrasfer;

namespace DistributionFileTransfer
{
	public class ServerComunicationManager :AComunicationManager
	{
		private NetWorkContoroller tcpClient;
		public ServerComunicationManager(DataReceiverController receiver, string ip,int port)
		{
			this.isAct = true;
			this.receiver = receiver;
		}

		// データの受信と送信
		public new void dataReceivThreadAction(object e)
		{
 			while (this.isAct)
			{
				Console.WriteLine("ServerComunicationManager");

				DataObject data = this.tcpClient.getRcvMessage();

				if (data != null)
				{
					this.receiver.setSendData(null);
				}
				System.Threading.Thread.Sleep(1000);
			}
		}


	}
}
