--liquibase formatted sql

--changeset argent:08_17_23_create_lead_blacklist_table.sql
create table `dial`.lead_blacklist
(
    id          bigint unsigned auto_increment not null,
    client_id   bigint unsigned                not null,
    lead_id     bigint unsigned                not null,
    created_by  bigint unsigned                not null,
    created_at  timestamp                      not null,
    primary key (id),
    constraint `lead_blacklist_client_id_FK` foreign key (client_id) references `client` (id),
    constraint `lead_blacklist_lead_id_FK` foreign key (lead_id) references `lead` (id),
    constraint `lead_blacklist_created_by_FK` foreign key (created_by) references `user` (user_id)
) ENGINE = InnoDB
  default charset = utf8
  collate = utf8_unicode_ci;
--rollback DROP TABLE `dial`.lead_blacklist;