--liquibase formatted sql

--changeset argent:09_18_00_change_rule_types
ALTER TABLE dial.`rule_group` MODIFY COLUMN
	`group_type`  enum('newleads', 'main', 'leadscoring', 'apirules')  not null;	
--rollback SELECT 1 FROM DUAL;
