CREATE   PROCEDURE Security_Trace_Create
@TrcMethod NVARCHAR (256)=NULL, @TrcParams NVARCHAR (2048)=NULL, @TrcIp VARCHAR (15)=NULL, @TrcUseragent VARCHAR (2048)=NULL, @Username NVARCHAR (256)=NULL, @TrcStartTime DATETIME, @TrcDuration DECIMAL (18, 3), @TrcStatusCode INT, @TrcMachineName VARCHAR (256), @TrcErrorPath VARCHAR (1028)=NULL, @TrcRequestVerb VARCHAR (50), @TrcRequestType VARCHAR (50)=NULL, @TrcCorrelationID VARCHAR (1028), @TrcJsonRpcErrorCode INT=NULL, @TrcContentLength BIGINT=NULL, @TrcReferrer VARCHAR (MAX)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT  INTO TD_API_TRACE (TRC_METHOD, TRC_PARAMS, TRC_IP, TRC_USERAGENT, TRC_USERNAME, TRC_DATETIME, TRC_STARTTIME, TRC_DURATION, TRC_STATUSCODE, TRC_MACHINENAME, TRC_REQUEST_TYPE, TRC_REQUEST_VERB, TRC_ERROR_PATH, TRC_CORRELATION_ID, TRC_JSONRPC_ERROR_CODE, TRC_CONTENT_LENGTH, TRC_REFERER)
    VALUES                   (@TrcMethod, @TrcParams, @TrcIp, @Trcuseragent, @Username, getdate(), @TrcStartTime, @TrcDuration, @TrcStatusCode, @TrcMachineName, @TrcRequestType, @TrcRequestVerb, @TrcErrorPath, @TrcCorrelationID, @TrcJsonRpcErrorCode, @TrcContentLength, @TrcReferrer);
    RETURN 1;
END

