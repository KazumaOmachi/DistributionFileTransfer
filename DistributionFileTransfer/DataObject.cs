using System;
using System.Collections.Generic;

namespace DistributionFileTrasfer
{
	public enum MessageTypeEnum
	{
		HB = 0,
		ConnectList = 1
		//Data = 1,
		//Controll = 2
	}

	// message when type is  1
	/*
	public enum DataMessageEnum
	{
		dataBody = 1,
		finish = 2,
		error = 3,
	}
	*/


	public class DataObject
	{

		private string n;
		private MessageTypeEnum messageType;

		// for data
		private int key;
		private byte[] dataByte;
		//private string messageID;
		//private string dataMessageType;

		// for contorolle
		//private string connectIP;

		public DataObject() { }

		public void setData(string n)
		{
			this.n = n;

		}

		public byte[] getSendData()
		{
			List<byte> sendByte = new List<byte>();
			byte[] portListByte = System.Text.Encoding.ASCII.GetBytes(this.n);
			int size = portListByte.Length;
			byte[] sizeByte = BitConverter.GetBytes(size);

			sendByte.AddRange(BitConverter.GetBytes(sizeByte.Length + portListByte.Length));
			sendByte.AddRange(sizeByte);
			sendByte.AddRange(portListByte);

			return sendByte.ToArray();
		}


		public void setData(MessageTypeEnum messageType , byte[] dataByte)
		{
			
		}
		public void setConnectionListData(string portList)
		{
			this.messageType = MessageTypeEnum.ConnectList;
			this.dataByte = System.Text.Encoding.ASCII.GetBytes(portList);
		}

		public DataObject byteToDataObjec(byte[] dataByte)
		{
			DataObject data = new DataObject();


			return data;	
		}



		/*
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
		*/
	}
}
