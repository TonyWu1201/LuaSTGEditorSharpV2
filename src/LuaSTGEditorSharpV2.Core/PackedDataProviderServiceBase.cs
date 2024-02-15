﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaSTGEditorSharpV2.Core.Exception;

namespace LuaSTGEditorSharpV2.Core
{
    public abstract class PackedDataProviderServiceBase<TData> : IPackedDataProviderService<TData>
        where TData : class
    {
        private readonly Dictionary<string, PriorityQueue<TData, PackageInfo>> _registered = [];

        public void Register(string id, PackageInfo packageInfo, TData data)
        {
            try
            {
                if (!_registered.ContainsKey(id))
                {
                    _registered.Add(id, new PriorityQueue<TData, PackageInfo>());
                }
                _registered[id].Enqueue(data, packageInfo);
            }
            catch (ArgumentException e)
            {
                throw new DuplicatedIDException(id, e);
            }
        }

        public void UnloadPackage(PackageInfo packageInfo)
        {
            List<TData> services = [];
            List<PackageInfo> packageInfos = [];
            foreach (var kvp in _registered)
            {
                var pq = kvp.Value;
                services.Clear();
                packageInfos.Clear();
                services.Capacity = services.Capacity < pq.Count ? pq.Count : services.Capacity;
                packageInfos.Capacity = packageInfos.Capacity < pq.Count ? pq.Count : packageInfos.Capacity;
                while (pq.TryDequeue(out TData? s, out PackageInfo? pi))
                {
                    if (pi != packageInfo)
                    {
                        services.Add(s);
                        packageInfos.Add(pi);
                    }
                }
                for (int i = 0; i < services.Count; i++)
                {
                    pq.Enqueue(services[i], packageInfos[i]);
                }
            }
        }

        internal protected TData? GetDataOfID(string id)
        {
            if (_registered.ContainsKey(id) && _registered[id].Count > 0)
            {
                return _registered[id].Peek();
            }
            return null;
        }

        internal protected IReadOnlyDictionary<string, TData> GetRegisteredAvailableData()
        {
            var dict = new Dictionary<string, TData>();
            foreach (var kvp in _registered)
            {
                dict.Add(kvp.Key, kvp.Value.Peek());
            }
            return dict;
        }
    }
}
