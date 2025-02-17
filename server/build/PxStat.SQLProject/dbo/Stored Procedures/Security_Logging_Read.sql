CREATE   PROCEDURE Security_Logging_Read
@LggDatetimeStart DATETIME, @LggDatetimeEnd DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT LGG_DATETIME AS LggDatetime,
           LGG_THREAD AS LggThread,
           LGG_LEVEL AS LggLevel,
           LGG_CLASS AS LggClass,
           LGG_METHOD AS LggMethod,
           LGG_LINE AS LggLine,
           LGG_MESSAGE AS LggMessage,
           LGG_EXCEPTION AS LggException,
           LGG_MACHINENAME AS LggMachinename,
           LGG_CORRELATION_ID AS LggCorrelationID
    FROM   TD_LOGGING
    WHERE  LGG_DATETIME >= @LggDatetimeStart
           AND LGG_DATETIME <= @LggDatetimeEnd;
END

