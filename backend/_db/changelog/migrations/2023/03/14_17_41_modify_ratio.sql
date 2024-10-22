--liquibase formatted sql

--changeset argent:14_17_41_modify_ratio
ALTER TABLE kl.`lead_queue` MODIFY COLUMN `ratio` DOUBLE NOT NULL;
--rollback SELECT 1 FROM DUAL;