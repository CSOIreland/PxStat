CREATE   PROCEDURE System_Navigation_Subject_ReadAllLanguages
@SbjCode INT=NULL, @SbjId INT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @SbjId IS NULL
        BEGIN
            SET @SbjId = (SELECT SBJ_ID
                          FROM   TD_SUBJECT
                          WHERE  SBJ_CODE = @SbjCode
                                 AND SBJ_DELETE_FLAG = 0);
        END
    SELECT SBJ_ID AS SbjId,
           SBJ_CODE AS SbjCode,
           SBJ_VALUE AS SbjValue,
           LNG_ISO_CODE AS LngIsoCode
    FROM   TD_SUBJECT
           INNER JOIN
           TS_LANGUAGE
           ON SBJ_LNG_ID = LNG_ID
              AND LNG_DELETE_FLAG = 0
              AND SBJ_DELETE_FLAG = 0
    WHERE  SBJ_ID = @SbjId
           OR (@SbjCode IS NULL
               AND @SbjId IS NULL)
    UNION
    SELECT SLG_SBJ_ID AS SbjId,
           SBJ_CODE AS SbjCode,
           SLG_VALUE AS SbjValue,
           LNG_ISO_CODE AS LngIsoCode
    FROM   TD_SUBJECT
           LEFT OUTER JOIN
           TD_SUBJECT_LANGUAGE
           ON SBJ_ID = SLG_SBJ_ID
              AND SBJ_DELETE_FLAG = 0
           INNER JOIN
           TS_LANGUAGE
           ON SLG_LNG_ID = LNG_ID
              AND LNG_DELETE_FLAG = 0
    WHERE  SLG_SBJ_ID = @SbjId
           OR (@SbjCode IS NULL
               AND @SbjId IS NULL);
END

