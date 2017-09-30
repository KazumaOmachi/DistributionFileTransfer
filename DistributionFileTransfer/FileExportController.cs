using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;

namespace DistributionFileTransfer
{
	public class FileExportController
	{
		private ConcurrentQueue<DataObject> dataQueue;

		public FileExportController()
		{
			this.dataQueue = new ConcurrentQueue<DataObject>();
			System.Threading.ThreadPool.QueueUserWorkItem(fileExportThread);
		}

		public void setFileData(DataObject data)
		{
			this.dataQueue.Enqueue(data);
		}

		// ファイルのエクスポートの実施（シングルに一度Queuing）
		private void fileExportThread(object e)
		{
			DataObject data;
			if (this.dataQueue.TryDequeue(out data))
			{
				// ファイルの作成処理
			}
			System.Threading.ThreadPool.QueueUserWorkItem(fileExportThread);

		}



	}
}
