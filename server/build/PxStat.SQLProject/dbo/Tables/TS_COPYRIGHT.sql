﻿CREATE TABLE [dbo].[TS_COPYRIGHT] (
    [CPR_ID]          INT             IDENTITY (1, 1) NOT NULL,
    [CPR_CODE]        NVARCHAR (32)   NOT NULL,
    [CPR_VALUE]       NVARCHAR (256)  NOT NULL,
    [CPR_URL]         NVARCHAR (2048) NOT NULL,
    [CPR_DTG_ID]      INT             NOT NULL,
    [CPR_DELETE_FLAG] BIT             CONSTRAINT [DF_TS_COPYRIGHT_CPR_DELETE_FLAG] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TS_COPYRIGHT] PRIMARY KEY CLUSTERED ([CPR_ID] ASC),
    CONSTRAINT [FK_TS_COPYRIGHT_TD_AUDITING] FOREIGN KEY ([CPR_DTG_ID]) REFERENCES [dbo].[TD_AUDITING] ([DTG_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_CPR_CODE]
    ON [dbo].[TS_COPYRIGHT]([CPR_CODE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CPR_DELETE]
    ON [dbo].[TS_COPYRIGHT]([CPR_DELETE_FLAG] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CPR_DTG_ID]
    ON [dbo].[TS_COPYRIGHT]([CPR_DTG_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CPR_VALUE]
    ON [dbo].[TS_COPYRIGHT]([CPR_VALUE] ASC);

