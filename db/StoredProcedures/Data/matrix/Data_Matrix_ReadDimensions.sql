SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 26/01/2022
-- Description:	Get dimensions for a matrix id
-- exec Data_Matrix_ReadDimensions 20
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadDimensions @MtrId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MDM_ID AS MdmId
		,MDM_SEQUENCE AS MdmSequence
		,MDM_CODE AS MdmCode
		,MDM_VALUE AS MdmValue
		,MDM_GEO_FLAG AS MdmGeoFlag
		,MDM_GEO_URL AS MdmGeoUrl
		,DMR_CODE AS DmrCode
		,DMR_VALUE AS DmrValue
	FROM TD_MATRIX
	INNER JOIN TD_MATRIX_DIMENSION ON MTR_ID = MDM_MTR_ID
		AND MTR_ID = @MtrId
		AND MTR_DELETE_FLAG = 0
	INNER JOIN TS_DIMENSION_ROLE ON MDM_DMR_ID = DMR_ID
	ORDER BY MDM_SEQUENCE 
END
GO


