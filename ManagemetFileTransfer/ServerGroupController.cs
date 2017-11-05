using System;
using System.Collections.Generic;
using DistributionFileTrasfer;
using System.Collections.Concurrent;

namespace ManagemetFileTransfer
{
	public class ServerGroupController
	{
		private ConcurrentDictionary<int, List<int>> finishFaileChache;
		List<ServerNodeManager> severConnetList;

		public ServerGroupController()
		{
			this.finishFaileChache = new ConcurrentDictionary<int, List<int>>();
			this.severConnetList = new List<ServerNodeManager>();

		}
		public void connectManagerThread(object e)
		{

		}
	}
}
