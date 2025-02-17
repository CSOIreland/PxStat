CREATE TABLE [dbo].[TS_APP_SETTING] (
    [APP_ID]              INT           IDENTITY (1, 1) NOT NULL,
    [APP_ASV_ID]          INT           NULL,
    [APP_KEY]             VARCHAR (200) NULL,
    [APP_VALUE]           VARCHAR (MAX) NULL,
    [APP_DESCRIPTION]     VARCHAR (MAX) NULL,
    [APP_SENSITIVE_VALUE] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([APP_ID] ASC),
    CONSTRAINT [FK_TS_APP_SETTING_APP_SETTING_VERSION] FOREIGN KEY ([APP_ASV_ID]) REFERENCES [dbo].[TM_APP_SETTING_CONFIG_VERSION] ([ASV_ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_TS_APP_SETTING_APS_KEY]
    ON [dbo].[TS_APP_SETTING]([APP_KEY] ASC, [APP_ASV_ID] ASC);


GO
CREATE TRIGGER TRIG_TS_APP_SETTING_UPDATE
    ON TS_APP_SETTING
    AFTER UPDATE
    AS BEGIN
           SET NOCOUNT ON;
           INSERT INTO TS_HISTORY_APP_SETTING ([HPP_ASV_ID], [HPP_KEY], [HPP_VALUE], [HPP_DESCRIPTION], [HPP_APP_ID])
           SELECT a.APP_ASV_ID,
                  a.APP_KEY,
                  a.APP_VALUE,
                  a.APP_DESCRIPTION,
                  a.APP_ID
           FROM   TS_APP_SETTING AS a
                  INNER JOIN
                  inserted AS i
                  ON a.APP_ID = i.APP_ID;
       END


GO
CREATE TRIGGER TRIG_TS_APP_SETTING_INSERT
    ON TS_APP_SETTING
    AFTER INSERT
    AS BEGIN
           SET NOCOUNT ON;
           INSERT INTO TS_HISTORY_APP_SETTING ([HPP_ASV_ID], [HPP_KEY], [HPP_VALUE], [HPP_DESCRIPTION], [HPP_APP_ID])
           SELECT a.APP_ASV_ID,
                  a.APP_KEY,
                  a.APP_VALUE,
                  a.APP_DESCRIPTION,
                  a.APP_ID
           FROM   TS_APP_SETTING AS a
                  INNER JOIN
                  inserted AS i
                  ON a.APP_ID = i.APP_ID;
       END

