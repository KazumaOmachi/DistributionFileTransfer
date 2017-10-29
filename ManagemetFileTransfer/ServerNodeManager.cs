using System;
using System.Collections.Generic;
using DistributionFileTrasfer;
using System.Net.Sockets;
using System.Net;
using System.Collections;

namespace ManagemetFileTransfer
{
	public class ServerNodeManager
	{
		List<NetWorkContoroller> serverList;

		public ServerNodeManager(List<string> connectList)
		{
			this.serverList = new List<NetWorkContoroller>();
			foreach (string conectInf in connectList)
			{
				try
				{
					string[] conInfList = conectInf.Split(':');
					string ip = conInfList[0];
					int port = Int32.Parse(conInfList[1]);
					TcpClient tcp = new TcpClient(ip, port);
					NetWorkContoroller serverConnection = new NetWorkContoroller(tcp);

					this.serverList.Add(serverConnection);

				}
				catch { }
			}

			// サーバ間のコネクション確立
			for (int i = this.serverList.Count - 1; i >= 0; i--)
			{
				//string toInf = createConnection(i);
				//this.serverList[i]
				DataObject data = new DataObject(MessageTypeEnum.ConnectInf, createConnection(i));
				this.serverList[i].setSndMessage(data);

				while (true)
				{
					DataObject rcData = this.serverList[i].getRcvMessage();
					if (rcData != null)
					{
						if (rcData.messageType == MessageTypeEnum.OKConnected)
						{
							Console.WriteLine("connected");
						}
						else if ( rcData.messageType == MessageTypeEnum.ConnectFail){
							this.serverList.Remove(this.serverList[i]);
						}
						break;
					}
					System.Threading.Thread.Sleep(1);
				}
			}

		}

		public string getHeadNodeInfo()
		{
			string toInf = null;
			if (this.serverList.Count > 0)
			{
				toInf = this.serverList[0].getIp() + ":" + this.serverList[0].getPort();
			}
			return toInf;
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

		public bool setHeadNodeConnection(string toInf)
		{
			DataObject data = new DataObject(MessageTypeEnum.ConnectInf, toInf);
			this.serverList[0].setSndMessage(data);
			bool status = true;
			while (true)
			{
				DataObject rcData = this.serverList[0].getRcvMessage();
				if (rcData != null)
				{
					if (rcData.messageType == MessageTypeEnum.OKConnected)
					{
						Console.WriteLine("connected");
						status = true;
					}
					else if (rcData.messageType == MessageTypeEnum.ConnectFail)
					{
						this.serverList.Remove(this.serverList[0]);
						status = false;
					}
					break;
				}
				System.Threading.Thread.Sleep(1);
			}
			return status;
		}


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

		private void checkConnection(object e)
		{
			// A <- B <- C
			// B fail
			// get A info
			// set C A info 
			lock (((ICollection)this.serverList).SyncRoot)
			{
				for (int i = 0; i < this.serverList.Count; i ++)
				{
					if (this.serverList[i].getStatus())
					{
						DataObject data = new DataObject(MessageTypeEnum.GetConnectStatus);
						this.serverList[i].setSndMessage(data);
						while (true)
						{
							DataObject rcData = this.serverList[i].getRcvMessage();
							if (rcData.messageType == MessageTypeEnum.OKConnected)
							{
								break;
							}
							else if (rcData.messageType == MessageTypeEnum.ConnectFail)
							{
								// リトライして失敗したら
								for (int k = i - 1; k >= 0; k--)
								{
									if (this.serverList[k].getStatus())
									{

									}

								}
							}
						}
					}
					else {
						// コネクション自体が死んでいる場合（サーバダウンを想定）
						this.serverList.Remove(this.serverList[i]);


						// 最終コネクション出ない場合
						if (i + i < this.serverList.Count - 1)
						{
							DataObject data = new DataObject(MessageTypeEnum.ConnectInf, createConnection(i));

							this.serverList[i + 1].setSndMessage(data);

						}
					}
				}
			}
		}

		private void getFinalTransferInfo(object e)
		{
			lock (((ICollection)this.serverList).SyncRoot)
			{
				int finNo = this.serverList.Count - 1;
				DataObject data = new DataObject(MessageTypeEnum.GetFinishDataList);
				this.serverList[finNo].setSndMessage(data);

			}
		}

	}
}
