--liquibase formatted sql

--changeset argent:28_18_34_update_ratio
ALTER TABLE dial.`lead_queue` ADD COLUMN `ratio_updated_at` timestamp NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;
