using System;
using System.Threading.Tasks;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class ClientCached
    {
        static object DisposeLock = new object();
        static DateTime Next;

        static void Dispose()
        {
            Task.Run(() => DisposeQueueInterval());
        }

        static void DisposeQueueInterval()
        {
            if (DateTime.Now < Next)
                return;

            var keys = DisposeQueue.Keys;

            if (keys.IsNot()) return;

            lock (DisposeLock)
            {
                if (DateTime.Now < Next)
                    return;

                Next = DateTime.Now.AddSeconds(30);
            }

            Debug.Log("Check dispose queue for abandoned clients");

            bool HasExpired(CacheModel cachedModel)
            {
                return cachedModel?.CachedClient == null || cachedModel.Expires < DateTime.Now;
            }

            foreach (var key in keys)
            {
                if (DisposeQueue.TryGetValue(key, out CacheModel cached))
                {
                    try
                    {
                        if (HasExpired(cached))
                        {
                            DisposeQueue.TryRemove(key, out CacheModel removed);
                            removed?.Dispose();
                            Debug.Log("Disposed client " + key);
                        }
                    }
                    catch
                    {
                        //Swallow
                    }
                }
            }
        }
    }
}