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
		private TcpClient tcpClient;
		private NetworkStream netStream;
		//private System.Collections.Generic.List<TcpClient> TcpList;

		public NetWorkContoroller(TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
			this.netStream = tcpClient.GetStream();

			this.sndList = new ConcurrentQueue<DataObject>();
			this.rcvList = new ConcurrentQueue<DataObject>();
			//this.TcpList = new System.Collections.Generic.List<TcpClient>();

			// thread start
			System.Threading.ThreadPool.QueueUserWorkItem(sndMessage);
			System.Threading.ThreadPool.QueueUserWorkItem(rcvMessage);

		}
		public bool getStatus()
		{
			if (tcpClient != null)
			{
				return tcpClient.Connected;
			}
			else {
				return false;
			}
		}

		/*
		public void addTcpClient(TcpClient client)
		{
			lock (((ICollection)this.TcpList).SyncRoot)
			{
				this.TcpList.Add(client);
				System.Threading.ThreadPool.QueueUserWorkItem(rcvMessage,client);
			}
		}
		*/

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
			if (this.tcpClient.Connected)
			{
				DataObject data;
				if (sndList.TryDequeue(out data))
				{
					byte[] dataByte = data.getSendData();
					this.netStream.Write(dataByte, 0, dataByte.Length);
				}
				    
				System.Threading.Thread.Sleep(1);
				System.Threading.ThreadPool.QueueUserWorkItem(sndMessage);
			}
		}

		private void rcvMessage(object e)
		{
			//TcpClient client = (TcpClient)e;
			if (this.tcpClient.Connected)
			{
				byte[] dataLngByte = new byte[4];
				int resSize = this.netStream.Read(dataLngByte, 0, dataLngByte.Length);
				int dataLng = BitConverter.ToInt32(dataLngByte, 0);
				if (dataLng > 0)
				{
					Console.WriteLine("TCP data receive " + dataLng);

					byte[] dataTypeByte = new byte[4];
					int resType = this.netStream.Read(dataLngByte, 0, dataLngByte.Length);
					int typeInt = BitConverter.ToInt32(dataLngByte, 0);

					Console.WriteLine("data type : " + typeInt);

					byte[] dataMainByte = new byte[dataLng];
					int mainSize = this.netStream.Read(dataMainByte, 0, dataMainByte.Length);
					Console.WriteLine("main data size : " + mainSize);

					string dataMain = System.Text.Encoding.ASCII.GetString(dataMainByte);

					Console.WriteLine("Main data = " + dataMain);


				}
				System.Threading.Thread.Sleep(1);
				System.Threading.ThreadPool.QueueUserWorkItem(rcvMessage);//, client);
			}
		}

	}
}
