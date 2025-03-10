﻿CREATE TABLE [dbo].[TM_CHANNEL_SUBSCRIPTION] (
    [CSB_ID]          INT IDENTITY (1, 1) NOT NULL,
    [CSB_CHN_ID]      INT NOT NULL,
    [CSB_USR_ID]      INT NOT NULL,
    [CSB_DELETE_FLAG] BIT NOT NULL,
    CONSTRAINT [PK_TD_CHANNEL_SUBSCRIPTION] PRIMARY KEY CLUSTERED ([CSB_ID] ASC),
    CONSTRAINT [FK_TD_CHANNEL_SUBSCRIPTION_TD_USER] FOREIGN KEY ([CSB_USR_ID]) REFERENCES [dbo].[TD_USER] ([USR_ID]),
    CONSTRAINT [FK_TM_CHANNEL_SUBSCRIPTION_TS_CHANNEL] FOREIGN KEY ([CSB_CHN_ID]) REFERENCES [dbo].[TS_CHANNEL] ([CHN_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_CSB_CHN_ID]
    ON [dbo].[TM_CHANNEL_SUBSCRIPTION]([CSB_CHN_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CSB_USR_ID]
    ON [dbo].[TM_CHANNEL_SUBSCRIPTION]([CSB_USR_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CSB_DELETE_FLAG]
    ON [dbo].[TM_CHANNEL_SUBSCRIPTION]([CSB_DELETE_FLAG] ASC);

