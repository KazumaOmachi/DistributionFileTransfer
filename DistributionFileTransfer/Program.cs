using System;

namespace DistributionFileTransfer
{
	class MainClass
	{
		public static void Main(string[] args)
		{

			Console.WriteLine("Hello World!");
			FileExportController fileCtl = new FileExportController();
			DataCacheController cache = new DataCacheController();
			DataSenderController sender = new DataSenderController(cache);
			DataReceiverController receiver = new DataReceiverController(cache, fileCtl, sender);

			ManagerComunicationController manager = new ManagerComunicationController(receiver,cache,fileCtl,sender);
		}
	}
}
