--liquibase formatted sql

--changeset argent:09_12_00_lead_cleanup
ALTER TABLE kl.`lead` MODIFY COLUMN registration_time timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL;
--rollback SELECT 1 FROM DUAL;
