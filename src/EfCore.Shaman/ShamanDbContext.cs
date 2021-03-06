﻿#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman
{
    public class ShamanDbContext : DbContext
    {
        #region Constructors

        public ShamanDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected ShamanDbContext()
        {
        }

        #endregion

        #region Instance Methods

        public IDirectSaver<T> GetDirectSaver<T>(Func<ShamanOptions> optionsFactory = null)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                object value;
                if (_directSaverCache.TryGetValue(typeof(T), out value))
                    return (IDirectSaver<T>)value;
                _lock.EnterWriteLock();
                try
                {
                    var options = ShamanOptions.CreateShamanOptions(GetType());
                    var o = options?.Services.OfType<IDirectSaverFactory>().FirstOrDefault();
                    if (o == null)
                        throw new Exception("Unable to find IDirectSaverFactory in ShamanOptions.Services.");
                    var result = o.GetDirectSaver<T>(GetType(), optionsFactory);
                    _directSaverCache[typeof(T)] = result;
                    return result;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }


        #endregion

        #region Fields

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IDictionary<Type, object> _directSaverCache = new Dictionary<Type, object>();

        #endregion
    }
}