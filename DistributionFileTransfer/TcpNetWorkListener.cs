using System;
using DistributionFileTrasfer;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace DistributionFileTransfer
{
	public class TcpNetWorkListener : ITcpNetWorkManager
	{

		private List<ITcpNetWorkManager> dataSenderList;
		private ConcurrentQueue<DataObject> dataList;

		public TcpNetWorkListener()
		{
		}

		public void setSndMessage(DataObject data)
		{
			this.dataList.Enqueue(data);
		}

		public DataObject getRcvMessage()
		{
			return this.tcpClient.getRcvMessage();
		}
	}
}
