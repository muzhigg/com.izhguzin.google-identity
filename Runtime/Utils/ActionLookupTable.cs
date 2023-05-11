using System;
using System.Collections.Generic;
using System.Threading;

namespace Izhguzin.GoogleIdentity.Utils
{
    public class ActionLookupTable<T>
    {
        #region Fileds and Properties

        private long _lastTimeStamp = -1;

        private readonly Dictionary<long, Action<T>> _table = new();

        #endregion

        ///<returns> A guaranteed unique long for the given Action. Use this to Get or Pull the Action.</returns>
        public long Put(Action<T> action)
        {
            long id = GetUniqueTick();
            lock (_table)
            {
                _table.Add(id, action);
            }

            return id;
        }

        public Action<T> Get(long id)
        {
            lock (_table)
            {
                if (_table.TryGetValue(id, out Action<T> action)) return action;
            }

            return null;
        }

        ///<summary>Removes and Returns the Action for the given id.</summary>
        ///<returns> Action or null if invalid key</returns>
        public Action<T> Pull(long id)
        {
            lock (_table)
            {
                if (_table.TryGetValue(id, out Action<T> action))
                {
                    _table.Remove(id);
                    return action;
                }
            }

            return null;
        }

        //Adapted from https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.compareexchange?view=netframework-4.8
        private long GetUniqueTick()
        {
            long initialValue, newValue;
            do
            {
                initialValue = _lastTimeStamp;
                long now = DateTime.UtcNow.Ticks;
                newValue = Math.Max(now, initialValue + 1);
                // CompareExchange compares lastTimeStamp to initialValue. If
                // they are not equal, then another thread has updated the
                // running total since this loop started. CompareExchange
                // does not update lastTimeStamp. CompareExchange returns the
                // contents of lastTimeStamp, which do not equal initialValue,
                // so the loop executes again.
            } while (Interlocked.CompareExchange(ref _lastTimeStamp, newValue, initialValue) != initialValue);
            // If no other thread updated the running total, then
            // lastTimeStamp and initialValue are equal when CompareExchange
            // compares them, and newValue is stored in lastTimeStamp.
            // CompareExchange returns the value that was in lastTimeStamp
            // before the update, which is equal to initialValue, so the
            // loop ends.

            // The function returns newValue, not lastTimeStamp, because
            // lastTimeStamp could be changed by another thread between
            // the time the loop ends and the function returns.
            return newValue;
        }
    }
}