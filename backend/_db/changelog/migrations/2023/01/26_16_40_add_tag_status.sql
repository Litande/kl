--liquibase formatted sql

--changeset argent:25_13_07_add_tag_status
ALTER TABLE kl.tag ADD COLUMN status enum('enable','disable') not null default 'enable';
--rollback SELECT 1 FROM DUAL;