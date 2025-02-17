CREATE   PROCEDURE Data_Geomap_Update
@GmpCode NVARCHAR (32), @GmpName NVARCHAR (256), @GmpDescription NVARCHAR (MAX), @CcnUsername NVARCHAR (256), @GlrCode NVARCHAR (256)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DtgID AS INT;
    DECLARE @GlrId AS INT;
    SET @DtgID = (SELECT GMP_DTG_ID
                  FROM   TD_GEOMAP
                  WHERE  GMP_CODE = @GmpCode);
    IF @DtgID = 0
       OR @DtgID IS NULL
        BEGIN
            RAISERROR ('Unknown GeoMap Code', 16, 1);
            RETURN 0;
        END
    SET @GlrId = (SELECT GLR_ID
                  FROM   TD_GEOLAYER
                  WHERE  GLR_CODE = @GlrCode
                         AND GLR_DELETE_FLAG = 0);
    IF @GlrId = 0
       OR @GlrId IS NULL
        BEGIN
            RAISERROR ('Unknown Geolayer Code', 16, 1);
            RETURN 0;
        END
    EXECUTE @DtgID = Security_Auditing_Update @DtgID, @CcnUsername;
    UPDATE TD_GEOMAP
    SET    GMP_NAME        = @GmpName,
           GMP_DESCRIPTION = @GmpDescription,
           GMP_GLR_ID      = @GlrId
    WHERE  GMP_CODE = @GmpCode;
    RETURN @@ROWCOUNT;
END

