CREATE TABLE [dbo].[tbl_member] (
    [member_id]               INT            NOT NULL,
    [title]                   NVARCHAR (20)  NOT NULL,
    [firstname]               NVARCHAR (50)  NULL,
    [lastname]                NVARCHAR (100) NULL,
    [fullname]                NVARCHAR (100) NULL,
    [gender]                  NCHAR (1)      NULL,
    [dob]                     DATE           NULL,
    [age]                     INT            NULL,
    [address_street]          NVARCHAR (100) NULL,
    [address_town]            NVARCHAR (50)  NULL,
    [email_address]           NVARCHAR (100) NULL,
    [home_phone]              NVARCHAR (20)  NULL,
    [office_phone]            NVARCHAR (20)  NULL,
    [mobile_phone]            NVARCHAR (20)  NULL,
    [profile_pic_ext]         VARCHAR (50)   NULL,
    [club]                    NVARCHAR (20)  NULL,
    [registration_date]       DATE           NULL,
    [is_reg_paid]             CHAR (1)       NULL,
    [is_active]               NCHAR (1)      NULL,
    [membership_type]         NVARCHAR (30)  NULL,
    [custom_registration_fee] FLOAT (53)     NULL,
    [custom_monthly_fee]      FLOAT (53)     NULL,
    [max_visits]              INT            NULL,
    [visits_left]             INT            NULL,
    [last_transaction_id]     INT            NULL,
    [is_part_payment]         NCHAR (1)      NULL,
    [installment_date]        DATE           NULL,
    [relationship_id]         INT            NULL,
    [heard_about_us]          NVARCHAR (50)  NULL,
    [employer_name]           NVARCHAR (50)  NULL,
    [occupation]              VARCHAR (200)  NULL,
    [payment_until]           DATE           NULL,
    [reason_for_leaving]      NVARCHAR (500) NULL,
    [num_visits]              INT            NULL,
    [last_visit]              DATETIME       NULL,
    [created_by]              NVARCHAR (30)  NULL,
    [updated_by]              NVARCHAR (30)  NULL,
    [last_updated_on]         DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([member_id] ASC)
);

















