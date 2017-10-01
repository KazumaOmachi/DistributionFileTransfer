using System;
using DistributionFileTrasfer;

namespace DistributionFileTransfer
{
	public interface ITcpNetWorkManager
	{
		void setSndMessage(DataObject obj);
		DataObject getRcvMessage();

	}
}
