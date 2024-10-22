--liquibase formatted sql

--changeset argent:07_14_40_modify_group_type
ALTER TABLE kl.rule_group MODIFY COLUMN
	`group_type` enum('newleads','main','apirules','forwardrules','behavior','leadscoring')
	COLLATE utf8_unicode_ci NOT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:07_14_40_set_group_type
Update kl.rule_group set `group_type` = case `group_type`
          when 'newleads' then 'forwardrules'
		  when 'main' then 'behavior'
		  when 'apirules' then 'apirules'
		  when 'leadscoring' then 'leadscoring'
		  end
--rollback SELECT 1 FROM DUAL;

--changeset argent:07_14_40_modify_new_group_type
ALTER TABLE kl.rule_group MODIFY COLUMN
	`group_type` enum('apirules','forwardrules','behavior','leadscoring')
	COLLATE utf8_unicode_ci NOT NULL;
--rollback SELECT 1 FROM DUAL;