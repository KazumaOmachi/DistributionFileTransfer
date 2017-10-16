using System;

namespace ClientFileTransfer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			FileTransferController ftc = new FileTransferController();
			ftc.run(@"/tmp/test.txt");

			/*
			if (args.Length == 1)
			{

				// ファイルの確認
				string filePath = args[0];

			}
			else {
				Console.WriteLine("Need input arg1");
			}
			*/

		}
	}
}
