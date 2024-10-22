--liquibase formatted sql

--changeset argent:24_14_00_upd_sip_provider

ALTER TABLE kl.`sip_provider` ADD COLUMN status enum('enable','disable') not null default 'enable';

--rollback SELECT 1 FROM DUAL;
