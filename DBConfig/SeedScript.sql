
SET NOCOUNT ON;

--------------------------
-- Reset & Seed FinBank --
--------------------------
USE FinBank;

-- Reset
DELETE FROM dbo.IdempotencyKeys;
DELETE FROM dbo.Transfers;
DELETE FROM dbo.Accounts;
DELETE FROM dbo.Employees;
DELETE FROM dbo.Customers;

-- Customers
DECLARE @custAlice UNIQUEIDENTIFIER = NEWID();
DECLARE @custBob   UNIQUEIDENTIFIER = NEWID();
DECLARE @custCarol UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Customers (CustomerId, CreatedAt, Name, PhoneNumber, Country, Birthday, Address)
VALUES
(@custAlice, SYSUTCDATETIME(), N'Alice Martin',  N'+40-700-000-001', N'RO', '1990-03-05', N'Str. Lalelelor 10, București'),
(@custBob,   SYSUTCDATETIME(), N'Bob Ionescu',   N'+40-700-000-002', N'RO', '1987-11-21', N'Str. Mărului 5, Cluj-Napoca'),
(@custCarol, SYSUTCDATETIME(), N'Carol Pop',     N'+40-700-000-003', N'RO', '1993-06-14', N'Timișoara, Bd. Revoluției 1');

-- Employees
DECLARE @empReviewer UNIQUEIDENTIFIER = NEWID();
DECLARE @empAnalyst  UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Employees (EmployeeId, Role, CreatedAt, Name, PhoneNumber, Country, Birthday, Address)
VALUES
(@empReviewer, N'Reviewer',  SYSUTCDATETIME(), N'Reviewer Two', N'+40-710-000-002', N'RO', '1989-04-12', N'Cluj'),
(@empAnalyst,  N'Analyst',   SYSUTCDATETIME(), N'Analyst One',  N'+40-710-000-001', N'RO', '1991-01-01', N'București');

-- Accounts (EUR & RON examples)
DECLARE @aliceEur NVARCHAR(34) = N'RO71BANK000000000000000001';
DECLARE @aliceRon NVARCHAR(34) = N'RO71BANK000000000000000002';
DECLARE @bobEur   NVARCHAR(34) = N'RO71BANK000000000000000003';
DECLARE @carolUsd NVARCHAR(34) = N'RO71BANK000000000000000004';

INSERT INTO dbo.Accounts (IBAN, CustomerId, CreatedAt, Currency)
VALUES
(@aliceEur, @custAlice,  SYSUTCDATETIME(), 'EUR'),
(@aliceRon, @custAlice,  SYSUTCDATETIME(), 'RON'),
(@bobEur,   @custBob,    SYSUTCDATETIME(), 'EUR'),
(@carolUsd, @custCarol,  SYSUTCDATETIME(), 'USD');

-- Transfers
DECLARE @t1 UNIQUEIDENTIFIER = NEWID();
DECLARE @t2 UNIQUEIDENTIFIER = NEWID();
DECLARE @t3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Transfers
(TransferId, FromAccountId, ToAccountId, ReviewedBy, CreatedAt, Status, Amount, Currency, Reason, CompletedAt, PolicyVersion)
VALUES
(@t1, @aliceEur, @bobEur,   NULL,         SYSUTCDATETIME(), 'Completed', 550.00, 'EUR', NULL          , SYSUTCDATETIME(), 'v1'),
(@t2, @carolUsd, @aliceEur, @empReviewer, SYSUTCDATETIME(), 'Rejected',  999.99, 'USD', N'KYC Blocked', SYSUTCDATETIME(), 'v1'),
(@t3, @aliceRon, @bobEur,   NULL,         SYSUTCDATETIME(), 'Pending',    75.25, 'RON', NULL,           NULL,             'v1');


----------------------
-- Reset & Seed KYC --
----------------------
USE KYC;

DELETE FROM dbo.CustomerRisk;

INSERT INTO dbo.CustomerRisk (CustomerId, RiskStatus, UpdatedAt)
VALUES
(@custAlice, 'Low',     SYSUTCDATETIME()),
(@custBob,   'Medium',  SYSUTCDATETIME()),
(@custCarol, 'High',    SYSUTCDATETIME());

PRINT 'Seeding completed.';