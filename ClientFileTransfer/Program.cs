using System;

namespace ClientFileTransfer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			if (args.Length == 1)
			{

				// ファイルの確認
				string filePath = args[0];
				if (System.IO.File.Exists(filePath)){

					// ファイルをローカルにコピー
					string fileName = System.IO.Path.GetFileName(filePath);
					Console.WriteLine(fileName);
					int id = System.Diagnostics.Process.GetCurrentProcess().Id;
					string tmpPath = @"/tmp/test/"+ id + "/";
					System.IO.Directory.CreateDirectory(tmpPath);
					System.IO.File.Copy(filePath, tmpPath + filePath);

					// 圧縮
					string zipFileName = System.IO.Path.GetFileNameWithoutExtension(filePath)+ ".zip";


					// 分割・データの作成

					// TCPの複数ポート作成

					// サーバへの接続

					// 接続待ち（サーバから複数接続）

					// 終了まち・タイムアウト設定

					// ファイルの削除
					System.IO.Directory.Delete(tmpPath, true);
					Console.WriteLine("file transfer compleat");

				}
			}
			else {
				Console.WriteLine("Need input arg1");
			}


		}
	}
}
