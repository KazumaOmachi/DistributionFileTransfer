using System;
using System.Collections.Generic;

namespace DistributionFileTrasfer
{
	public enum MessageTypeEnum
	{
		HB = 0,
		ConnectList = 10,
		FileData = 11,
		FileFinish = 12,

		ConnectInf = 20,
		GetConnectStatus = 21,
		OKConnected = 22,
		ConnectFail = 23,

		GetFinishDataList = 31,
		ReturnFinishDataList = 32,

		DeleteFileData = 41,
		DeleteMassageSend = 42,
		DeleteMassageMissed = 43,

	}



	public class DataObject
	{
		public MessageTypeEnum messageType;

		// for data
		public int key = 0;
		public int seqNo = 0;
		public string dataStr;
		public int dataInt;
		public byte[] dataByte;

		public DataObject(MessageTypeEnum msgTyp)
		{
			this.messageType = msgTyp;

		}

		public DataObject(MessageTypeEnum msgTyp, string str) {
			this.messageType = msgTyp;
			if (msgTyp == MessageTypeEnum.ConnectList || msgTyp == MessageTypeEnum.ConnectInf)
			{
				// クライアント　サーバのポートリスト
				this.dataStr = str;
			}
		}
		public DataObject(MessageTypeEnum msgTyp, int key)
		{
			this.messageType = msgTyp;
			if (msgTyp == MessageTypeEnum.FileFinish)
			{
				this.key = key;
			}
		}



		public DataObject(MessageTypeEnum msgTyp, int key, int seqNo ,byte[] data)
		{
			this.messageType = msgTyp;
			this.key = key;
			this.seqNo = seqNo;
			if (msgTyp == MessageTypeEnum.FileData)
			{
				// クライアント to サーバのファイルデータ
				this.dataByte = data;
			}
		}

		// コンストラクタ
		public DataObject(byte[] data)
		{
			byte[] msgTypByte = new byte[4];
			Array.Copy(data, 0, msgTypByte, 0, msgTypByte.Length);
			int msgTypInt = BitConverter.ToInt32(msgTypByte, 0);
			this.messageType = (MessageTypeEnum)Enum.ToObject(typeof(MessageTypeEnum), msgTypInt);

			Console.WriteLine(this.messageType);
			if (this.messageType == MessageTypeEnum.ConnectList|| this.messageType == MessageTypeEnum.ConnectInf)
			{
				byte[] strByte = new byte[data.Length - msgTypByte.Length];
				Array.Copy(data, 4, strByte, 0, strByte.Length);
				this.dataStr = System.Text.Encoding.ASCII.GetString(strByte);
				Console.WriteLine("---> "+this.dataStr);
			}
			if (this.messageType == MessageTypeEnum.FileData)
			{
				byte[] keyByte = new byte[4];
				Array.Copy(data, 4, keyByte, 0, keyByte.Length);
				this.key = BitConverter.ToInt32(keyByte, 0);

				byte[] seqNoByte = new byte[4];
				Array.Copy(data, 8, seqNoByte, 0, seqNoByte.Length);
				this.seqNo = BitConverter.ToInt32(seqNoByte, 0);

				this.dataByte = new byte[data.Length - 12];
				Array.Copy(data, 12, this.dataByte, 0, this.dataByte.Length);

			}
			if (this.messageType == MessageTypeEnum.FileFinish)
			{
				byte[] keyByte = new byte[4];
				Array.Copy(data, 4, keyByte, 0, keyByte.Length);
				this.key = BitConverter.ToInt32(keyByte, 0);

			}

		}

		// 送信データに変換(object --> byte[])
		public byte[] getSendData()
		{
			List<byte> sendData = new List<byte>();
			sendData.AddRange(BitConverter.GetBytes((int)this.messageType));
			if (this.messageType == MessageTypeEnum.ConnectList|| this.messageType == MessageTypeEnum.ConnectInf)
			{
				sendData.AddRange(System.Text.Encoding.ASCII.GetBytes(this.dataStr));

			}
			if (this.messageType == MessageTypeEnum.FileData)
			{
				sendData.AddRange(BitConverter.GetBytes(this.key));
				sendData.AddRange(BitConverter.GetBytes(this.seqNo));
				sendData.AddRange(this.dataByte);
			}
			if (this.messageType == MessageTypeEnum.FileFinish)
			{
				sendData.AddRange(BitConverter.GetBytes(this.key));
			}
			return sendData.ToArray();
		}

		/*
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
		*/



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
