SET NOCOUNT ON;

--------------------------
-- Reset & Seed FinBank --
--------------------------
USE FinBank;
GO

-- Reset data (maintaining FK order)
DELETE FROM dbo.IdempotencyKeys;
DELETE FROM dbo.Transfers;
DELETE FROM dbo.Accounts;
DELETE FROM dbo.Users;
GO

-- Users
DECLARE @custAlice UNIQUEIDENTIFIER = NEWID();
DECLARE @custBob   UNIQUEIDENTIFIER = NEWID();
DECLARE @custCarol UNIQUEIDENTIFIER = NEWID();
DECLARE @empReviewer UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Users (UserId, Email, Password, Role, CreatedAt, Name, PhoneNumber, Country, Birthday, Address)
VALUES
(@custAlice, N'alice@finbank.test',  N'hash_pwd_alice',  N'Customer', SYSUTCDATETIME(), N'Alice Martin',  N'+40-700-000-001', N'RO', '1990-03-05', N'Str. Lalelelor 10, București'),
(@custBob,   N'bob@finbank.test',    N'hash_pwd_bob',    N'Customer', SYSUTCDATETIME(), N'Bob Ionescu',   N'+40-700-000-002', N'RO', '1987-11-21', N'Str. Mărului 5, Cluj-Napoca'),
(@custCarol, N'carol@finbank.test',  N'hash_pwd_carol',  N'Customer', SYSUTCDATETIME(), N'Carol Pop',     N'+40-700-000-003', N'RO', '1993-06-14', N'Timișoara, Bd. Revoluției 1'),
(@empReviewer, N'reviewer@finbank.test', N'hash_pwd_reviewer', N'Employee', SYSUTCDATETIME(), N'Risk Reviewer', N'+40-700-000-004', N'RO', NULL, N'București HQ');
GO

-- Accounts
DECLARE @aliceEur NVARCHAR(34) = N'RO71BANK000000000000000001';
DECLARE @aliceRon NVARCHAR(34) = N'RO71BANK000000000000000002';
DECLARE @bobEur   NVARCHAR(34) = N'RO71BANK000000000000000003';
DECLARE @carolUsd NVARCHAR(34) = N'RO71BANK000000000000000004';

INSERT INTO dbo.Accounts (IBAN, CustomerId, CreatedAt, Balance, Currency)
VALUES
(@aliceEur, @custAlice, SYSUTCDATETIME(), 1000.50, 'EUR'),
(@aliceRon, @custAlice, SYSUTCDATETIME(), 5345.00, 'RON'),
(@bobEur,   @custBob,   SYSUTCDATETIME(), 420.00,  'EUR'),
(@carolUsd, @custCarol, SYSUTCDATETIME(), 100.99,  'USD');
GO

-- Transfers
DECLARE @t1 UNIQUEIDENTIFIER = NEWID();
DECLARE @t2 UNIQUEIDENTIFIER = NEWID();
DECLARE @t3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Transfers
(TransferId, FromAccountId, ToAccountId, ReviewedBy, CreatedAt, Status, Amount, Currency, Reason, CompletedAt, PolicyVersion)
VALUES
(@t1, @aliceEur, @bobEur,   NULL,         SYSUTCDATETIME(), 'Completed', 550.00, 'EUR', NULL,           SYSUTCDATETIME(), 'v1'),
(@t2, @carolUsd, @aliceEur, @empReviewer, SYSUTCDATETIME(), 'Rejected',  999.99, 'USD', N'KYC Blocked', SYSUTCDATETIME(), 'v1'),
(@t3, @aliceRon, @bobEur,   NULL,         SYSUTCDATETIME(), 'Pending',    75.25, 'RON', NULL,           NULL,             'v1');
GO


----------------------
-- Reset & Seed KYC --
----------------------
USE KYC;
GO

DELETE FROM dbo.CustomerRisk;
GO

INSERT INTO dbo.CustomerRisk (CustomerId, RiskStatus, UpdatedAt)
VALUES
(@custAlice, 'Low',     SYSUTCDATETIME()),
(@custBob,   'Medium',  SYSUTCDATETIME()),
(@custCarol, 'High',    SYSUTCDATETIME());
GO

PRINT 'Seeding completed successfully.';
GO
