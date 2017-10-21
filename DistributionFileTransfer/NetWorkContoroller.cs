using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DistributionFileTrasfer
{

	public class NetWorkContoroller
	{
		private ConcurrentQueue<byte[]> sndList;
		private ConcurrentQueue<byte[]> rcvList;
		private TcpClient tcpClient;
		private NetworkStream netStream;
		//private System.Collections.Generic.List<TcpClient> TcpList;

		public NetWorkContoroller(TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
			this.netStream = tcpClient.GetStream();

			this.sndList = new ConcurrentQueue<byte[]>();
			this.rcvList = new ConcurrentQueue<byte[]>();

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

		public int getPort()
		{
			return  ((System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint).Port;
		}

		public string getIp()
		{
			return ((System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
		}

		public void setSndMessage(DataObject obj)
		{
			if (obj != null)
			{
				this.sndList.Enqueue(obj.getSendData());
			}
		}

		public DataObject getRcvMessage()
		{
			DataObject obj = null;
			byte[] data = null;
			if (this.rcvList.TryDequeue(out data)) {
				obj = new DataObject(data);
			}
			return obj; // データオブジェクトの作成
		}

		// tcp
		private void sndMessage(object e)
		{
			if (this.tcpClient.Connected)
			{
				byte[] data;
				if (sndList.TryDequeue(out data))
				{
					// データ長を含めて伝送
					List<byte> sendData = new List<byte>();
					byte[] dataLeng = BitConverter.GetBytes(data.Length);

					sendData.AddRange(dataLeng);
					sendData.AddRange(data);

					byte[] dataByte = sendData.ToArray();
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
				// データ長を取得して取り込み
				byte[] dataLngByte = new byte[4];
				int resSize = this.netStream.Read(dataLngByte, 0, dataLngByte.Length);
				int dataLng = BitConverter.ToInt32(dataLngByte, 0);
				if (dataLng > 0)
				{
					Console.WriteLine("TCP data receive " + dataLng);

					byte[] data = new byte[dataLng];
					this.netStream.Read(data, 0, data.Length);
					this.rcvList.Enqueue(data);
				}
				System.Threading.Thread.Sleep(1);
				System.Threading.ThreadPool.QueueUserWorkItem(rcvMessage);//, client);
			}
		}

	}
}
