--liquibase formatted sql

--changeset argent:11_16_25_add_max_min_ratio
ALTER TABLE dial.`lead_queue`
    ADD COLUMN max_ratio DOUBLE NULL DEFAULT NULL,
    ADD COLUMN min_ratio DOUBLE NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;