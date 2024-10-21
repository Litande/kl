--liquibase formatted sql

--changeset argent:07_11_50_remove_behaviour_lead_rule_table
DROP TABLE dial.`behaviour_lead_rule`;
--rollback CREATE TABLE dial.behaviour_lead_rule (id bigint unsigned auto_increment	not null, queue_id  bigint unsigned null default null, rule_id	bigint unsigned	not null, ordinal bigint unsigned not null default 0, primary key (id),	constraint `behaviour_lead_rule_queue_id_FK` foreign key (queue_id) references `lead_queue` (id), constraint `behaviour_lead_rule_rule_id_FK` foreign key (rule_id) references `rule` (id));

--changeset argent:07_11_55_remove_score_lead_rule_table
DROP TABLE dial.`score_lead_rule`;
--rollback CREATE TABLE dial.score_lead_rule (id bigint unsigned auto_increment	not null, queue_id bigint unsigned null	default null, rule_id bigint unsigned not null,	ordinal	bigint unsigned	not null default 0,	primary key (id), constraint `score_lead_rule_queue_id_FK` foreign key (queue_id) references `lead_queue` (id),	constraint `score_lead_rule_rule_id_FK` foreign key (rule_id) references `rule` (id));

--changeset argent:07_11_55_remove_forward_lead_queue_rule_table
DROP TABLE dial.`forward_lead_queue_rule`;
--rollback CREATE TABLE dial.forward_lead_queue_rule (queue_id bigint unsigned not null, rule_id bigint unsigned not null,	ordinal	bigint unsigned	not null default 0,	primary key (queue_id, rule_id), constraint `forward_lead_queue_rule_queue_id_FK` foreign key (queue_id) references `lead_queue` (id),	constraint `forward_lead_queue_rule_rule_id_FK` foreign key (rule_id) references `rule` (id));