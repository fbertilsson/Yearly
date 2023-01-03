using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YearlyBackend.DataLayer
{
    // TODO Delete
    public static class StorageQueryHelper 
    {
        /// <summary>
        /// Execute an Azure Storage query repeatedly until all data is fetched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        //public static async Task<IList<T>> ExecuteQueryAsync<T>(
        //    this CloudTable table, 
        //    TableQuery<T> query, 
        //    CancellationToken ct = default(CancellationToken), 
        //    Action<IList<T>> onProgress = null) 
        //    where T : ITableEntity, new()
        //{
        //    var items = new List<T>();
        //    TableContinuationToken token = null;

        //    do
        //    {
        //        var seg = await table.ExecuteQuerySegmentedAsync(query, token);
        //        token = seg.ContinuationToken;
        //        items.AddRange(seg);
        //        onProgress?.Invoke(items);

        //    } while (token != null && !ct.IsCancellationRequested);

        //    return items;
        //}
    }
}