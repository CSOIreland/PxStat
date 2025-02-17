CREATE   PROCEDURE [dbo].[Data_Matrix_Read_LiveAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MTR_CODE AS MtrCode,
           MTR_TITLE AS MtrTitle,
           LNG_ISO_CODE AS LngIsoCode,
           RLS_VERSION AS RlsVersion,
           RLS_REVISION AS RlsRevision,
           RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
    FROM   TD_MATRIX
           INNER JOIN
           VW_RELEASE_LIVE_NOW
           ON MTR_ID = VRN_MTR_ID
           INNER JOIN
           TD_RELEASE
           ON RLS_ID = VRN_RLS_ID
              AND RLS_ID = MTR_RLS_ID
           INNER JOIN
           TS_LANGUAGE
           ON LNG_ID = MTR_LNG_ID
              AND LNG_DELETE_FLAG = 0;
END

