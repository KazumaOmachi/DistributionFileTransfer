using System;
namespace DistributionFileTrasfer
{
	public enum MessageTypeEnum
	{
		Data = 1,
		Controll = 2
	}

	// message when type is  1
	public enum DataMessageEnum
	{
		dataBody = 1,
		finish = 2,
		error = 3,
	}



	public class DataObject
	{
		private MessageTypeEnum messageType;
		// for data
		private byte[] fileDataMain;
		private string messageID;
		private string dataMessageType;

		// for contorolle
		private string connectIP;

		public DataObject()
		{

		}

		// file data message from client
		public void setFileData(byte[] data)
		{
			this.messageType = messageType = MessageTypeEnum.Data;
			// date (YYYYMMDDhhmmssnnn)-pid
			this.messageID = "";
			this.fileDataMain = data;
		}

		public void errorFileMessage(string msId)
		{
			this.messageID = msId;
		}

		//
		public void setConnetMEssage(string ip)
		{
			this.messageType = messageType = MessageTypeEnum.Data;
			this.connectIP = ip;
		}

		public byte[] getDataList()
		{
			byte[] data = new byte[5];
			return data;
		}

		public MessageTypeEnum getMessageType()
		{
			return this.messageType;
		}

		public byte[] getFileDataMain()
		{
			return this.fileDataMain;
		}
	}
}
