using System;
using System.Collections.Generic;

namespace ManagemetFileTransfer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!!!");
			/*
			if (args.Length != 0)
			{
				string confFile = args[0];

			}
			*/

			List<ServerManagementController> SvrMgConList = new List<ServerManagementController>();
			string line;
			Dictionary<int,List<ConnectionInfo>> tmpDict = new Dictionary<int, List<ConnectionInfo>>();
			System.IO.StreamReader file = new System.IO.StreamReader(args[0]);
			Console.WriteLine(args[0]);
			while ((line = file.ReadLine()) != null)
			{
				System.Console.WriteLine(line);
				string[] tmpList = line.Split(',');
				if (System.Text.RegularExpressions.Regex.IsMatch(tmpList[0], @"\d+"))
				{
					int region = Int32.Parse(tmpList[0]);
					if (!tmpDict.ContainsKey(region))
					{
						List<ConnectionInfo> tmpConObj = new List<ConnectionInfo>();
						tmpDict.Add(region, tmpConObj);
					}


					ConnectionInfo tmp = new ConnectionInfo();
					tmp.region = Int32.Parse(tmpList[0]);
					tmp.group = Int32.Parse(tmpList[1]);
					tmp.number = Int32.Parse(tmpList[2]);
					tmp.ip = tmpList[3];
					tmp.serverPort = Int32.Parse(tmpList[4]);
					tmp.managerPort = Int32.Parse(tmpList[5]);
					tmp.clientPort = Int32.Parse(tmpList[6]);

					tmpDict[region].Add(tmp);


				}
			}
			foreach (int key in tmpDict.Keys)
			{
				Console.WriteLine("region : " + key);
				ServerManagementController SvrMgCon = new ServerManagementController(tmpDict[key]);
			}
			while (true) { }
		}
	}
}
