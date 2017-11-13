using System;
using DistributionFileTrasfer;

using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Collections;

namespace DistributionFileTransfer
{
	public class ManagerComunicationController
	{

		private System.Threading.Thread dataReceiveThread;
		private System.Threading.Thread dataManagementThread;

		private AComunicationManager comManag;
		private	FileExportController fileExport;
		private DataCacheController dataCache;
		private DataReceiverController dataReciv;
		private DataSenderController dataSend;

		private List<NetWorkContoroller> managentList;
		private TcpListener listener_;


		public ManagerComunicationController(DataReceiverController dataReciv, 
		                                     DataCacheController dataCache,
		                                     FileExportController fileExport,
		                                     DataSenderController dataSend,
		                                     int port
		                                     )

		{
			Console.WriteLine("manager ");
			this.dataReciv = dataReciv;
			this.dataCache = dataCache;
			this.fileExport = fileExport;
			this.dataSend = dataSend;

			this.comManag = new AComunicationManager();

			this.managentList = new List<NetWorkContoroller>();
			// dataReceiveThreadの初期化
			//this.dataReceiveThread = new System.Threading.Thread(this.comManag.dataReceivThreadAction);

			// 
			string ipString = "0.0.0.0";
			IPAddress ipAdd = IPAddress.Parse(ipString);
			//int port = 6001;

			this.listener_ = new TcpListener(ipAdd, port);
			this.listener_.Start();

			// management受け入れのスレッドの起動
			System.Threading.ThreadPool.QueueUserWorkItem(acceptManagemet);
			System.Threading.ThreadPool.QueueUserWorkItem(dataManagementThreadAction);

		}


		// マネージャプロセウスの接続
		private void acceptManagemet(object e)
		{
			TcpClient client = this.listener_.AcceptTcpClient();
			Console.WriteLine("manager process connected");
			NetWorkContoroller managent = new NetWorkContoroller(client);
			System.Threading.ThreadPool.QueueUserWorkItem(acceptManagemet);

			lock (((ICollection)this.managentList).SyncRoot)
			{
				this.managentList.Add(managent);
			}

		}

		// 
		private void dataManagementThreadAction(object e)
		{
			lock (((ICollection)this.managentList).SyncRoot)
			{
				foreach (NetWorkContoroller manager in this.managentList)
				{
					if (manager.getStatus())
					{
						DataObject data = manager.getRcvMessage();

						// 変装用メッセージ
						if (data != null)
						{
							DataObject rcData = new DataObject(MessageTypeEnum.HB);
							// 分岐
							// 接続情報の受信（サーバへのコネクト確立・クライアント解放）
							if (data.messageType == MessageTypeEnum.ConnectInf)
							{
								string[] dataStrList = data.dataStr.Split(':');
								string ip = dataStrList[0];
								int port = Int32.Parse(dataStrList[1]);
								setDataReceiveThreadStarter(ip, port);
								Console.WriteLine("connection infomation received : "+ip + ":" + port);
								if (this.comManag.getStatus())
								{
									rcData = new DataObject(MessageTypeEnum.OKConnected);
								}
								else {
									rcData = new DataObject(MessageTypeEnum.ConnectFail);
								}
							}

							// コネクション状況の返答
							else if (data.messageType == MessageTypeEnum.GetConnectStatus)
							{
								if (this.comManag.getStatus())
								{
									rcData = new DataObject(MessageTypeEnum.OKConnected);
								}
								else {
									rcData = new DataObject(MessageTypeEnum.ConnectFail);
								}
							}
							// 終了いるファイルリスト
							else if (data.messageType == MessageTypeEnum.GetFinishDataList)
							{
								rcData = new DataObject(MessageTypeEnum.ReturnFinishDataList, this.fileExport.getFinishKeyList());

							}

							// クライアント切断
							else if (data.messageType == MessageTypeEnum.DeleteFileData)
							{
								if (this.comManag.GetType() == typeof(ClientComunicationManager))
								{
									ClientComunicationManager tmpClientCon = this.comManag as ClientComunicationManager;
									tmpClientCon.removeClient(data.dataInt);
									this.dataReciv.setSendData(data);
									rcData = new DataObject(MessageTypeEnum.DeleteMassageSend);
								}
								else {
									rcData = new DataObject(MessageTypeEnum.DeleteMassageMissed);
								}
							}


							manager.setSndMessage(rcData);
						}
						//
					}
					else
					{
						this.managentList.Remove(manager);
					}
				}
			}
			System.Threading.Thread.Sleep(10);
			System.Threading.ThreadPool.QueueUserWorkItem(dataManagementThreadAction);

		}

		public void setDataReceiveThreadStarter(string ip , int port )
		{
			while (this.dataReceiveThread.IsAlive)
			{
				this.comManag.tofalse();
				System.Threading.Thread.Sleep(10);
			}

			if (ip == "client")
			{
				startClietCommunication(port);

			}
			else
			{
				// サーバプロセス用
				startServerComumnication(ip, port);
			}

			this.dataReceiveThread = new System.Threading.Thread(this.comManag.dataReceivThreadAction);
			this.dataCache.resetAllCacheData();

			this.dataReceiveThread.Start();

		}

		// サーバプロセスの初期化
		public void startServerComumnication(string ip,int port)
		{
			this.comManag = new ServerComunicationManager(this.dataReciv, ip, port);
		}

		// クライアントプロセスの初期化
		public void startClietCommunication(int port)
		{
			// クライアントプロセス用
			this.comManag = new ClientComunicationManager(this.dataReciv, port);
		}
	}
}
