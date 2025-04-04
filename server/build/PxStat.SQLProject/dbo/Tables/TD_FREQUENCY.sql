﻿CREATE TABLE [dbo].[TD_FREQUENCY] (
    [FRQ_ID]     INT            IDENTITY (1, 1) NOT NULL,
    [FRQ_CODE]   NVARCHAR (256) NOT NULL,
    [FRQ_VALUE]  NVARCHAR (256) NOT NULL,
    [FRQ_MTR_ID] INT            NOT NULL,
    CONSTRAINT [PK_TD_FREQUENCY] PRIMARY KEY CLUSTERED ([FRQ_ID] ASC),
    CONSTRAINT [FK_TD_FREQUENCY_TD_MATRIX] FOREIGN KEY ([FRQ_MTR_ID]) REFERENCES [dbo].[TD_MATRIX] ([MTR_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_FRQ_CODE]
    ON [dbo].[TD_FREQUENCY]([FRQ_CODE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FRQ_MTR_ID]
    ON [dbo].[TD_FREQUENCY]([FRQ_MTR_ID] ASC);

