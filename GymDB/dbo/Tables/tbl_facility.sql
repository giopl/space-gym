CREATE TABLE [dbo].[tbl_facility] (
    [facility_id]     INT        IDENTITY (100, 1) NOT NULL,
    [transaction_id]  INT        NOT NULL,
    [due_date]        DATE       NULL,
    [due_amount]      FLOAT (53) NULL,
    [is_paid]         CHAR (1)   DEFAULT ('N') NULL,
    [installment_num] INT        NULL
);



