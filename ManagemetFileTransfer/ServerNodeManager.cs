using System;
using System.Collections.Generic;
using DistributionFileTrasfer;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace ManagemetFileTransfer
{
	public class ServerNodeManager
	{
		private List<NetWorkContoroller> serverList;
		private ConcurrentDictionary<string, ServerStatus> serverStatus;
		private List<int> finishKetList;
		private int retryCnt = 7;

		public ServerNodeManager(List<string> connectList)
		{
			this.serverList = new List<NetWorkContoroller>();
			this.serverStatus = new ConcurrentDictionary<string, ServerStatus>();
			this.finishKetList = new List<int>();

			Parallel.ForEach(connectList, conectInf =>
			{
				string[] conInfList = conectInf.Split(':');
				string ip = conInfList[0];
				int port = Int32.Parse(conInfList[1]);

				// サーバへ接続
				NetWorkContoroller serverConnection = connectToServer(ip, port);
				if (serverConnection.getStatus())
				{
					this.serverList.Add(serverConnection);


				}
			});

			// サーバ間のコネクション確立
			//==> 0番目（head node）には接続情報を送らないため「i > 0」を設定
			for (int i = this.serverList.Count - 1; i > 0; i--)
			{
				string connectionInfo = createConnection(i);
				if (connectionInfo != null)
				{
					DataObject data = new DataObject(MessageTypeEnum.ConnectInf, connectionInfo);
					this.serverList[i].setSndMessage(data);

					ServerStatus tmpSvStat = new ServerStatus(connectionInfo);
					string svStatKey = this.serverList[i].getIp() + ":" + this.serverList[i].getPort();
					this.serverStatus.TryAdd(svStatKey, tmpSvStat);
				}
			}

			// 最終データ取得用メッセージ
			getFinalTransferInfoRequest();

			System.Threading.ThreadPool.QueueUserWorkItem(checkConnectionThread);
			System.Threading.ThreadPool.QueueUserWorkItem(dataRecievThread);
		}

		// マネージャからサーバへの接続
		private NetWorkContoroller connectToServer(string ip, int port)
		{
			NetWorkContoroller serverConnection = null;
			// リトライ
			for (int i = 0; i < this.retryCnt; i++)
			{
				try
				{
					Console.WriteLine("connection to. count --> " + i + 1 + "/" + retryCnt);
					TcpClient tcp = new TcpClient(ip, port);
					serverConnection = new NetWorkContoroller(tcp);
					break;
				}
				catch
				{
					Console.WriteLine("connection error. retry count: " + i + 1 + "/" + retryCnt);
				}
				System.Threading.Thread.Sleep(1000);
			}
			return serverConnection;
		}		


		// コネクション先の情報を取得
		private string createConnection(int num)
		{
			string toInf = null;
			int befNum = num - 1;
			if (befNum >= 0)
			{
				toInf = this.serverList[befNum].getIp() + ":" + this.serverList[befNum].getPort();
			}
			return toInf;
		}

		// マネージャサーバ間のコネクションチェック
		private void checkConnectionThread(object e)
		{
			lock (((ICollection)this.serverList).SyncRoot)
			{
				for (int i = 1; i < this.serverList.Count; i++)
				{

					if (this.serverList[i].getStatus())
					{
						// マネージャとサーバの接続がある
						DataObject data = new DataObject(MessageTypeEnum.GetConnectStatus);
						this.serverList[i].setSndMessage(data);
					}
					else
					{
						string svStatKey = this.serverList[i].getIp() + ":" + this.serverList[i].getPort();

						if (this.serverStatus[svStatKey].status)
						{
							// コネクションのリトライ
							this.serverList[i] = connectToServer(this.serverList[i].getIp(), this.serverList[i].getPort());
							if (this.serverList[i].getStatus())
							{
								// リトライ後も接続できなかった場合
								// 接続がない場合サーバダウンと判定。後続ノードに接続メッセージの送信
								if (i + 1 < this.serverList.Count)
								{
									string connectionInfo = createConnection(i);
									DataObject data = new DataObject(MessageTypeEnum.ConnectInf, connectionInfo);
									this.serverList[i + 1].setSndMessage(data);
								}
								this.serverList.Remove(this.serverList[i]);
							}
						}
						else {
							this.serverList.Remove(this.serverList[i]);
						}
					}
				}
			}
			System.Threading.Thread.Sleep(100);
			System.Threading.ThreadPool.QueueUserWorkItem(checkConnectionThread);
		}


		// サーバサーバ間のコネクションチェック
		private void dataRecievThread(object e)
		{
			lock (((ICollection)this.serverList).SyncRoot)
			{
				for (int i = 1; i < this.serverList.Count; i++)
				{
					string svStatKey = this.serverList[i].getIp() + ":" + this.serverList[i].getPort();

					DataObject rcData = this.serverList[i].getRcvMessage();
					if (rcData.messageType == MessageTypeEnum.OKConnected)
					{
						this.serverStatus[svStatKey].status = true;

					}
					else if (rcData.messageType == MessageTypeEnum.ConnectFail)
					{
						this.serverStatus[svStatKey].status = false;
					}

					// 最終ノードから伝送完了ファイル一覧を取得
					if (rcData.messageType == MessageTypeEnum.ReturnFinishDataList)
					{
						string[] keyList = rcData.dataStr.Split(',');
						List<int> tmp = new List<int>();
						foreach (string key in keyList)
						{
							tmp.Add(Int32.Parse(key));
						}
						lock (((ICollection)this.serverList).SyncRoot)
						{
							this.finishKetList = tmp;
						}
						getFinalTransferInfoRequest();

					}
				}
			}
			System.Threading.Thread.Sleep(1);
			System.Threading.ThreadPool.QueueUserWorkItem(dataRecievThread);
		}

		// 最終コネクションに対してデータのリストの取得メッセージをそうしん
		private void getFinalTransferInfoRequest()
		{
			lock (((ICollection)this.serverList).SyncRoot)
			{
				int finNo = this.serverList.Count - 1;
				DataObject data = new DataObject(MessageTypeEnum.GetFinishDataList);
				this.serverList[finNo].setSndMessage(data);
			}
		}



		//---- 上位ノード用メソッド ----
		public string getHeadNodeInfo()
		{
			string toInf = null;
			if (this.serverList.Count > 0)
			{
				toInf = this.serverList[0].getIp() + ":" + this.serverList[0].getPort();
			}
			return toInf;
		}

		public void sendHeadNodeMessage(DataObject data)
		{
			lock (((ICollection)this.serverList).SyncRoot)
			{
				this.serverList[0].setSndMessage(data);
			}
		}

		public bool getHeadNodeStatus()
		{
			bool status = false;
			lock (((ICollection)this.serverList).SyncRoot)
			{
				status = this.serverList[0].getStatus();
			}
			return status;
		}

		public DataObject getHeadNodeMessage()
		{
			DataObject data = null;
			lock (((ICollection)this.serverList).SyncRoot)
			{
				data = this.serverList[0].getRcvMessage();
			}
			return data;
		}

		public List<int> getFinishKeyList()
		{
			List<int> tmp = new List<int>();
			lock (((ICollection)this.serverList).SyncRoot)
			{
				tmp = this.finishKetList;
			}
			return tmp;
		}

		public int getConnectionCount()
		{
			int cnt = 0;
			lock (((ICollection)this.serverList).SyncRoot)
			{
				cnt = this.serverList.Count;
			}
			return cnt;

		}
		/*
		public void setHeadNodeConnection(string toInf)
		{
			DataObject data = new DataObject(MessageTypeEnum.ConnectInf, toInf);
			this.serverList[0].setSndMessage(data);
		}
		*/

	}
}
