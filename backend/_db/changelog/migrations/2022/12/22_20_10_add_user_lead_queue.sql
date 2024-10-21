--liquibase formatted sql

--changeset argent:user_lead_queue
create table `dial`.user_lead_queue (
	user_id					 bigint unsigned							not null,
	lead_queue_id			 bigint unsigned							not null,
	primary key (user_id, lead_queue_id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;
