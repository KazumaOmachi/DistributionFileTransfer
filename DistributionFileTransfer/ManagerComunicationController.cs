using System;
using DistributionFileTrasfer;

namespace DistributionFileTransfer
{
	public class ManagerComunicationController
	{

		private System.Threading.Thread dataReceiveThread;
		private System.Threading.Thread dataManagementThread;

		private AComunicationManager comManag;
		private	FileExportController fileExport;
		private DataCacheController dataCache;
		private DataReceiverController dataReciv;
		private DataSenderController dataSend;

		private NetWorkContoroller managent;

		public ManagerComunicationController(DataReceiverController dataReciv, 
		                                     DataCacheController dataCache,
		                                     FileExportController fileExport,
		                                     DataSenderController dataSend
		                                     )

		{
			this.comManag = new AComunicationManager();
			// dataReceiveThreadの初期化
			this.dataReceiveThread = new System.Threading.Thread(this.comManag.dataReceivThreadAction);

			this.dataSend = dataSend;

			// management受け入れのスレッドの起動
			System.Threading.ThreadPool.QueueUserWorkItem(acceptManagemet);
		}


		private void acceptManagemet(object e)
		{
			System.Threading.Thread.Sleep(1000);
			System.Threading.ThreadPool.QueueUserWorkItem(acceptManagemet);
		}

		// 
		private void dataManagementThreadAction(object e)
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
				// サーバプロセス用
				string ip = "";
				int port = 0;
				this.comManag = new ServerComunicationManager(this.dataReciv,ip, port);
			}
			else
			{
				// クライアントプロセス用
				this.comManag = new ClientComunicationManager(this.dataReciv);
			}
			this.dataReceiveThread = new System.Threading.Thread(this.comManag.dataReceivThreadAction);
			this.dataCache.resetAllCacheData();

			this.dataReceiveThread.Start();

		}
	}
}
