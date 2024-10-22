--liquibase formatted sql

--changeset argent:lead_add_system_status
alter table kl.lead
	MODIFY COLUMN `system_status` enum('processing','inthecall','waitingfeedback') NULL Default NULL;
--rollback SELECT 1 FROM DUAL;