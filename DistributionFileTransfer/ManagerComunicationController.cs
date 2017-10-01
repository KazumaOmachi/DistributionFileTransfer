using System;
using DistributionFileTrasfer;

namespace DistributionFileTransfer
{
	public class ManagerComunicationController
	{

		private System.Threading.Thread dataReceiveThread;
		private AComunicationManager comManag;

		public ManagerComunicationController() {
			this.comManag = new AComunicationManager();
			this.dataReceiveThread = new System.Threading.Thread(this.comManag.dataReceivThreadAction);
		}
		/*
		DataReceiverController dataReciv,
                				   			 DataCacheController dataCache,
									  		 FileExportController fileExport,
									  		 DataSenderController dataSender)
		{
			
		}
		*/

		// 
		private void receiveManagemetMessage(object e)
		{

		}

		public void setDataReceiveThreadStarter(bool stat)
		{
			while (this.dataReceiveThread.IsAlive)
			{
				this.comManag.tofalse();
				System.Threading.Thread.Sleep(10);
			}

			if (stat)
			{
				this.comManag = new ServerComunicationManager();
			}
			else
			{
				this.comManag = new ClientComunicationManager();
			}
			this.dataReceiveThread = new System.Threading.Thread(this.comManag.dataReceivThreadAction);

			this.dataReceiveThread.Start();

		}
	}
}
