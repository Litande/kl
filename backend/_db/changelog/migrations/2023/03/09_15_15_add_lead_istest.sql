--liquibase formatted sql

--changeset argent:09_15_15_add_lead_istest
ALTER TABLE dial.`lead` ADD COLUMN `is_test` tinyint(1) NOT NULL DEFAULT 0;
--rollback SELECT 1 FROM DUAL;
