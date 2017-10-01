using System;
using DistributionFileTrasfer;

namespace DistributionFileTransfer
{
	public class TcpNetWorkConnector :ITcpNetWorkManager
	{
		private NetWorkContoroller tcpClient;

		public TcpNetWorkConnector(string ip , int port)
		{
			
		}

		public void setSndMessage(DataObject data)
		{
			this.tcpClient.setSndMessage(data);
		}

		public DataObject getRcvMessage()
		{
			return this.tcpClient.getRcvMessage();
		}
	}
}
