--liquibase formatted sql

--changeset argent:26_16_00_add_createad_update_at_to_cdr
ALTER TABLE `dial`.call_detail_record
    ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    ADD COLUMN bridge_id VARCHAR(100) NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;