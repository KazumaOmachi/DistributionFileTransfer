using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Concurrent;

namespace DistributionFileTrasfer
{

	public class NetWorkContoroller
	{
		private ConcurrentQueue<DataObject> sndList;
		private ConcurrentQueue<DataObject> rcvList;
		private System.Collections.Generic.List<TcpClient> TcpList;

		public NetWorkContoroller()
		{
			this.sndList = new ConcurrentQueue<DataObject>();
			this.rcvList = new ConcurrentQueue<DataObject>();
			this.TcpList = new System.Collections.Generic.List<TcpClient>();

			// thread start
			System.Threading.ThreadPool.QueueUserWorkItem(sndMessage);

		}

		public void addTcpClient(TcpClient client)
		{
			lock (((ICollection)this.TcpList).SyncRoot)
			{
				this.TcpList.Add(client);
				System.Threading.ThreadPool.QueueUserWorkItem(rcvMessage,client);
			}
		}

		public void setSndMessage(DataObject obj)
		{
			if (obj != null)
			{
				this.sndList.Enqueue(obj);
			}
		}

		public DataObject getRcvMessage()
		{
			DataObject obj = null;
			if (this.rcvList.TryDequeue(out obj)) { }
			return obj;
		}

		// tcp
		private void sndMessage(object e)
		{

			System.Threading.ThreadPool.QueueUserWorkItem(sndMessage);
		}

		private void rcvMessage(object e)
		{
			TcpClient client = (TcpClient)e;
			if (client.Connected)
			{
				
				System.Threading.ThreadPool.QueueUserWorkItem(rcvMessage, client);
			}
		}

	}
}
