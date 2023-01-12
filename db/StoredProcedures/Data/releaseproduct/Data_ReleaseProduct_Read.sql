/****** Object:  StoredProcedure [dbo].[Data_ReleaseProduct_Read]    Script Date: 10/08/2022 08:33:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[Data_ReleaseProduct_Read]
@ReleaseCode INT, @LngIsoCode CHAR (2)=NULL
AS
BEGIN
    SET NOCOUNT ON;

	SELECT   PRC_CODE AS PrcCode,
             PRC_VALUE AS PrcValue,
             SBJ_CODE AS SbjCode,
             SBJ_VALUE AS SbjValue
    FROM     (SELECT TD_PRODUCT.PRC_ID, TD_PRODUCT.PRC_CODE,
                     SBJ_CODE,
                     COALESCE (PLG_Value, PRC_VALUE) AS PRC_VALUE,
                     COALESCE (SLG_VALUE, SBJ_VALUE) AS SBJ_VALUE,
                     PRC_DELETE_FLAG,
                     RLS_DELETE_FLAG
              FROM   TD_PRODUCT
                     INNER JOIN
                     [TD_SUBJECT]
                     ON SBJ_ID = PRC_SBJ_ID
                        AND SBJ_DELETE_FLAG = 0
                     LEFT OUTER JOIN
                     (SELECT   PRC_CODE,
                               COUNT(*) AS mtrCount
                      FROM     (SELECT DISTINCT PRC_CODE,
                                                MTR_CODE
                                FROM   TD_PRODUCT
                                       INNER JOIN
                                       TD_RELEASE
                                       ON PRC_ID = RLS_PRC_ID
                                          AND PRC_DELETE_FLAG = 0
                                          AND RLS_DELETE_FLAG = 0
                                       INNER JOIN
                                       TD_MATRIX
                                       ON MTR_RLS_ID = RLS_ID
                                          AND MTR_DELETE_FLAG = 0) AS prcInner
                     GROUP BY PRC_CODE) AS prcMtrCounter
                     ON TD_PRODUCT.PRC_CODE = prcMtrCounter.PRC_CODE
                     LEFT OUTER JOIN
                     (SELECT SLG_SBJ_ID,
                             SLG_VALUE
                      FROM   TD_SUBJECT_LANGUAGE
                             INNER JOIN
                             TS_LANGUAGE
                             ON SLG_LNG_ID = LNG_ID
                                AND LNG_DELETE_FLAG = 0
                                AND LNG_ISO_CODE = @LngIsoCode) AS INNER_SLG
                     ON SBJ_ID = INNER_SLG.SLG_SBJ_ID
                     LEFT OUTER JOIN
                     (SELECT prg.PLG_PRC_ID,
                             prg.PLG_VALUE
                      FROM   TD_PRODUCT_LANGUAGE AS prg
                             INNER JOIN
                             TS_LANGUAGE
                             ON prg.PLG_LNG_ID = LNG_ID
                                AND LNG_DELETE_FLAG = 0
                                AND LNG_ISO_CODE = @LngIsoCode) AS INNER_PRG
                     ON PRC_ID = INNER_PRG.PLG_PRC_ID
                     LEFT OUTER JOIN
                     TD_RELEASE
                     ON RLS_PRC_ID = PRC_ID
                        AND RLS_DELETE_FLAG = 0
                        AND RLS_id IN (SELECT VRN_RLS_ID
                                       FROM   VW_RELEASE_LIVE_NOW)) AS INNER_SELECT
					LEFT OUTER JOIN
					TD_MATRIX
					ON MTR_RLS_ID = (SELECT RLS_ID FROM TD_RELEASE WHERE RLS_CODE = @ReleaseCode)

    WHERE    PRC_ID IN (SELECT RPR_PRC_ID FROM TM_RELEASE_PRODUCT WHERE RPR_RLS_ID = (SELECT RLS_ID FROM TD_RELEASE WHERE RLS_CODE = @ReleaseCode) 
	AND RPR_DELETE_FLAG = 0)
	group by MTR_CODE, PRC_CODE, PRC_VALUE, SBJ_CODE, SBJ_VALUE 
END

