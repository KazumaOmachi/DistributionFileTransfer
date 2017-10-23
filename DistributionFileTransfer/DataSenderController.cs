using System;
using System.Collections.Concurrent;
using DistributionFileTrasfer;
using System.Collections.Generic;
using System.Collections;

using System.Net.Sockets;
using System.Net;

namespace DistributionFileTransfer
{
	public class DataSenderController
	{
		private System.Threading.Thread dataSenderThread;
		private ConcurrentQueue<DataObject> dataQue;
		private DataCacheController dataCache;
		private List<NetWorkContoroller> dataSenderList;
		private bool isAct;

		private TcpListener listener_;

		public DataSenderController(DataCacheController dataCache , int port)
		{
			this.isAct = true;
			this.dataCache = dataCache;
			this.dataSenderList = new List<NetWorkContoroller>();
			this.dataQue = new ConcurrentQueue<DataObject>();

			string ipString = "0.0.0.0";
			IPAddress ipAdd = IPAddress.Parse(ipString);

			this.listener_ = new TcpListener(ipAdd, port);
			this.listener_.Start();


			// TCP Listenerの起動
			System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);

			// データ送信スレッドの起動
			this.dataSenderThread = new System.Threading.Thread(dataSenderThreadAction);
			this.dataSenderThread.Start();


		}

		// アクセプト
		private void acceptTcpConnection(object e)
		{
			// 
			if (this.isAct)
			{

				TcpClient client = this.listener_.AcceptTcpClient();

				// create tcp connecton
				NetWorkContoroller dataSender = new NetWorkContoroller(client);
				System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);


				lock (((ICollection)this.dataSenderList).SyncRoot)
				{
					// 新規コネクションに対してキャッシュデータの送信
					List<DataObject> cacheData = this.dataCache.getAllDataList();
					foreach (DataObject data in cacheData)
					{
						// データの送信
						dataSender.setSndMessage(data);
					}
					// 送信先リストに追加
					this.dataSenderList.Add(dataSender);
				}
			}
		}



		// データ送信用のスレッド
		private void dataSenderThreadAction(object e)
		{
			Console.WriteLine("start data sender thread ");
			while (this.isAct)
			{
				DataObject data;
				if (this.dataQue.TryDequeue(out data))
				{
					Console.WriteLine("Send data key : " + data.key);
					lock (((ICollection)this.dataSenderList).SyncRoot)
					{
						foreach (NetWorkContoroller tcpClient in this.dataSenderList)
						{
							tcpClient.setSndMessage(data);
						}
					}
				}
				System.Threading.Thread.Sleep(1);
			}

		}

		// 終了処理
		public void Dispose()
		{
			lock (this)
			{
				this.isAct = false;
			}
		}

		public void setDataObject(DataObject data)
		{

			Console.WriteLine("DataSender :set send data list");
			this.dataQue.Enqueue(data);

		}
	}
}
