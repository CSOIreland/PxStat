
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 25/01/2022
-- Description:	Load a dimension and return the mdm_id
-- =============================================
CREATE
	

 PROCEDURE Data_Matrix_Dimension_Create @MdmSequence INT
	,@MdmCode NVARCHAR(256)
	,@MdmValue NVARCHAR(256)
	,@MtrId INT
	,@DmrValue NVARCHAR(512)
	,@MdmGeoFlag BIT
	,@MdmGeoUrl NVARCHAR(2048) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DmrId INT
	DECLARE @errorMessage NVARCHAR(256)

	SET @DmrId = (
			SELECT DMR_ID
			FROM TS_DIMENSION_ROLE
			WHERE DMR_VALUE = @DmrValue
			)


	IF @DmrId IS NULL
	BEGIN
		SET @errorMessage = 'SP: Matrix_Dimension_Create - Dimension role not found: ' + @DmrValue

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END



	IF (
			SELECT COUNT(*)
			FROM TD_MATRIX_DIMENSION
			WHERE MDM_MTR_ID = @MtrId
				AND MDM_CODE = @MdmCode
				
			) > 0
	BEGIN
		SET @errorMessage = 'SP: Matrix_Dimension_Create - Duplicate Dimension Code: ' + @MdmCode

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	INSERT INTO TD_MATRIX_DIMENSION (
		MDM_SEQUENCE
		,MDM_CODE
		,MDM_VALUE
		,MDM_MTR_ID
		,MDM_DMR_ID
		,MDM_GEO_FLAG
		,MDM_GEO_URL
		)
	VALUES (
		@MdmSequence
		,@MdmCode
		,@MdmValue
		,@MtrId
		,@DmrId
		,@MdmGeoFlag
		,@MdmGeoUrl
		)

		RETURN @@IDENTITY
END
