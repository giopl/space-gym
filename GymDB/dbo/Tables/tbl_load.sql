CREATE TABLE [dbo].[tbl_load] (
    [title]              NVARCHAR (20)  NOT NULL,
    [firstname]          NVARCHAR (50)  NULL,
    [lastname]           NVARCHAR (50)  NULL,
    [fullname]           NVARCHAR (100) NULL,
    [gender]             NCHAR (1)      NULL,
    [dob]                DATE           NULL,
    [address_street]     NVARCHAR (100) NULL,
    [address_town]       NVARCHAR (50)  NULL,
    [email_address]      NVARCHAR (100) NULL,
    [home_phone]         NVARCHAR (20)  NULL,
    [office_phone]       NVARCHAR (20)  NULL,
    [mobile_phone]       NVARCHAR (20)  NULL,
    [club]               NVARCHAR (20)  NULL,
    [registration_date]  DATE           NULL,
    [is_active]          NCHAR (1)      NULL,
    [membership_type]    NVARCHAR (30)  NULL,
    [heard_about_us]     NVARCHAR (50)  NULL,
    [employer_name]      NVARCHAR (50)  NULL,
    [next_payment_date]  DATE           NULL,
    [reason_for_leaving] NVARCHAR (500) NULL,
    [created_by]         NVARCHAR (30)  NULL
);

