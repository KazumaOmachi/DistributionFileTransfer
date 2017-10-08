using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;

namespace DistributionFileTransfer
{
	public class DataReceiverController
	{
		//private NetWorkContoroller dataReciever;
		private ConcurrentQueue<DataObject> dataList;
		private DataCacheController dataCache;
		private FileExportController fileExport;
		private DataSenderController dataSender;
		private System.Threading.Thread dataReceiveThread;
		private bool isAct;

		// コンストラクタ
		public DataReceiverController(DataCacheController dataCache,
		                              FileExportController fileExport,
		                              DataSenderController dataSender)
		{
			//this.dataReciever = null;
			this.dataList = new ConcurrentQueue<DataObject>();
			this.dataCache = dataCache;
			this.fileExport = fileExport;
			this.dataSender = dataSender;
			this.isAct = true;
			this.dataReceiveThread = new System.Threading.Thread(dataSendThreadAction);
		}

		// データ受信用スレッド

		private void dataSendThreadAction(object e)
		{
			while (this.isAct )//&& this.dataReciever != null)
			{
				DataObject data = null;
				if (this.dataList.TryDequeue(out data))
				{
					// データの送信
					this.dataSender.setDataObject(data);

					// キャッシュへ登録
					this.dataCache.setDataCache(data);

					// ファイルの出力
					this.fileExport.setFileData(data);

				}
				System.Threading.Thread.Sleep(10);
			}
		}

		// データの登録
		public void setSendData(DataObject data)
		{
			this.dataList.Enqueue(data);
		}

		// 終了処理
		public void Dispose()
		{
			lock(this)
			{
				this.isAct = false;
			}
		}

	}
}
