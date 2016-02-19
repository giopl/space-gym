CREATE TABLE [dbo].[tbl_admin] (
    [user_id]          INT            IDENTITY (100, 1) NOT NULL,
    [username]         NVARCHAR (30)  NOT NULL,
    [fullname]         NVARCHAR (50)  NULL,
    [password]         NVARCHAR (200) NULL,
    [is_active]        NCHAR (1)      NULL,
    [last_login]       DATETIME       NULL,
    [access_level]     INT            NULL,
    [num_logins]       INT            CONSTRAINT [DF_tbl_admin_num_logins] DEFAULT ((0)) NULL,
    [is_temp_password] CHAR (1)       DEFAULT ('N') NULL,
    [email_addr]       NVARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([username] ASC)
);

