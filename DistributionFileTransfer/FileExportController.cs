using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;

namespace DistributionFileTransfer
{
	public class FileExportController
	{
		private ConcurrentDictionary<int, List<byte>> fileDataCache;
		              
		public FileExportController() {
			this.fileDataCache = new ConcurrentDictionary<int, List<byte>>();
		}

		public void setFileData(DataObject data)
		{
			//this.fileDataQueue.Enqueue(data);
			if (!this.fileDataCache.ContainsKey(data.key))
			{
				Console.WriteLine("Recieve New Key : " + data.key);
				this.fileDataCache.TryAdd(data.key, new List<byte>());
			}
			if (data.messageType == MessageTypeEnum.FileData)
			{
				Console.WriteLine("Add File data byte. key : " + data.key + " / size : " + data.dataByte.Length);
				this.fileDataCache[data.key].AddRange(data.dataByte);
			}
			else if (data.messageType == MessageTypeEnum.FileFinish)
			{
				// 終了
				Console.WriteLine("Finish Created file. key : "+ data.key);
				System.Threading.ThreadPool.QueueUserWorkItem(createFileThread, data.key);
			}
		}

		// ファイル作成
		private void createFileThread(object e)
		{
			int key = (int)e;
			Console.WriteLine("start create file ");

			List<byte> dataList;
			this.fileDataCache.TryRemove(key, out dataList);
			byte[] dataByte = dataList.ToArray();

			// ファイルの保存先
			int myPid = System.Diagnostics.Process.GetCurrentProcess().Id;
			string basePath = @"/tmp/" + myPid;
			if (!System.IO.Directory.Exists(basePath))
			{
				Console.WriteLine("create process dictionary");
				System.IO.Directory.CreateDirectory(basePath);
			}

			string filePath = basePath + "/" + key + ".txt";
			Console.WriteLine("create file key : " + key + " / file path : " + filePath);
			FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
			fs.Write(dataByte, 0, dataByte.Length);
			Console.WriteLine("finish create file ");
			fs.Close();
		}

	}
}
