using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace DistributionFileTransfer
{
	public class ClientComunicationManager :AComunicationManager
	{

		private ConcurrentDictionary<int, List<NetWorkContoroller>> clientList;
		private TcpListener listener_;


		public ClientComunicationManager(DataReceiverController receiver, int port)
		{
			this.receiver = receiver;
			this.clientList = new ConcurrentDictionary<int, List<NetWorkContoroller>>();
		

			// 
			string ipString = "0.0.0.0";
			IPAddress ipAdd = IPAddress.Parse(ipString);
			//int port = 6001;

			this.listener_ = new TcpListener(ipAdd, port);
			this.listener_.Start();

			System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);
		}

		// データの削除
		public void removeClient(int key)
		{
			List<NetWorkContoroller> list = new List<NetWorkContoroller>();
			clientList.TryRemove(key, out list);
			foreach (NetWorkContoroller cli in list)
			{
				// コネクション切断
			}
		}

		// クライアント接続の受け入れ
		private void acceptTcpConnection(object e)
		{
			
			TcpClient client = this.listener_.AcceptTcpClient();
			System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);
			Console.WriteLine("client connet");

			// create tcp connecton
			NetWorkContoroller master = new NetWorkContoroller(client);

			int key = master.getPort();
			string ip = master.getIp();
			List<NetWorkContoroller> tcplist = new List<NetWorkContoroller>();
			// 複数コネクションを作成
			while (true)
			{
				// Console.WriteLine("wait get connection list ");
				DataObject data = master.getRcvMessage();
				if (data != null)
				{
					Console.WriteLine(data.messageType + " / " + data.dataStr);
					foreach (string portStr in data.dataStr.Split(','))
					{
						Console.WriteLine("port --> " + portStr);
						int port = Int32.Parse(portStr);

						Console.WriteLine("master ; " + ip + ":" + key + " /  conntect to " + ip + ":" + port);
						TcpClient tcp = new TcpClient(ip, port);
						tcplist.Add(new NetWorkContoroller(tcp));

					}
					break;
				}
				System.Threading.Thread.Sleep(1);
				//tcplist.Add
			}

			this.clientList.TryAdd(key, tcplist);
			System.Threading.ThreadPool.QueueUserWorkItem(dataReceverThread, key);

		}

		// データの受信オブジェクト
		private void dataReceverThread(object e)
		{
			int key = (int)e;
			Console.WriteLine("start Client connection. key : " + key);
			int n = 0;
			while (true)
			{
				if (this.clientList.ContainsKey(key))
				{
					List<NetWorkContoroller> cnectList = this.clientList[key];

					if (cnectList.Count > 0 )
					{
						int cnt = n % cnectList.Count;
						DataObject data = cnectList[cnt].getRcvMessage();
						if (data != null)
						{
							data.key = key;
							this.receiver.setSendData(data);
						
							if (data.messageType == MessageTypeEnum.FileFinish)
							{
								Console.WriteLine("finish this file transfer");
								break;
							}
							else {
								Console.WriteLine("Client data Recieve : " + data.key + " / " + data.seqNo + " / " + data.dataByte.Length);

							}
							n++;
						}
					}


				}
				else
				{
					break;
				}
				System.Threading.Thread.Sleep(10);
			}
			System.Threading.ThreadPool.QueueUserWorkItem(dataReceverThread, key);
		}


	}
}
