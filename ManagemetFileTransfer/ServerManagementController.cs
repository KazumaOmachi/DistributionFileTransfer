using System;
using System.Collections.Generic;
using DistributionFileTrasfer;
using System.Collections;
using System.Collections.Concurrent;


namespace ManagemetFileTransfer
{
	public class ServerManagementController
	{

		List<ServerNodeManager> severConnetList;

		private ConcurrentDictionary<string, ServerStatus> serverStatus;
		//ConcurrentDictionary<string, ServerStatus> serverStasutDict;
		bool isStat;
		int clientPort = 6901;

		public ServerManagementController()
		{
			this.isStat = true;
			this.severConnetList = new List<ServerNodeManager>();
			this.serverStatus = new ConcurrentDictionary<string, ServerStatus>();
			// head nodeのコネクション
			for (int i = this.severConnetList.Count - 1; i >= 0; i--)
			{
				if (this.severConnetList[i].getConnectionCount() > 0)
				{
					DataObject data = null;
					// 最終コネクションでない場合はスキップ
					if (i == 0)
					{
						//this.severConnetList[i].setHeadNodeConnection("client:" + clientPort);
						data = new DataObject(MessageTypeEnum.ConnectInf, "client:" + clientPort);
					}
					else
					{
						data = new DataObject(MessageTypeEnum.ConnectInf, this.severConnetList[i - 1].getHeadNodeInfo());
						string key = this.severConnetList[i - 1].getHeadNodeInfo();
						ServerStatus tmpSv = new ServerStatus(key);
						this.serverStatus.TryAdd(key, tmpSv);
						//string toInf = this.severConnetList[i - 1].getHeadNodeInfo();
						//this.severConnetList[i].setHeadNodeConnection(toInf);
					}
					if (data != null)
					{
						this.severConnetList[i].sendHeadNodeMessage(data);
					}
				}
				else
				{
					this.severConnetList.Remove(this.severConnetList[i]);
				}

			}
			System.Threading.Thread finishDataCheck = new System.Threading.Thread(checkFinishData);
			finishDataCheck.Start();

		}

		// マネージャサーバ間のコネクションチェック
		private void checkConnectionThread(object e)
		{
			lock (((ICollection)this.severConnetList).SyncRoot)
			{
				for (int i = 0; i < this.severConnetList.Count; i ++)
				{
					if (this.severConnetList[i].getHeadNodeStatus())
					{
						if (i != 0)
						{
							//this.severConnetList[i].sendHeadNodeMessage();
							DataObject data = new DataObject(MessageTypeEnum.GetConnectStatus);
							this.severConnetList[i].sendHeadNodeMessage(data);
						}
					}
					else {
						Console.WriteLine("ERROR");
					}
				}
			}
		}

		private void getHeadNodeReceiveThread(object e)
		{
			lock (((ICollection)this.severConnetList).SyncRoot)
			{
				for (int i = 1; i < this.severConnetList.Count; i++)
				{
					DataObject rcData = this.severConnetList[i].getHeadNodeMessage();

					string svStatKey = this.severConnetList[i - 1].getHeadNodeInfo();
					if (rcData.messageType == MessageTypeEnum.OKConnected)
					{
						this.serverStatus[svStatKey].status = true;

					}
					else if (rcData.messageType == MessageTypeEnum.ConnectFail)
					{
						this.serverStatus[svStatKey].status = false;
					}

					DataObject data = new DataObject(MessageTypeEnum.GetConnectStatus);
					this.severConnetList[i].sendHeadNodeMessage(data);

				}
			}

		}

		// 最終データの取得
		private void checkFinishData(object e)
		{
			Dictionary<int, int> prevFinishKeyList = new Dictionary<int, int>();
			while (this.isStat)
			{
				int numOfSev = 0;
				Dictionary<int, int> finishKeyList = new Dictionary<int, int>();
				lock (((ICollection)this.severConnetList).SyncRoot)
				{
					numOfSev = this.severConnetList.Count;
					foreach (ServerNodeManager servNdMg in this.severConnetList)
					{
						foreach (int i in servNdMg.getFinishKeyList())
						{
							if (!finishKeyList.ContainsKey(i))
							{
								finishKeyList.Add(i, 0);

							}
							finishKeyList[i]++;
						}
					}
				}
				foreach (int key in finishKeyList.Keys)
				{
					// 件数とコネクション数が決まっている
					if (numOfSev == finishKeyList[key])
					{
						// 既に送信していればスキップ
						if (!prevFinishKeyList.ContainsKey(key))
						{
							//severConnetList[0].sendHeadNodeMessage(null);
						}
					}

				}
				prevFinishKeyList = finishKeyList;
				System.Threading.Thread.Sleep(10);
			}
		}




	}
}
