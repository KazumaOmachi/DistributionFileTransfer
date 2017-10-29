using System;
using System.Collections.Generic;
using DistributionFileTrasfer;
using System.Collections;

namespace ManagemetFileTransfer
{
	public class ServerManagementController
	{

		List<ServerNodeManager> severConnetList;
		int clientPort = 6901;

		public ServerManagementController()
		{
			this.severConnetList = new List<ServerNodeManager>();




		}

		private void checkNodeConnection(object e)
		{
			lock (((ICollection)this.severConnetList).SyncRoot)
			{
				for (int i = this.severConnetList.Count - 1; i >= 0; i--)
				{
					if (this.severConnetList[i].getConnectionCount() > 0)
					{
						// 最終コネクションでない場合はスキップ
						if (i == 0)
						{
							this.severConnetList[i].setHeadNodeConnection("client:"+clientPort);
						}
						else
						{

							string toInf = this.severConnetList[i - 1].getHeadNodeInfo();
							this.severConnetList[i].setHeadNodeConnection(toInf);
						}

					}
					else {
						this.severConnetList.Remove(this.severConnetList[i]);
					}

				}
			}
		}





	}
}
