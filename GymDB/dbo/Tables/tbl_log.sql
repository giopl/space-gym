CREATE TABLE [dbo].[tbl_log] (
    [log_id]        INT            IDENTITY (100, 1) NOT NULL,
    [username]      VARCHAR (30)   NULL,
    [operation]     VARCHAR (100)  NULL,
    [item_type]     VARCHAR (100)  NULL,
    [item_id]       INT            NULL,
    [log_timestamp] DATETIME       NULL,
    [description]   VARCHAR (1000) NULL
);

