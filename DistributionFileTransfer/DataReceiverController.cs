using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;

namespace DistributionFileTransfer
{
	public class DataReceiverController
	{
		private DataCacheController dataCache;
		private FileExportController fileExport;
		private DataSenderController dataSender;

		// コンストラクタ
		public DataReceiverController(DataCacheController dataCache,
									  FileExportController fileExport,
									  DataSenderController dataSender)
		{
			this.dataCache = dataCache;
			this.fileExport = fileExport;
			this.dataSender = dataSender;
		}

		// データ受信 --> 送信
		public void setSendData(DataObject data)
		{
			if (data != null)
			{
				lock (this)
				{
					if (data.messageType == MessageTypeEnum.FileData
						|| data.messageType == MessageTypeEnum.FileFinish)
					{
						// データの送信
						this.dataSender.setDataObject(data);

						// キャッシュへ登録
						this.dataCache.setDataCache(data);

						// ファイルの出力
						this.fileExport.setFileData(data);
					}
				}
			}
		}

		// 終了処理
		public void Dispose()
		{
			lock(this)
			{
				//this.isAct = false;
			}
		}

	}
}
