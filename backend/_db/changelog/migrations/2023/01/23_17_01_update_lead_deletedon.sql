--liquibase formatted sql

--changeset argent:update_lead_deletedon
alter table kl.lead
	ADD COLUMN `deleted_on` timestamp NULL Default NULL;
--rollback SELECT 1 FROM DUAL;
