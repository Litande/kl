--liquibase formatted sql

--changeset argent:6_20_34_update_lead_history
ALTER TABLE kl.lead_history MODIFY COLUMN action_type enum('status','data','system') NOT NULL;
--rollback SELECT 1 FROM DUAL;
