--liquibase formatted sql

--changeset argent:23_11_21_add_call_duration
ALTER TABLE kl.`call_detail_record` ADD COLUMN `call_duration` BIGINT UNSIGNED NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;
