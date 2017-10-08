using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DistributionFileTransfer
{
	public class ClientComunicationManager :AComunicationManager
	{

		private ConcurrentDictionary<string, List<NetWorkContoroller>> clientList;

		public ClientComunicationManager(DataReceiverController receiver)
		{
			this.receiver = receiver;
			this.clientList = new ConcurrentDictionary<string, List<NetWorkContoroller>>();
			System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);

		}

		// データの削除
		public void removeClient(string key)
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
			System.Threading.ThreadPool.QueueUserWorkItem(acceptTcpConnection);


			// create tcp connecton
			NetWorkContoroller master = new NetWorkContoroller(null);

			List<NetWorkContoroller> tcplist = new List<NetWorkContoroller>();
			// 複数コネクションを作成
			while (true)
			{
				DataObject data = master.getRcvMessage();
				break;
				//tcplist.Add
			}
			string key = "";
			this.clientList.TryAdd(key, tcplist);
			System.Threading.ThreadPool.QueueUserWorkItem(dataReceverThread, key);

		}

		// データの受信オブジェクト
		private void dataReceverThread(object e)
		{
			string key = e as string;

			int n = 0;
			while (true)
			{
				if (this.clientList.ContainsKey(key))
				{
					List<NetWorkContoroller> cnectList = this.clientList[key];

					int cnt = n % cnectList.Count;
					DataObject data = cnectList[cnt].getRcvMessage();
					if (data != null)
					{
						this.receiver.setSendData(data);
						n++;
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
