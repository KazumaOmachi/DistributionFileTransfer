using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using DistributionFileTrasfer;
using System.Collections;

namespace ClientFileTransfer
{
	public class FileTransferController
	{
		private int[] opnPortList;
		private NetWorkContoroller[] tcpList;

		public FileTransferController() { }

		public void run(string filePath)
		{

			if (System.IO.File.Exists(filePath))
			{
				// ファイルをローカルにコピー
				string fileName = System.IO.Path.GetFileName(filePath);
				Console.WriteLine(fileName);
				int id = System.Diagnostics.Process.GetCurrentProcess().Id;
				string tmpPath = @"/tmp/test/" + id + "/";
				System.IO.Directory.CreateDirectory(tmpPath);
				System.IO.File.Copy(filePath, tmpPath + fileName);

				// 圧縮

				//ファイルのバイト変換
				System.IO.FileStream fs = new System.IO.FileStream(
					tmpPath + fileName,
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read);
				byte[] bs = new byte[fs.Length];
				fs.Read(bs, 0, bs.Length);
				fs.Close();

				Console.WriteLine("file read : " + bs.Length + "(byte)");

				Console.WriteLine(fs);

				// サーバへの接続
				try
				{
					string ip = "127.0.0.1"; 
					int port = 6001;
					Console.WriteLine("conntect to " + ip + ":" + port);
					TcpClient tcp = new TcpClient(ip, port);
					NetWorkContoroller masterConnection = new NetWorkContoroller(tcp);



					Console.WriteLine("connected");


					// コネクション用ポートの開始
					int portCnt = 3;
					this.opnPortList = new int[portCnt];
					this.tcpList = new NetWorkContoroller[portCnt];
					for (int i = 0; i < portCnt; i++)
					{
						System.Threading.ThreadPool.QueueUserWorkItem(startMultiConnection, i);
					}

					// 解放されたポートを取得
					string portList = getStartPortList(portCnt);


					Console.WriteLine("port data list : " + portList );
					DataObject data = new DataObject();
					data.setData(portList);
					masterConnection.setSndMessage(data);

					connectionStartCheck(portCnt);



					// 分割・データの作成



					// TCPの複数ポート作成





					// 接続待ち（サーバから複数接続）

					// 終了まち・タイムアウト設定

					// ファイルの削除
					System.IO.Directory.Delete(tmpPath, true);
					Console.WriteLine("file transfer compleat");
				}
				catch (Exception E)
				{
					Console.WriteLine("Error no connection " + E.ToString());
				}
			}
			else {
				Console.WriteLine("file not exist");
			}

		}

		// サーバからの複数接続まち
		private void startMultiConnection(object e)
		{
			int index = (int)e ;
			Console.WriteLine("start index " + index);
			string ipString = "0.0.0.0";
			IPAddress ipAdd = IPAddress.Parse(ipString);

			TcpListener listener = new TcpListener(ipAdd, 0 );
			listener.Start();

			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			Console.WriteLine("open port : " + port);
			lock (((ICollection)this.opnPortList).SyncRoot)
			{
				this.opnPortList[index] = port;
			}
			TcpClient client  = listener.AcceptTcpClient();
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
						Console.WriteLine("connection check");
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
						Console.WriteLine("connection count : " + cnnctCnt);
						if (cnnctCnt == portCnt)
						{
							break;
						}
						System.Threading.Thread.Sleep(1000);
					}
		}
	}
}
