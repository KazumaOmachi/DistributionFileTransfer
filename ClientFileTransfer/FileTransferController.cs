using System;

using DistributionFileTrasfer;
using System.Collections;
using System.Collections.Generic;

namespace ClientFileTransfer
{
	public class FileTransferController
	{


		public FileTransferController() { }

		public void run(string filePath)
		{
			int portCnt = 3;
			int fileSplitCnt = 5;

			if (System.IO.File.Exists(filePath))
			{
				// ファイルをローカルにコピー
				string fileName = System.IO.Path.GetFileName(filePath);
				Console.WriteLine(fileName);
				int id = System.Diagnostics.Process.GetCurrentProcess().Id;
				string tmpPath = @"/tmp/test/" + id + "/";
				System.IO.Directory.CreateDirectory(tmpPath);
				System.IO.File.Copy(filePath, tmpPath + fileName);

				// 圧縮

				//ファイルのバイト変換
				System.IO.FileStream fs = new System.IO.FileStream(
					tmpPath + fileName,
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read);
				byte[] bs = new byte[fs.Length];
				fs.Read(bs, 0, bs.Length);
				fs.Close();

				Console.WriteLine("file read : " + bs.Length + "(byte)");
				// 送信
				double doubleSendSize = Convert.ToDouble(bs.Length) / Convert.ToDouble(fileSplitCnt);
				double roundSendSize = Math.Round(doubleSendSize, MidpointRounding.AwayFromZero);
				int sendBaseSize = Convert.ToInt32(roundSendSize);
				Console.WriteLine("Send Base size (byte) : " + sendBaseSize);

				DataObject[] splitFileList = new DataObject[fileSplitCnt];
				int setSize = 0;
				for (int num = 0; num < splitFileList.Length; num++)
				{
					// 送信サイズの決定
					int sendSize = Math.Min(bs.Length - setSize, sendBaseSize);
					Console.WriteLine("size ---> " + sendSize);
					byte[] splitData = new byte[sendSize];
					Array.Copy(bs, setSize, splitData, 0, sendSize);
					setSize += sendSize;
					splitFileList[num] = new DataObject(MessageTypeEnum.FileData, 0, num, splitData);

				}

				// -- Debug --
				/*
				int n = 0;
				foreach (DataObject doe in splitFileList){
					Console.WriteLine("seq no : "+ doe.seqNo +" / "+ doe.dataByte.Length);
					n += doe.dataByte.Length;
				}

				Console.WriteLine("compare size : " + n + "(send) / " + bs.Length + "(origin)");
				*/
				// -----------

				// Console.WriteLine(fs);
				// 分割・データの作成

				// 伝送開始
				string[] connectList = { "127.0.0.1:6001" };
				List<MultiConectionManager> multConList = new List<MultiConectionManager>();
				foreach (string conStr in connectList)
				{
					string[] conInf = conStr.Split(':');
					string ip = conInf[0];
					int port = Int32.Parse( conInf[1]);
					MultiConectionManager tmpMultCon = new MultiConectionManager();
					tmpMultCon.run(portCnt, ip,port, splitFileList);
					multConList.Add(tmpMultCon);
				}

				// 伝送状況確認



				// ファイルの削除
				System.IO.Directory.Delete(tmpPath, true);

			}
			else {
				Console.WriteLine("file not exist");
			}

		}


	}
}
