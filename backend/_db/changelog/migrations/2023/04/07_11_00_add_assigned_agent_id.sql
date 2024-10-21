--liquibase formatted sql

--changeset argent:07_11_00_add_assigned_agent_id
ALTER TABLE dial.`lead`
    ADD COLUMN assigned_agent_id BIGINT(20) UNSIGNED NULL,
    ADD CONSTRAINT `lead_assigned_user_id_FK` FOREIGN KEY (assigned_agent_id) REFERENCES `user` (user_id),
    DROP COLUMN is_fixed_assigned;
--rollback SELECT 1 FROM DUAL;