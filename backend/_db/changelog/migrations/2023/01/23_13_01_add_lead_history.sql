--liquibase formatted sql

--changeset argent:lead_history
create table `dial`.lead_history (
	id					bigint unsigned auto_increment			not null,
	lead_id			    bigint unsigned							not null,
    action_type         enum('status','data')                   not null,
    changes             json                                    not null,
    created_at          timestamp                               not null,
    created_by          bigint unsigned                         null,
	primary key (id),
    constraint `lead_history_created_by_FK` foreign key (created_by) references `user` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:add_lead_history_index
alter table `dial`.lead_history add index action_type_IDX (action_type);
--rollback SELECT 1 FROM DUAL;