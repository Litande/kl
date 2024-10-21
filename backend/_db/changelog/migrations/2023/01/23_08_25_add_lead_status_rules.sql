--liquibase formatted sql

--changeset argent:lead_status_rules
create table `dial`.lead_status_rules  (
	status					bigint unsigned    not null,
	allow_transit_status    bigint unsigned    not null,
    client_id    			bigint unsigned   not null,
	primary key (status, allow_transit_status, client_id),
	key `client_index` (`client_id`),
	constraint `lead_status_rules_client_id_FK` foreign key (client_id) references `client` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;
