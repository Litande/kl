--liquibase formatted sql

--changeset argent:07_16_08_rename_cdr_callstartedat
ALTER TABLE kl.call_detail_record CHANGE COLUMN `started_at` `originated_at` timestamp not null DEFAULT CURRENT_TIMESTAMP;
--rollback SELECT 1 FROM DUAL;

--changeset argent:08_11_04_update_createdat_default
ALTER TABLE kl.`user` MODIFY COLUMN created_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL;
--rollback SELECT 1 FROM DUAL;
