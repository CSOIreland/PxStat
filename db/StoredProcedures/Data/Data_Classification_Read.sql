SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 19/03/2019
-- Description:	Reads an individual Classification based on ClassificationID
-- exec Data_Classification_Read 58
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Classification_Read @ClsID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CLS_CODE AS ClsCode
		,CLS_VALUE AS ClsValue
		,CLS_GEO_URL AS ClsGeoUrl
		,VRB_CODE AS VrbCode
		,VRB_VALUE AS VrbValue
		,MTR_CODE AS MtrCode
		,CASE 
			WHEN rcount IS NULL
				THEN 0
			ELSE rcount
			END AS VrbCount
	FROM TD_CLASSIFICATION
	INNER JOIN td_variable
		ON cls_id = vrb_cls_id
	INNER JOIN TD_MATRIX
		ON CLS_MTR_ID = MTR_ID
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON MTR_ID = VRN_MTR_ID
	LEFT JOIN (
		SELECT CLS_ID AS clsID
			,COUNT(*) AS rcount
		FROM TD_CLASSIFICATION
		INNER JOIN TD_VARIABLE
			ON VRB_CLS_ID = CLS_ID
		GROUP BY CLS_ID
		) countQuery
		ON CLS_ID = clsID
	WHERE (CLS_ID = @ClsID)
END
GO


