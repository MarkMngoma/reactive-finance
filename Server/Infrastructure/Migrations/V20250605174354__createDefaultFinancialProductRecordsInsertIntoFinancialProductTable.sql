-- Migration script: V20250605174354__createDefaultFinancialProductRecordsInsertIntoFinancialProductTable.sql
-- Author: Mark Mngoma
-- Created on: 2025-06-05 17:43:54
INSERT INTO dboFinance.FINANCIAL_PRODUCT (NAME, DESCRIPTION, PRICE, BILLING_CYCLE)
VALUES('Essentials', 'The essentials to grow your business.', 10.00, 3);

INSERT INTO dboFinance.FINANCIAL_PRODUCT (NAME, DESCRIPTION, PRICE, BILLING_CYCLE)
VALUES('Business Plan', 'A plan that scales with your rapidly growing business.', 1200.00, 3);

INSERT INTO dboFinance.FINANCIAL_PRODUCT (NAME, DESCRIPTION, PRICE, BILLING_CYCLE)
VALUES('Premium Plan', 'For growing teams that need more services and flexibility.', 4000.00, 3);
