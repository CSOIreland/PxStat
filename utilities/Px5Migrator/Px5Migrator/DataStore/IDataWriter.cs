using API;

namespace Px5Migrator
{

    public interface IDataWriter
    {
        void CreateAndLoadDataField(IADO ado, IDmatrix dMatrix, string username, int releaseId);
        void CreateAndLoadMetadata(IADO ado, IDmatrix dmatrix);
    }

}
