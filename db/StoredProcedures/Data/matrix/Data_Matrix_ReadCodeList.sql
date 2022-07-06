SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 22 Oct 2018
-- Description:	Reads record(s) from the TD_MATRIX and TD_Release table
-- exec Data_Matrix_ReadCodeList 'okeeffene','en','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadCodeList @CcnUsername NVARCHAR(256)
	
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);
	

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT DISTINCT MtrCode
		
	FROM (
		SELECT mtr.MTR_CODE MtrCode
			,mtr.MTR_LNG_ID
			,mtrLng.MTR_TITLE MtrTitle
			
		FROM TD_MATRIX mtr
		LEFT JOIN (
			SELECT MTR_CODE
				,MTR_TITLE
				,MTR_RLS_ID
			FROM TD_MATRIX
			WHERE MTR_DELETE_FLAG = 0
				
			) mtrLng
			ON mtr.MTR_CODE = mtrLng.MTR_CODE
				AND MTR.MTR_RLS_ID = mtrLng.MTR_RLS_ID
		INNER JOIN TD_RELEASE
			ON RLS_ID = mtr.MTR_RLS_ID
				AND RLS_DELETE_FLAG = 0
		INNER JOIN @GroupUserHasAccess g
			ON g.GRP_ID = RLS_GRP_ID
		WHERE MTR_DELETE_FLAG = 0
		GROUP BY mtr.MTR_CODE
			,mtr.MTR_LNG_ID
			,mtrLng.MTR_TITLE
		) q
	
	ORDER BY q.MtrCode
END
GO


