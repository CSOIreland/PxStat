
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/03/2021
-- Description:	Reads record(s) from the TD_Subject table
--exec System_Navigation_Theme_Read  null,'ga','xxx'
-- =============================================
CREATE
	

 PROCEDURE [dbo].[System_Navigation_Theme_Read] @ThmCode INT = NULL
	,@LngIsoCode CHAR(2)
	,@ThmValue NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIsoId AS INT;

	SET @LngIsoId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			);

	SELECT ThmCode
		,ThmValue
		,sum((
				CASE 
					WHEN SBJ_ID IS NOT NULL
						THEN 1
					ELSE 0
					END
				)) AS SbjCount
	FROM (
		SELECT 
			SBJ_ID
			,COALESCE(INNER_TLG.TLG_VALUE, THM_VALUE) AS ThmValue
			,THM_CODE AS ThmCode
		FROM TD_THEME
		LEFT OUTER JOIN TD_SUBJECT ON SBJ_THM_ID = THM_ID
			AND SBJ_DELETE_FLAG = 0
			AND THM_DELETE_FLAG = 0
	
		LEFT OUTER JOIN (
			SELECT TLG_THM_ID
				,TLG_VALUE
			FROM TD_THEME_LANGUAGE
			INNER JOIN TS_LANGUAGE ON TLG_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
			) AS INNER_TLG ON THM_ID = INNER_TLG.TLG_THM_ID
		WHERE (
				(
					@ThmCode IS NOT NULL
					AND [THM_CODE]  = @ThmCode
					)
				OR @ThmCode IS NULL
				)
			AND (
				(
					@ThmValue IS NOT NULL
					AND [THM_VALUE] = @ThmValue
					)
				OR @ThmValue IS NULL
				)
			AND THM_DELETE_FLAG = 0
		)  INNER_SELECT
	GROUP BY ThmCode
		,ThmValue;
END
