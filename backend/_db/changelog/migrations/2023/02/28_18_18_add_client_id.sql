--liquibase formatted sql

--changeset argent:28_18_18_add_client_id
ALTER TABLE kl.`lead_queue` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE kl.`group` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE kl.`tag` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE kl.`rule_group` ADD COLUMN `client_id` bigint(20) unsigned NULL;
ALTER TABLE kl.`settings` ADD COLUMN `client_id` bigint(20) unsigned NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_19_add_client_id_value
UPDATE kl.`lead_queue` SET `client_id` = 1;
UPDATE kl.`group` SET `client_id` = 1;
UPDATE kl.`tag` SET `client_id` = 1;
UPDATE kl.`rule_group` SET `client_id` = 1;
UPDATE kl.`settings` SET `client_id` = 1;
UPDATE kl.`call_detail_record` SET `client_id` = 1;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_19_add_client_remove_constraints
ALTER TABLE kl.`call_detail_record` DROP FOREIGN KEY `cdr_client_id_FK`;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_20_add_client_id_not_null
ALTER TABLE kl.`lead_queue` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE kl.`group` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE kl.`tag` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE kl.`rule_group` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE kl.`settings` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
ALTER TABLE kl.`call_detail_record` MODIFY COLUMN `client_id` bigint(20) unsigned NOT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:28_18_21_add_client_id_add_constraints
ALTER TABLE kl.`lead_queue` ADD CONSTRAINT `lead_queue_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE kl.`group` ADD CONSTRAINT `group_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE kl.`tag` ADD CONSTRAINT `tag_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE kl.`rule_group` ADD CONSTRAINT `rule_group_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE kl.`settings` ADD CONSTRAINT `settings_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
ALTER TABLE kl.`call_detail_record` ADD CONSTRAINT `call_detail_record_client_id_FK` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
--rollback SELECT 1 FROM DUAL;
