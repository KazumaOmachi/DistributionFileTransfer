using System;
namespace DistributionFileTransfer
{
	public class ClientComunicationManager :AComunicationManager
	{
		
		
		public ClientComunicationManager()
		{
			this.isAct = true;
		}

		public new void dataReceivThreadAction(object e)
		{
			while (this.isAct)
			{
				Console.WriteLine("ClientComunicationManager");
				System.Threading.Thread.Sleep(1000);
			}
		}


	}
}
