using System;
namespace DistributionFileTransfer
{
	public class ServerComunicationManager :AComunicationManager
	{

		public ServerComunicationManager()
		{
			this.isAct = true;
		}

		public new void dataReceivThreadAction(object e)
		{
			while (this.isAct)
			{
				Console.WriteLine("ServerComunicationManager");
				System.Threading.Thread.Sleep(1000);
			}
		}


	}
}
