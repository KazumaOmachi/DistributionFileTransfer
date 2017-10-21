using System;
using DistributionFileTrasfer;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;

namespace DistributionFileTransfer
{
	public class FileExportController
	{
		// private ConcurrentQueue<DataObject> dataQueue;

		private ConcurrentDictionary<int, FileStream> fileDictionary;
		private ConcurrentDictionary<int, List<byte>> fileByteCache;
		private ConcurrentDictionary<int, int> filePoint;

		public FileExportController()
		{
			this.fileDictionary = new ConcurrentDictionary<int, FileStream>();
			this.filePoint = new ConcurrentDictionary<int, int>();

			this.fileByteCache = new ConcurrentDictionary<int, List<byte>>();
			//this.dataQueue = new ConcurrentQueue<DataObject>();
			//ystem.Threading.ThreadPool.QueueUserWorkItem(fileExportThread);
		}

		public void setFileData2(DataObject data)
		{
			if (data.messageType == MessageTypeEnum.FileFinish)
			{
				//removeFile(data.key);

				// -- Test --
				Console.WriteLine("FileManager :Finish message recieve");
				int myPid = System.Diagnostics.Process.GetCurrentProcess().Id;
				string basePath = @"/tmp/" + myPid;
				if (!System.IO.Directory.Exists(basePath))
				{
					Console.WriteLine("create process dictionary");
					System.IO.Directory.CreateDirectory(basePath);
				}
				string filePath = basePath + "/" + data.key + ".txt";
				FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				byte[] fileDataByte = this.fileByteCache[data.key].ToArray();
				Console.WriteLine("write file size : " + fileDataByte.Length);
				foreach (byte d in fileDataByte)
				{
					Console.Write(d + " ");
				}
				Console.WriteLine();
				fs.Write(fileDataByte, 0, fileDataByte.Length);
				fs.Close();
			}
			else if (data.messageType == MessageTypeEnum.FileData)
			{
				lock (this)
				{
					if (!this.fileByteCache.ContainsKey(data.key))
					{
						List<byte> tmp = new List<byte>();
						this.fileByteCache.TryAdd(data.key, tmp);
					}
					this.fileByteCache[data.key].AddRange(data.dataByte);
				}
			}
		}

		public void setFileData(DataObject data)
		{
			if (data.messageType == MessageTypeEnum.FileData)
			{
				// ファイルの保存先
				int myPid = System.Diagnostics.Process.GetCurrentProcess().Id;
				string basePath = @"/tmp/" + myPid;
				if (!System.IO.Directory.Exists(basePath))
				{
					Console.WriteLine("create process dictionary");
					System.IO.Directory.CreateDirectory(basePath);
				}

				if (!this.fileDictionary.ContainsKey(data.key))
				{
					string filePath = basePath + "/" + data.key + ".txt";
					FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
					fs.Write(data.dataByte, 0, data.dataByte.Length);
					fs.Close();
					// this.fileDictionary.TryAdd(data.key, fs);

				}
			}
		}


					/*
					if (!this.filePoint.ContainsKey(data.key))
					{
						this.filePoint.TryAdd(data.key, 0);
					}
					int point = this.filePoint[data.key];
					Console.WriteLine("File Transfer size : " + data.dataByte.Length);
					this.fileDictionary[data.key].Write(data.dataByte, 0, data.dataByte.Length);
					this.filePoint[data.key] += data.dataByte.Length;

				}
			}

		}
		*/

		public void removeFile(int key)
		{
			Console.WriteLine("finish file create : " + key);
			lock(this)
			{
				if (this.fileDictionary.ContainsKey(key))
				{
					this.fileDictionary[key].Close();
					FileStream fs;
					this.fileDictionary.TryRemove(key, out fs);
				}
				if (this.filePoint.ContainsKey(key))
				{
					int rm = 0;
					this.filePoint.TryRemove(key, out rm);
				}
			}
		}



		// ファイルのエクスポートの実施（シングルに一度Queuing）
		/*
		private void fileExportThread(object e)
		{
			DataObject data;
			if (this.dataQueue.TryDequeue(out data))
			{
				// ファイルの作成処理
			}
			System.Threading.ThreadPool.QueueUserWorkItem(fileExportThread);

		}
		*/


	}
}
