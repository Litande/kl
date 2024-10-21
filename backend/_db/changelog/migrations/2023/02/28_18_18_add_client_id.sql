--liquibase formatted sql

--changeset argent:28_18_18_add_client_id
ALTER TABLE dial.`lead_queue` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE dial.`group` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE dial.`tag` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE dial.`rule_group` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE dial.`settings` ADD COLUMN `client_id` bigint(20) unsigned NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_19_add_client_id_value
UPDATE dial.`lead_queue` SET `client_id` = 1;
UPDATE dial.`group` SET `client_id` = 1;
UPDATE dial.`tag` SET `client_id` = 1;
UPDATE dial.`rule_group` SET `client_id` = 1;
UPDATE dial.`settings` SET `client_id` = 1;
UPDATE dial.`call_detail_record` SET `client_id` = 1;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_19_add_client_remove_constraints
ALTER TABLE dial.`call_detail_record` DROP FOREIGN KEY `cdr_client_id_FK`;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_20_add_client_id_not_null
ALTER TABLE dial.`lead_queue` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE dial.`group` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE dial.`tag` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE dial.`rule_group` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE dial.`settings` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE dial.`call_detail_record` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_21_add_client_id_add_constraints
ALTER TABLE dial.`lead_queue` ADD CONSTRAINT `lead_queue_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE dial.`group` ADD CONSTRAINT `group_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE dial.`tag` ADD CONSTRAINT `tag_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE dial.`rule_group` ADD CONSTRAINT `rule_group_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE dial.`settings` ADD CONSTRAINT `settings_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE dial.`call_detail_record` ADD CONSTRAINT `call_detail_record_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
--rollback SELECT 1 FROM DUAL;
