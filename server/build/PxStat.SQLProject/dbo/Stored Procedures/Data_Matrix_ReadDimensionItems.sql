
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 26/01/2022
-- Description:	Get dimension items for a dimension id
-- exec Data_Matrix_ReadDimensionItems 9
-- =============================================
CREATE
	

 PROCEDURE Data_Matrix_ReadDimensionItems @MdmId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DMT_CODE AS DmtCode
		,DMT_VALUE AS DmtValue
		,DMT_SEQUENCE AS DmtSequence
		,DMT_ELIMINATION_FLAG AS DmtEliminationFlag
		,DMT_DECIMAL AS DmtDecimals
		,DMT_UNIT AS DmtUnit
	FROM TD_DIMENSION_ITEM
	WHERE DMT_MDM_ID = @MdmId
	ORDER BY DMT_SEQUENCE 
END
