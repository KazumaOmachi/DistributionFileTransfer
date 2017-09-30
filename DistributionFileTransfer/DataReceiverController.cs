using System;
using DistributionFileTrasfer;

namespace DistributionFileTransfer
{
	public class DataReceiverController
	{
		private NetWorkContoroller dataReciever;
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
			this.dataReciever = null;
			this.dataCache = dataCache;
			this.fileExport = fileExport;
			this.dataSender = dataSender;
			this.isAct = true;
			this.dataReceiveThread = new System.Threading.Thread(dataReceiveThreadAction);
		}

		// データ受信用スレッド
		private void dataReceiveThreadAction(object e)
		{
			while (this.isAct && this.dataReciever != null)
			{
				
				System.Threading.Thread.Sleep(10);
			}
		}

		private void dataReceiveAction(DistributionFileTrasfer.DataObject data)
		{
			// データのキャッシュ
			this.dataCache.setDataCache(data);
			this.fileExport.setFileData(data);
			this.dataSender.setDataObject(data);

		}

		// データ受信インスタンスのセット
		public void setDataReciever(NetWorkContoroller dataReciever)
		{
			while (this.dataReceiveThread.IsAlive)
			{
				this.Dispose();
			}
			lock(this)
			{
				this.dataReciever = dataReciever;
			}
			this.dataReceiveThread.Start();

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
