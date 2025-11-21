## Overview

FinBank is a **transaction management API** for a digital bank.  
It manages **customers, accounts, and fund transfers** with full **auditing and transactional integrity** using EF Core and SQL Server.  

Designed as a **Clean Architecture** system, it applies CQRS, Repository + Unit of Work, and MediatR for reliable, scalable, and testable financial operations.  

All **money movement** is gated by **KYC and risk policies**, enforced via a dedicated KYC service that exposes verification status, risk level, and screening data.
