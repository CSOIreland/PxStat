CREATE   PROCEDURE Security_Api_Trace_Read
@StartDate DATETIME, @EndDate DATETIME, @Const_AUTHENTICATED NVARCHAR (32), @Const_REGISTERED NVARCHAR (32), @Const_ANONYMOUS NVARCHAR (32), @Const_ANY NVARCHAR (32), @AuthenticationType VARCHAR (32)=NULL, @TrcUsername NVARCHAR (256)=NULL, @TrcIp VARCHAR (15)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @AuthenticationType IS NULL
        BEGIN
            SET @AuthenticationType = @Const_ANY;
        END
    SELECT   t.TRC_USERNAME AS TrcCcnUsername,
             a.PRV_VALUE AS TrcPrvValue,
             t.trc_ip AS TrcIp,
             t.TRC_METHOD AS TrcMethod,
             t.trc_params AS TrcParams,
             t.trc_useragent AS TrcUserAgent,
             t.trc_datetime AS TrcDatetime,
             t.TRC_STARTTIME AS TrcStartTime,
             t.TRC_DURATION AS TrcDuration,
             t.TRC_STATUSCODE AS TrcStatusCode,
             t.TRC_REQUEST_TYPE AS TrcRequestType,
             t.TRC_REQUEST_VERB AS TrcRequestVerb,
             t.TRC_MACHINENAME AS TrcMachinename,
             t.TRC_CORRELATION_ID AS TrcCorrelationId,
             t.TRC_ERROR_PATH AS TrcErrorPath,
             t.TRC_CONTENT_LENGTH AS TrcContentLength,
             t.TRC_REFERER AS TrcReferer
    FROM     TD_API_TRACE AS t
             LEFT OUTER JOIN
             (SELECT ac.CCN_USERNAME,
                     ac.CCN_PRV_ID,
                     ac.CCN_ID,
                     pr.PRV_VALUE
              FROM   TD_ACCOUNT AS ac
                     INNER JOIN
                     TS_PRIVILEGE AS pr
                     ON ac.CCN_PRV_ID = pr.PRV_ID
                        AND ac.CCN_DELETE_FLAG = 0
              WHERE  ccn_delete_flag = 0) AS a
             ON t.TRC_USERNAME = a.CCN_USERNAME
    WHERE    t.TRC_DATETIME >= @StartDate
             AND t.TRC_DATETIME <= @EndDate
             AND ((@AuthenticationType = @Const_REGISTERED
                   AND a.CCN_USERNAME IS NOT NULL)
                  OR (@AuthenticationType = @Const_ANONYMOUS
                      AND a.CCN_USERNAME IS NULL
                      AND t.TRC_USERNAME IS NULL)
                  OR (@AuthenticationType = @Const_AUTHENTICATED
                      AND t.TRC_USERNAME IS NOT NULL
                      AND a.CCN_USERNAME IS NULL)
                  OR (@AuthenticationType = @Const_ANY))
             AND (@TrcUsername IS NULL
                  OR t.TRC_USERNAME = @TrcUsername)
             AND (@TrcIp IS NULL
                  OR t.TRC_IP = @TrcIp)
    ORDER BY t.TRC_DATETIME;
END

