CREATE TABLE [dbo].[TD_API_TRACE] (
    [TRC_ID]                 INT             IDENTITY (1, 1) NOT NULL,
    [TRC_METHOD]             VARCHAR (256)   NULL,
    [TRC_PARAMS]             NVARCHAR (2048) NULL,
    [TRC_IP]                 VARCHAR (15)    NULL,
    [TRC_USERAGENT]          VARCHAR (2048)  NULL,
    [TRC_USERNAME]           NVARCHAR (256)  NULL,
    [TRC_DATETIME]           DATETIME        NOT NULL,
    [TRC_STARTTIME]          DATETIME        NOT NULL,
    [TRC_DURATION]           DECIMAL (18, 3) NOT NULL,
    [TRC_STATUSCODE]         INT             NOT NULL,
    [TRC_MACHINENAME]        VARCHAR (256)   NOT NULL,
    [TRC_REQUEST_TYPE]       VARCHAR (50)    NULL,
    [TRC_REQUEST_VERB]       VARCHAR (50)    NOT NULL,
    [TRC_ERROR_PATH]         VARCHAR (1024)  NULL,
    [TRC_CORRELATION_ID]     VARCHAR (400)   NULL,
    [TRC_JSONRPC_ERROR_CODE] INT             NULL,
    [TRC_CONTENT_LENGTH]     BIGINT          NULL,
    [TRC_REFERER]            VARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([TRC_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the method that was called', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_METHOD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the parameters that were passed in', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_PARAMS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the ip address of the request', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_IP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the useragent of the request (browser)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_USERAGENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the username if available of the person that made the request', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_USERNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the type of request e.g. jsonrpc', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_REQUEST_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the request method e.g. get, post', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_REQUEST_VERB';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the path of the url if there is an error', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TD_API_TRACE', @level2type = N'COLUMN', @level2name = N'TRC_ERROR_PATH';

