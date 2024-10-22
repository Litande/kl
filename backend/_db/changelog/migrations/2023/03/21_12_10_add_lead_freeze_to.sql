--liquibase formatted sql

--changeset argent:21_12_10_add_lead_freeze_to
ALTER TABLE kl.`lead` DROP FOREIGN KEY lead_user_id_FK;

ALTER TABLE kl.`lead` CHANGE `user_id` `last_call_user_id` bigint(20) unsigned NULL;
ALTER TABLE kl.`lead` ADD COLUMN `freeze_to` timestamp NULL DEFAULT NULL;

ALTER TABLE kl.`lead` ADD CONSTRAINT `lead_user_id_FK` FOREIGN KEY (`last_call_user_id`) REFERENCES `user` (id);
--rollback SELECT 1 FROM DUAL;