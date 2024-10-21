--liquibase formatted sql

--changeset argent:15_10_43_create_lead_comment_table
create table `dial`.`lead_comment`
(
    id         bigint unsigned auto_increment not null,
    lead_id    bigint unsigned                not null,
    comment    varchar(255)                   not null,
    created_by bigint unsigned                not null,
    created_at timestamp                      not null,
    primary key (id),
    constraint `lead_comment_lead_id_FK` foreign key (lead_id) references `lead` (id),
    constraint `lead_comment_created_by_FK` foreign key (created_by) references `user` (user_id)
) ENGINE = InnoDB
  default charset = utf8
  collate = utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;