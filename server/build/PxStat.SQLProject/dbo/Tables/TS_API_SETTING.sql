CREATE TABLE [dbo].[TS_API_SETTING] (
    [API_ID]              INT           IDENTITY (1, 1) NOT NULL,
    [API_ASV_ID]          INT           NULL,
    [API_KEY]             VARCHAR (200) NULL,
    [API_VALUE]           VARCHAR (MAX) NULL,
    [API_DESCRIPTION]     VARCHAR (MAX) NULL,
    [API_SENSITIVE_VALUE] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([API_ID] ASC),
    CONSTRAINT [FK_TS_API_SETTING_APP_SETTING_VERSION] FOREIGN KEY ([API_ASV_ID]) REFERENCES [dbo].[TM_APP_SETTING_CONFIG_VERSION] ([ASV_ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_TS_API_SETTING_APS_KEY]
    ON [dbo].[TS_API_SETTING]([API_KEY] ASC, [API_ASV_ID] ASC);


GO
CREATE TRIGGER TRIG_TS_API_SETTING_UPDATE
    ON TS_API_SETTING
    AFTER UPDATE
    AS BEGIN
           SET NOCOUNT ON;
           INSERT INTO TS_HISTORY_API_SETTING ([HPI_ASV_ID], [HPI_KEY], [HPI_VALUE], [HPI_DESCRIPTION], [HPI_API_ID])
           SELECT a.API_ASV_ID,
                  a.API_KEY,
                  a.API_VALUE,
                  a.API_DESCRIPTION,
                  a.API_ID
           FROM   TS_API_SETTING AS a
                  INNER JOIN
                  inserted AS i
                  ON a.API_ID = i.API_ID;
       END


GO
CREATE TRIGGER TRIG_TS_API_SETTING_INSERT
    ON TS_API_SETTING
    AFTER INSERT
    AS BEGIN
           SET NOCOUNT ON;
           INSERT INTO TS_HISTORY_API_SETTING ([HPI_ASV_ID], [HPI_KEY], [HPI_VALUE], [HPI_DESCRIPTION], [HPI_API_ID])
           SELECT a.API_ASV_ID,
                  a.API_KEY,
                  a.API_VALUE,
                  a.API_DESCRIPTION,
                  a.API_ID
           FROM   TS_API_SETTING AS a
                  INNER JOIN
                  inserted AS i
                  ON a.API_ID = i.API_ID;
       END

