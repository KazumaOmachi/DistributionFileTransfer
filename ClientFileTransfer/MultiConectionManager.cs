using System;

using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using DistributionFileTrasfer;

namespace ClientFileTransfer
{



	public class MultiConectionManager
	{
		private int[] opnPortList;
		private NetWorkContoroller[] tcpList;
		//private bool isRuning;
		//private string ip;
		//private int port;
		//private int portCnt;

		public MultiConectionManager() { }

		//int portCnt, string ip, int port, DataObject[] splitFileList)

			//this.portCnt = portCnt;
			//this.ip = ip;
			//this.port = port;
			// = "127.0.0.1";
			//int port = 6001;
			/*
			System.Threading.ThreadPool.QueueUserWorkItem(runServerConnectionThread);
		}

		private void runServerConnectionThread(object e)
		{
			DataObject[] splitFileList = e as DataObject[];

			this.isRuning = true;
			*/
		public bool run(int portCnt, string ip, int port, DataObject[] splitFileList)
		{
			bool status = true;
			// サーバへの接続
			try
			{

				Console.WriteLine("conntect to " + ip + ":" + port);
				TcpClient tcp = new TcpClient(ip, port);
				NetWorkContoroller masterConnection = new NetWorkContoroller(tcp);



				Console.WriteLine("connected");


				// コネクション用ポートの開始
				this.opnPortList = new int[portCnt];
				this.tcpList = new NetWorkContoroller[portCnt];
				for (int i = 0; i < portCnt; i++)
				{
					System.Threading.ThreadPool.QueueUserWorkItem(startMultiConnection, i);
				}

				// 解放されたポートを取得
				string portList = getStartPortList(portCnt);


				Console.WriteLine("port data list : " + portList);
				DataObject data = new DataObject(MessageTypeEnum.ConnectList, portList);
				masterConnection.setSndMessage(data);

				connectionStartCheck(portCnt);


				// データの送信
				int num = 0;
				lock (((ICollection)this.opnPortList).SyncRoot)
				{
					foreach (DataObject sendData in splitFileList)
					{
						tcpList[num % portCnt].setSndMessage(sendData);
						num++;
					}
				}
				DataObject fileFinish = new DataObject(MessageTypeEnum.FileFinish, 0);
				//num++;
				tcpList[num % portCnt].setSndMessage(fileFinish);

				// 接続待ち（サーバから複数接続）
				disconnectionStartCheck();

				// 終了まち・タイムアウト設定

				Console.WriteLine("file transfer compleat");
			}
			catch (Exception E)
			{
				Console.WriteLine("Error no connection " + E.ToString());
			}
			return status;
			//this.isRuning = false;

		}
		// サーバからの複数接続まち
		private void startMultiConnection(object e)
		{
			int index = (int)e;
			Console.WriteLine("start index " + index);
			string ipString = "0.0.0.0";
			IPAddress ipAdd = IPAddress.Parse(ipString);

			TcpListener listener = new TcpListener(ipAdd, 0);
			listener.Start();

			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			Console.WriteLine("open port : " + port);
			lock (((ICollection)this.opnPortList).SyncRoot)
			{
				this.opnPortList[index] = port;
			}
			TcpClient client = listener.AcceptTcpClient();
			Console.WriteLine("server connected");
			lock (((ICollection)this.tcpList).SyncRoot)
			{
				this.tcpList[index] = new NetWorkContoroller(client);
			}
		}

		// 開始されたポートのリストを取得
		private string getStartPortList(int portCnt)
		{
			// ポートオープンチェック
			while (true)
			{
				int portActCnt = 0;
				lock (((ICollection)this.opnPortList).SyncRoot)
				{
					foreach (int n in this.opnPortList)
					{
						if (n > 0)
						{
							portActCnt++;
						}
					}
				}
				if (portActCnt == portCnt)
				{
					break;
				}
				System.Threading.Thread.Sleep(10);
			}

			string portList = "";
			lock (((ICollection)this.opnPortList).SyncRoot)
			{
				portList = String.Join(",", this.opnPortList);
			}
			Console.WriteLine(portList);

			return portList;
		}




		// コネクションのチェック
		private void connectionStartCheck(int portCnt)
		{
			// コネクションチェッック
			while (true)
			{
				//Console.WriteLine("connection check");
				int cnnctCnt = 0;
				lock (((ICollection)this.tcpList).SyncRoot)
				{
					foreach (NetWorkContoroller nwCtn in this.tcpList)
					{
						if (nwCtn != null)
						{
							if (nwCtn.getStatus())
							{
								// コネクションカウント
								cnnctCnt++;
							}
						}
					}
				}
				//Console.WriteLine("connection count : " + cnnctCnt);
				if (cnnctCnt == portCnt)
				{
					break;
				}
				System.Threading.Thread.Sleep(10);
			}
			Console.WriteLine("connected all connection");
		}

		// コネクションのチェック
		private void disconnectionStartCheck()
		{
			while (true)
			{
				int clsCnt = 0;
				lock (((ICollection)this.tcpList).SyncRoot)
				{
					foreach (NetWorkContoroller nwCtn in this.tcpList)
					{
						if (!nwCtn.getStatus())
						{
							clsCnt++;
						}
					}
				}
				lock (((ICollection)this.tcpList).SyncRoot)
				{
					if (this.tcpList.Length == clsCnt)
					{
						Console.WriteLine("All sesstion disconnect");
						break;
					}
				}
				System.Threading.Thread.Sleep(10);
			}

		}
	}
}
