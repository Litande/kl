--liquibase formatted sql

--changeset argent:24_10_30_add_sip_provider
create table kl.`sip_provider` (
	id					bigint unsigned auto_increment			not null,
	provider_name varchar(100) not null,
    provider_address varchar(100) not null,
    provider_username varchar(100) not null,
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;
