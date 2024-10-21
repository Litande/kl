--liquibase formatted sql

--changeset argent:6_20_34_update_lead_history
ALTER TABLE dial.lead_history MODIFY COLUMN action_type enum('status','data','system') NOT NULL;
--rollback SELECT 1 FROM DUAL;
