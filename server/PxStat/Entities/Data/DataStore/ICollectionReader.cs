﻿using API;
using System;

namespace PxStat.DataStore
{
    public interface ICollectionReader
    {
        dynamic ReadCollectionMetadata(IADO ado, string languageCode, DateTime DateFrom, string PrcCode = null, bool meta = true);
    }
}