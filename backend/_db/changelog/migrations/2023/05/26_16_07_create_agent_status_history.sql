--liquibase formatted sql

--changeset argent:26_16_07_create_agent_status_history
create table kl.`agent_status_history`
(
    id         bigint unsigned auto_increment not null,
    agent_id   bigint unsigned                not null,
    old_status enum('waitingforthecall','inthecall','fillingfeedback','offline','dialing', 'inbreak') not null,
    new_status enum('waitingforthecall','inthecall','fillingfeedback','offline','dialing', 'inbreak') not null,
    initiator  varchar(255)                   not null,
    created_at timestamp                      not null,
    primary key (id),
    constraint `agent_status_agent_id_FK` foreign key (agent_id) references `user` (id)
) ENGINE = InnoDB
  default charset = utf8
  collate = utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;