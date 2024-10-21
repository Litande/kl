--liquibase formatted sql

--changeset argent:27_10_45_add_imported_on_to_lead
ALTER TABLE dial.`lead` ADD COLUMN first_time_queued_on timestamp NULL DEFAULT NULL;
ALTER TABLE dial.`lead` ADD COLUMN imported_on timestamp NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;