﻿#region using

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class NotMappedAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                columnInfo.IsNotMapped = true;
        }

        #endregion
    }
}