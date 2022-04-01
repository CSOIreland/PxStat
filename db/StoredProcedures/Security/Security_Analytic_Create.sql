SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 16/01/2019
-- Description:	Adds an entry to the analytic table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Analytic_Create @matrix NVARCHAR(20)
	,@NltMaskedIp NVARCHAR(11)
	,@NltOs NVARCHAR(32) = NULL
	,@NltBrowser NVARCHAR(32) = NULL
	,@NltBotFlag BIT
	,@NltReferer NVARCHAR(256) = NULL
	,@NltM2m BIT
	,@NltWidget BIT=NULL
	,@NltDate DATE
	,@LngIsoCode CHAR(2)
	,@FrmType NVARCHAR(32) = NULL
	,@FrmVersion NVARCHAR(32) = NULL
	,@EnvironmentLngIsoCode CHAR(2)=NULL
	,@NltUser BIT=0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MtrID AS INT;
	DECLARE @FrmID AS INT;

	SELECT @MtrID = MTR_ID
	FROM TD_MATRIX
	INNER JOIN VW_RELEASE_LIVE_NOW
		ON MTR_ID = VRN_MTR_ID
	INNER JOIN TS_LANGUAGE
		ON LNG_ID = MTR_LNG_ID
			AND LNG_DELETE_FLAG = 0
			AND LNG_ISO_CODE = @LngIsoCode
			AND MTR_CODE = @matrix;

	IF @MtrID IS NULL
	BEGIN
		RETURN 0
	END

	SELECT @FrmID = FRM_ID
	FROM TS_FORMAT
	WHERE @FrmType IS NOT NULL
		AND @FrmVersion IS NOT NULL
		AND FRM_DIRECTION = 'DOWNLOAD'
		AND FRM_TYPE = @FrmType
		AND FRM_VERSION = @FrmVersion;

	IF @FrmID IS NULL
	BEGIN
		RETURN 0
	END

	INSERT INTO TD_ANALYTIC (
		NLT_MTR_ID
		,NLT_MASKED_IP
		,NLT_OS
		,NLT_BROWSER
		,NLT_BOT_FLAG
		,NLT_REFERER
		,NLT_M2M_FLAG
		,NLT_WIDGET 
		,NLT_DATE
		,NLT_USER_FLAG
		,NLT_FRM_ID
		,NLT_LNG_ISO_CODE 
		)
	VALUES (
		@MtrID
		,@NltMaskedIp
		,@NltOs
		,@NltBrowser
		,@NltBotFlag
		,@NltReferer
		,@NltM2m
		,coalesce(@NltWidget,0) 
		,@NltDate
		,@NltUser 
		,@FrmID
		,@EnvironmentLngIsoCode
		);

	RETURN @@identity;
END
GO