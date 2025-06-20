-- Migration script: V20250603124455_createTransactionSequenceWithTable.sql
-- Author: Mark Mngoma
-- Created on: 03/06/2025 12:44
CREATE SEQUENCE IF NOT EXISTS dboFinance.TRANSACTION_SEQ START WITH 1 INCREMENT BY 1 CACHE 1 NOCYCLE;

CREATE TABLE IF NOT EXISTS dboFinance.TRANSACTIONS (
   ID BIGINT UNSIGNED NOT NULL DEFAULT NEXTVAL(dboFinance.TRANSACTION_SEQ),
   TRANSACTION_ID VARCHAR(100) UNIQUE NOT NULL,
   SUBSCRIPTION_ID BIGINT UNSIGNED NOT NULL,
   TYPE VARCHAR(50) NOT NULL,
   AMOUNT DECIMAL(10, 2) NOT NULL,
   CURRENCY VARCHAR(3) NOT NULL DEFAULT 'ZAR',
   STATUS VARCHAR(50) NOT NULL DEFAULT 'PENDING',
   AUTHORIZATION_ID BIGINT UNSIGNED DEFAULT NULL,
   TRANSACTION_DATE DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
   DESCRIPTION VARCHAR(255) DEFAULT NULL,
   CREATED_AT DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
   UPDATED_AT DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
   PRIMARY KEY (ID),
   FOREIGN KEY (SUBSCRIPTION_ID) REFERENCES dboFinance.SUBSCRIPTIONS(ID),
   FOREIGN KEY (AUTHORIZATION_ID) REFERENCES dboFinance.TRANSACTIONS(ID)
);
