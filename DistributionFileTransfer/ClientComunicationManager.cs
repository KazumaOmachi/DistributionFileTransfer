using System;
namespace DistributionFileTransfer
{
	public class ClientComunicationManager :AComunicationManager
	{
		
		
		public ClientComunicationManager(DataReceiverController receiver)
		{
			this.isAct = true;
			this.receiver = receiver;
		}

		public new void dataReceivThreadAction(object e)
		{
			while (this.isAct)
			{
				Console.WriteLine("ClientComunicationManager");

				this.receiver.setSendData(null);

				System.Threading.Thread.Sleep(1000);
			}
		}


	}
}
