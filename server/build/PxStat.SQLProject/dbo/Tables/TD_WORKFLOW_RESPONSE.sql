﻿CREATE TABLE [dbo].[TD_WORKFLOW_RESPONSE] (
    [WRS_ID]     INT IDENTITY (1, 1) NOT NULL,
    [WRS_RSP_ID] INT NOT NULL,
    [WRS_CMM_ID] INT NOT NULL,
    [WRS_WRQ_ID] INT NOT NULL,
    [WRS_DTG_ID] INT NOT NULL,
    CONSTRAINT [PK_TD_WORKFLOW_RESPONSE] PRIMARY KEY CLUSTERED ([WRS_ID] ASC),
    CONSTRAINT [FK_TD_WORKFLOW_RESPONSE_TD_AUDITING] FOREIGN KEY ([WRS_DTG_ID]) REFERENCES [dbo].[TD_AUDITING] ([DTG_ID]),
    CONSTRAINT [FK_TD_WORKFLOW_RESPONSE_TD_COMMENT] FOREIGN KEY ([WRS_CMM_ID]) REFERENCES [dbo].[TD_COMMENT] ([CMM_ID]),
    CONSTRAINT [FK_TD_WORKFLOW_RESPONSE_TD_WORKFLOW_REQUEST] FOREIGN KEY ([WRS_WRQ_ID]) REFERENCES [dbo].[TD_WORKFLOW_REQUEST] ([WRQ_ID]),
    CONSTRAINT [FK_TD_WORKFLOW_RESPONSE_TS_RESPONSE] FOREIGN KEY ([WRS_RSP_ID]) REFERENCES [dbo].[TS_RESPONSE] ([RSP_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_WRS_CMM_ID]
    ON [dbo].[TD_WORKFLOW_RESPONSE]([WRS_CMM_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WRS_DTG_ID]
    ON [dbo].[TD_WORKFLOW_RESPONSE]([WRS_DTG_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WRS_RSP_ID]
    ON [dbo].[TD_WORKFLOW_RESPONSE]([WRS_RSP_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WRS_WRQ_ID]
    ON [dbo].[TD_WORKFLOW_RESPONSE]([WRS_WRQ_ID] ASC);

