using System;
namespace DistributionFileTransfer
{
	public class AComunicationManager
	{
		protected bool isAct;
		protected DataReceiverController receiver;

		public AComunicationManager()
		{
		}

		public void tofalse()
		{
			lock (this)
			{
				this.isAct = false;

			}
		}

		public void dataReceivThreadAction(object e)
		{
			
		}
	}
}
