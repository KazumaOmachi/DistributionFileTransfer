using System;
using System.Collections.Concurrent;
using DistributionFileTrasfer;
using System.Collections.Generic;
using System.Collections;

// データをキャッシュするクラス

namespace DistributionFileTransfer
{
	public class DataCacheController
	{
		private ConcurrentDictionary<int, List<DataObject>> dataCache;

		// コンストラクタ
		public DataCacheController()
		{
			this.dataCache = new ConcurrentDictionary<int, List<DataObject>>();
		}

		// キャッシュにセット
		public void setDataCache(DataObject data)
		{
			Console.WriteLine("Data ccche set data");
			try
			{
				if (!this.dataCache.ContainsKey(data.key))
				{
					Console.WriteLine("create new key");
					List<DataObject> tmpObjList = new List<DataObject>();
					this.dataCache.TryAdd(data.key, tmpObjList);
				}
				if (data.messageType == MessageTypeEnum.FileData)
				{
					this.dataCache[data.key].Add(data);
				}
				if (data.messageType == MessageTypeEnum.FileFinish)
				{
					List<DataObject> rmlistObj;
					this.dataCache.TryRemove(data.key, out rmlistObj);
				}
			}
			catch { }

		}

		// ID指定でデータを取得する
		public List<DataObject> getSingleDataList(int dataId) 
		{
			List<DataObject> returnDataList = new List<DataObject>();
			if (dataCache.ContainsKey(dataId))
			{
				returnDataList = dataCache[dataId];
			}
			return returnDataList;
		}

		// 全てのデータキャッシュを取得する
		public List<DataObject> getAllDataList()
		{
			List<DataObject> returnDataList = new List<DataObject>();
			lock (this)
			{
				foreach (int dataId in dataCache.Keys)
				{
					returnDataList.AddRange(dataCache[dataId]);
				}
			}
			return returnDataList;
		}

		// キャッシュデータのリセット
		public void resetAllCacheData()
		{
			ConcurrentDictionary<int, List<DataObject>> tmp= new ConcurrentDictionary<int, List<DataObject>>();
			this.dataCache = tmp;
		}

		// キャッシュデータのリセット
		public void removeCacheData(int key)
		{
			List<DataObject> rmCache = new List<DataObject>();
			this.dataCache.TryRemove(key, out rmCache);
		}

	}
}
