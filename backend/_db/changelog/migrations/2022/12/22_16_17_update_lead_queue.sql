--liquibase formatted sql

--changeset argent:update_lead_queue
ALTER TABLE kl.lead_queue
	ADD COLUMN `default`	tinyint(1)	not null default 0,
	ADD COLUMN `priority`	bigint unsigned	not null,
	ADD COLUMN `ratio`	bigint unsigned	not null,
	ADD COLUMN `type`	enum('default','future','cold')	not null default 'default';
--rollback SELECT 1 FROM DUAL;
