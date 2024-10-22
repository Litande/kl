--liquibase formatted sql

--changeset argent:settings
create table kl.settings  (
	id	     bigint unsigned    not null,
    value    json,
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;
