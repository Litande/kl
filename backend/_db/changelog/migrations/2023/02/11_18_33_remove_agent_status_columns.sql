--liquibase formatted sql

--changeset argent:11_18_33_remove_agent_status_columns
ALTER TABLE kl.user
    DROP COLUMN last_agent_status,
    DROP COLUMN last_agent_status_updated_at;
--rollback SELECT 1 FROM DUAL;