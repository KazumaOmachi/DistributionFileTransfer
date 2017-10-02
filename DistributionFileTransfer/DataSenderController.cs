using System;
using System.Collections.Concurrent;
using DistributionFileTrasfer;
using System.Collections.Generic;
using System.Collections;

namespace DistributionFileTransfer
{
	public class DataSenderController
	{
		private System.Threading.Thread dataSenderThread;
		private ConcurrentQueue<DataObject> dataQue;
		private DataCacheController dataCache;
		private List<NetWorkContoroller> dataSenderList;
		private bool isAct;

		public DataSenderController(DataCacheController dataCache)
		{
			this.isAct = true;
			this.dataCache = dataCache;
			this.dataSenderList = new List<NetWorkContoroller>();
			this.dataQue = new ConcurrentQueue<DataObject>();

			this.dataSenderThread = new System.Threading.Thread(dataSenderThreadAction);
			this.dataSenderThread.Start();

			// TCP Listenerの起動
			System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);

		}

		// アクセプト
		private void acceptTcpConnection(object e)
		{
			// 
			if (this.isAct)
			{
				System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);
			}

			lock (((ICollection)this.dataSenderList).SyncRoot)
			{

				List<DataObject> cacheData = this.dataCache.getAllDataList();
				foreach (DataObject data in cacheData)
				{
					// データの送信

				}
			}
				/*
			this.dataSenderThread.Start();
			*/
		}



		// データ送信用のスレッド
		private void dataSenderThreadAction(object e)
		{
			while (this.isAct)
			{
				DataObject data;
				if (this.dataQue.TryDequeue(out data))
				{
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
			
		}
	}
}
