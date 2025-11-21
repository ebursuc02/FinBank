---------------------------------------
-- Create databases (if not created) --
---------------------------------------
IF DB_ID('FinBank') IS NULL
BEGIN
  CREATE DATABASE FinBank;
END;
GO

IF DB_ID('KYC') IS NULL
BEGIN
  CREATE DATABASE KYC;
END;
GO


---------------------------------------
------- FinBank schema & tables -------
---------------------------------------
USE FinBank;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO


-- Customers --
IF OBJECT_ID('dbo.Customers','U') IS NOT NULL DROP TABLE dbo.Customers;
GO
CREATE TABLE dbo.Customers
(
    CustomerId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
    CreatedAt      DATETIME2(3)     NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT SYSUTCDATETIME(),
    Name           NVARCHAR(200)    NOT NULL,
    PhoneNumber    NVARCHAR(50)     NOT NULL,
    Country        NVARCHAR(100)    NULL,
    Birthday       DATE             NULL,
    Address        NVARCHAR(300)    NULL
);
GO


-- Employees --
IF OBJECT_ID('dbo.Employees','U') IS NOT NULL DROP TABLE dbo.Employees;
GO
CREATE TABLE dbo.Employees
(
    EmployeeId    UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Employee PRIMARY KEY,
    Role          NVARCHAR(50)     NOT NULL,
    CreatedAt     DATETIME2(3)     NOT NULL CONSTRAINT DF_Employee_CreatedAt DEFAULT SYSUTCDATETIME(),
    Name          NVARCHAR(200)    NOT NULL,
    PhoneNumber   NVARCHAR(50)     NOT NULL,
    Country       NVARCHAR(100)    NULL,
    Birthday      DATE             NULL,
    Address       NVARCHAR(300)    NULL
    
);
GO


-- Accounts --
IF OBJECT_ID('dbo.Accounts','U') IS NOT NULL DROP TABLE dbo.Accounts;
GO
CREATE TABLE dbo.Accounts
(
    AccountId  UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Accounts PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    IBAN       NVARCHAR(34)     NOT NULL,
    CreatedAt  DATETIME2(3)     NOT NULL CONSTRAINT DF_Accounts_CreatedAt DEFAULT SYSUTCDATETIME(),
    Currency   CHAR(3)          NOT NULL,

    CONSTRAINT UQ_Accounts_IBAN UNIQUE (IBAN),
    CONSTRAINT FK_Accounts_Customers
        FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO
CREATE INDEX IX_Accounts_CustomerId ON dbo.Accounts(CustomerId);
GO


-- Transfers --
IF OBJECT_ID('dbo.Transfers','U') IS NOT NULL DROP TABLE dbo.Transfers;
GO
CREATE TABLE dbo.Transfers
(
    TransferId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Transfers PRIMARY KEY,
    FromAccountId  UNIQUEIDENTIFIER NOT NULL,
    ToAccountId    UNIQUEIDENTIFIER NOT NULL,
    ReviewedBy     UNIQUEIDENTIFIER NULL,
    CreatedAt      DATETIME2(3)     NOT NULL CONSTRAINT DF_Transfers_CreatedAt DEFAULT SYSUTCDATETIME(),
    Status         VARCHAR(30)      NOT NULL,   -- Pending, Completed, Rejected, UnderReview
    Amount         DECIMAL(18,2)    NOT NULL,
    Currency       CHAR(3)          NOT NULL,
    Reason         NVARCHAR(500)    NULL,
    CompletedAt    DATETIME2(3)     NULL,
    PolicyVersion  VARCHAR(20)      NULL,

    CONSTRAINT CK_Transfers_Status
        CHECK (Status IN ('Pending','Completed','Rejected','UnderReview')),
    CONSTRAINT CK_Transfers_FromToDifferent
        CHECK (FromAccountId <> ToAccountId),

    CONSTRAINT FK_Transfers_FromAccount
        FOREIGN KEY (FromAccountId) REFERENCES dbo.Accounts(AccountId)
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Transfers_ToAccount
        FOREIGN KEY (ToAccountId)   REFERENCES dbo.Accounts(AccountId)
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Transfers_ReviewedBy
        FOREIGN KEY (ReviewedBy)    REFERENCES dbo.Employees(EmployeeId)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO
CREATE INDEX IX_Transfers_FromAccountId ON dbo.Transfers(FromAccountId);
CREATE INDEX IX_Transfers_ToAccountId   ON dbo.Transfers(ToAccountId);
CREATE INDEX IX_Transfers_StatusCreated ON dbo.Transfers(Status, CreatedAt);
GO


-- IdempotencyKeys --
IF OBJECT_ID('dbo.IdempotencyKeys','U') IS NOT NULL DROP TABLE dbo.IdempotencyKeys;
GO
CREATE TABLE dbo.IdempotencyKeys
(
    [Key]             NVARCHAR(100)  NOT NULL CONSTRAINT PK_IdempotencyKeys PRIMARY KEY,
    TransferId        UNIQUEIDENTIFIER NOT NULL,
    RequestHash       NVARCHAR(200)  NULL,
    ResponseJson      NVARCHAR(MAX)  NULL,
    FirstProcessedAt  DATETIME2(3)   NULL,

    CONSTRAINT FK_Idem_Transfer
        FOREIGN KEY (TransferId) REFERENCES dbo.Transfers(TransferId)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO
CREATE INDEX IX_Idem_TransferId ON dbo.IdempotencyKeys(TransferId);
CREATE UNIQUE INDEX UX_Idem_RequestHash ON dbo.IdempotencyKeys(RequestHash) WHERE RequestHash IS NOT NULL;
GO


---------------------------------------
------- KYC database and schema -------
---------------------------------------
USE KYC;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- CustomerRisk --
IF OBJECT_ID('dbo.CustomerRisk','U') IS NOT NULL DROP TABLE dbo.CustomerRisk;
GO
CREATE TABLE dbo.CustomerRisk
(
    CustomerId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CustomerRisk PRIMARY KEY,
    RiskStatus VARCHAR(20)      NOT NULL,  -- Low, Medium, High, Blocked, Unknown
    UpdatedAt  DATETIME2(3)     NOT NULL CONSTRAINT DF_CustomerRisk_UpdatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_CustomerRisk_Status CHECK (RiskStatus IN ('Low','Medium','High','Blocked','Unknown'))
);
GO



PRINT 'Databases FinBank and KYC created with required tables and constraints.';
GO
