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
		private ConcurrentDictionary<string, List<DataObject>> dataCache;

		// コンストラクタ
		public DataCacheController()
		{
			this.dataCache = new ConcurrentDictionary<string, List<DataObject>>();
		}

		// キャッシュにセット
		public void setDataCache(DataObject data)
		{

		}

		// ID指定でデータを取得する
		public List<DataObject> getSingleDataList(string dataId) 
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
			lock (((ICollection)dataCache).SyncRoot)
			{
				foreach (string dataId in dataCache.Keys)
				{
					returnDataList.AddRange(dataCache[dataId]);
				}
			}
			return returnDataList;
		}

		// キャッシュデータのリセット
		public void resetAllCacheData()
		{
			ConcurrentDictionary<string, List<DataObject>> tmp= new ConcurrentDictionary<string, List<DataObject>>();
			this.dataCache = tmp;
		}

	}
}
