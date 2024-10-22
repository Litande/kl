--liquibase formatted sql

--changeset argent:01_18_05_add_timezone_lead
ALTER TABLE kl.`lead` ADD COLUMN `timezone` varchar(256) NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;