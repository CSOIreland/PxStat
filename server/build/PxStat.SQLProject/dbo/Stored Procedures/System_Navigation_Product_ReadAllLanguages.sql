CREATE   PROCEDURE System_Navigation_Product_ReadAllLanguages
@PrcCode NVARCHAR (32)=NULL, @PrcId INT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @PrcId IS NULL
        BEGIN
            SET @PrcId = (SELECT PRC_ID
                          FROM   TD_PRODUCT
                          WHERE  PRC_CODE = @PrcCode
                                 AND PRC_DELETE_FLAG = 0);
        END
    SELECT PRC_ID AS PrcId,
           PRC_CODE AS PrCode,
           PRC_VALUE AS PrcValue,
           LNG_ISO_CODE AS LngIsoCode
    FROM   TD_PRODUCT
           INNER JOIN
           TS_LANGUAGE
           ON LNG_ID = PRC_LNG_ID
              AND LNG_DELETE_FLAG = 0
              AND PRC_DELETE_FLAG = 0
    WHERE  PRC_ID = @PrcId
           OR (@PrcId IS NULL
               AND @PrcCode IS NULL)
    UNION
    SELECT PLG_PRC_ID AS PrcId,
           PRC_CODE AS PrcCode,
           PLG_VALUE AS PrcValue,
           LNG_ISO_CODE AS LngIsoCode
    FROM   TD_PRODUCT
           LEFT OUTER JOIN
           TD_PRODUCT_LANGUAGE
           ON PRC_ID = PLG_PRC_ID
              AND PRC_DELETE_FLAG = 0
           INNER JOIN
           TS_LANGUAGE
           ON PLG_LNG_ID = LNG_ID
              AND LNG_DELETE_FLAG = 0
    WHERE  PLG_PRC_ID = @PrcId
           OR (@PrcId IS NULL
               AND @PrcCode IS NULL);
END

