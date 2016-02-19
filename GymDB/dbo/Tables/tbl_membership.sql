CREATE TABLE [dbo].[tbl_membership] (
    [membership_id]    INT            IDENTITY (100, 1) NOT NULL,
    [code]             NVARCHAR (20)  NOT NULL,
    [name]             NVARCHAR (150) NOT NULL,
    [description]      NVARCHAR (500) NULL,
    [membership_rules] VARCHAR (1000) NULL,
    [registration_fee] FLOAT (53)     NULL,
    [fee]              FLOAT (53)     NULL,
    [month_terms]      INT            NULL,
    [num_members]      INT            NULL,
    [is_active]        CHAR (1)       DEFAULT ('Y') NULL,
    [updated_on]       DATETIME       NULL,
    [updated_by]       VARCHAR (50)   NULL,
    [display_order]    INT            NULL,
    [is_system]        CHAR (1)       DEFAULT ('N') NULL,
    [max_visits]       INT            NULL,
    [is_pass]          CHAR (1)       NULL,
    [category]         VARCHAR (20)   NULL,
    PRIMARY KEY CLUSTERED ([code] ASC)
);







