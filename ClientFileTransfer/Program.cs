using System;
using System.Collections.Generic;

namespace ClientFileTransfer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			FileTransferController ftc = new FileTransferController();

			List<string> connectList = new List<string>();
			string line;
			System.IO.StreamReader file = new System.IO.StreamReader(args[0]);
			while ((line = file.ReadLine()) != null)
			{
				System.Console.WriteLine(line);
				string[] tmpList = line.Split(',');
				if (tmpList[1] == "0" && tmpList[2] == "0")
				{
					connectList.Add(tmpList[3] + ":" + tmpList[6]);
				}
			}

			ftc.run(@"/tmp/test.txt",connectList.ToArray());

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
