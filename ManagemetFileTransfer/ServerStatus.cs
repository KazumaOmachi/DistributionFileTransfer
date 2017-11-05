using System;
namespace ManagemetFileTransfer
{
	public class ServerStatus
	{
		public string connectTo;
		public bool status;

		public ServerStatus(string connectTo)
		{
			this.connectTo = connectTo;
			this.status = true;
		}
	}
}
