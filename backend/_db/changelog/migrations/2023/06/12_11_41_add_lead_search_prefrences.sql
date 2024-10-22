--liquibase formatted sql

--changeset argent:12_11_41_add_lead_search_prefrences
create table kl.`user_filter_preferences`
(
    id          bigint unsigned auto_increment not null,
    created_by  bigint unsigned                not null,
    filter_name varchar(255)                   not null,
    filter      json                           not null,
    primary key (id),
    constraint `filer_preferences_created_by_FK` foreign key (created_by) references `user` (id)
) ENGINE = InnoDB
  default charset = utf8
  collate = utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;