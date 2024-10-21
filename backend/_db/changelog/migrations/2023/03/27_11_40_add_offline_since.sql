--liquibase formatted sql

--changeset argent:27_11_40_add_offline_since_column
ALTER TABLE dial.`user` ADD COLUMN `offline_since` timestamp NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;