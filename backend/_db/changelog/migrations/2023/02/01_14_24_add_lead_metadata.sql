--liquibase formatted sql

--changeset argent:01_14_24_add_lead_metadata
ALTER TABLE `dial`.lead ADD COLUMN `metadata` json NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;