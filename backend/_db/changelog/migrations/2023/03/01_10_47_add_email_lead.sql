--liquibase formatted sql

--changeset argent:01_10_47_add_email_lead
ALTER TABLE kl.`lead` ADD COLUMN `email` varchar(256) NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;