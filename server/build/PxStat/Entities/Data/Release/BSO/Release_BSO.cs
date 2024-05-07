using API;
using System;

namespace PxStat.Data
{
    internal class Release_BSO : IDisposable
    {
        private IADO _ado;
        internal Release_BSO(IADO ado)
        {
            _ado = ado;
        }

        public void Dispose()
        {
        }

        internal bool IsLiveWithWip(int rlsCode, string samAccountName)
        {
            return GetWipForLive(rlsCode, samAccountName) != null;
        }



        internal dynamic GetWipForLive(int rlsCode, string samAccountName)
        {
            Release_ADO rAdo = new Release_ADO(_ado);
            if (!rAdo.IsLiveNow(rlsCode))
            {
                return null;
            }

            dynamic query = rAdo.Read(rlsCode, samAccountName);
            if (query == null)
            {
                return null;
            }
            query = rAdo.ReadLatest(new Release_DTO_Read() { MtrCode = query.MtrCode });
            if (query == null)
            {
                return null;
            }
            if (rAdo.IsWip(query.RlsCode))
            {
                return query;
            }

            return null;
        }

        internal dynamic GetPendingLiveForLive(int rlsCode, string samAccountName)
        {
            Release_ADO rAdo = new Release_ADO(_ado);
            if (!rAdo.IsLiveNow(rlsCode))
            {
                return null;
            }

            dynamic query = rAdo.Read(rlsCode, samAccountName);
            if (query == null)
            {
                return null;
            }
            query = rAdo.ReadPendingLive(new Release_DTO_Read() { MtrCode = query.MtrCode });
            if (query == null)
            {
                return null;
            }
            if (rAdo.IsLiveNext(query.RlsCode))
            {
                return query;
            }

            return null;
        }


    }
}
