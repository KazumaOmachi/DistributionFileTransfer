using System;

namespace DistributionFileTransfer
{
	class MainClass
	{
		public static void Main(string[] args)
		{

			Console.WriteLine("Hello World!");

			/* Debug */
			// server -> server
			/*
			int connectPort;
			int recievePort;
			if (args.Length > 0)
			{
				// サーバプロセス
				connectPort = 6501;
				recievePort = 6502;
			}
			else {
				connectPort = -1; // for Client
				recievePort = 6501;
			}
			*/
			int recievePort = 6501;


			FileExportController fileCtl = new FileExportController();
			DataCacheController cache = new DataCacheController();

			DataSenderController sender = new DataSenderController(cache, recievePort);
			DataReceiverController receiver = new DataReceiverController(cache, fileCtl, sender);

			ManagerComunicationController manager = new ManagerComunicationController(receiver,cache,fileCtl,sender, 6901);

			/* Debug */
			// クライアント用ポートの設定
			/*
			if (connectPort > 0)
			{
				// 接続先IPポートを設定
				manager.startServerComumnication("127.0.0.1", connectPort);
			}
			else 
			{
				manager.startClietCommunication(6001);

			}
			*/
		}
	}
}
