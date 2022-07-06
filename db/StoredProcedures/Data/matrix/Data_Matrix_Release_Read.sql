SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 26/01/2022
-- Description:	Read Matrix highlights from release code
-- exec Data_Matrix_Release_Read 1,'en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_Release_Read @RlsCode INT
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SELECT MTR_ID AS MtrId
		,MTR_CODE AS MtrCode
	FROM TD_MATRIX
	INNER JOIN TD_RELEASE ON RLS_DELETE_FLAG = 0
		AND MTR_DELETE_FLAG = 0
		AND MTR_RLS_ID = RLS_ID
		AND RLS_CODE = @RlsCode
	INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
		AND LNG_ISO_CODE = @LngIsoCode
END
GO


