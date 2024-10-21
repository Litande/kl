--liquibase formatted sql

--changeset argent:25_13_07_cleanup_users_tables
DROP TABLE `dial`.user_claims;
DROP TABLE `dial`.user_logins;
DROP TABLE `dial`.user_roles;
DROP TABLE `dial`.user_tokens;
DROP TABLE `dial`.users;
--rollback SELECT 1 FROM DUAL;

--changeset argent:25_13_07_drop_user_constraints
ALTER TABLE `dial`.lead DROP FOREIGN KEY `lead_user_id_FK`;
ALTER TABLE `dial`.user_tag DROP FOREIGN KEY `user_tag_user_id_FK`;
ALTER TABLE `dial`.user_lead_queue DROP FOREIGN KEY `user_id_FK`;
ALTER TABLE `dial`.user_group DROP FOREIGN KEY `user_group_user_id_FK`;
ALTER TABLE `dial`.user_data_source_map DROP FOREIGN KEY `user_data_source_map_user_id_FK`;
--rollback SELECT 1 FROM DUAL;

--changeset argent:25_13_08_drop_user_constraints2
ALTER TABLE `dial`.lead_history DROP FOREIGN KEY `lead_history_created_by_FK`;
--rollback SELECT 1 FROM DUAL;

--changeset argent:25_13_09_add_user_id_fk
ALTER TABLE `dial`.user CHANGE id user_id bigint(20) unsigned NOT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:25_13_10_drop_user_id
ALTER TABLE `dial`.user DROP COLUMN username;
ALTER TABLE `dial`.user DROP COLUMN password;
ALTER TABLE `dial`.user DROP COLUMN remember_token;
ALTER TABLE `dial`.user DROP COLUMN password_last_updated_at;
ALTER TABLE `dial`.user DROP COLUMN enable_two_fa;
--rollback SELECT 1 FROM DUAL;

--changeset argent:25_13_11_add_user_constraints
ALTER TABLE `dial`.user ADD CONSTRAINT `user_id_asp_net_users_id_FK` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`);
ALTER TABLE `dial`.lead ADD CONSTRAINT `lead_user_id_FK` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);
ALTER TABLE `dial`.user_tag ADD CONSTRAINT `user_tag_user_id_FK` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);
ALTER TABLE `dial`.user_lead_queue ADD CONSTRAINT `user_lead_queue_user_id_FK` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);
ALTER TABLE `dial`.user_group ADD CONSTRAINT `user_group_user_id_FK` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);
ALTER TABLE `dial`.user_data_source_map ADD CONSTRAINT `user_data_source_map_user_id_FK` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);
ALTER TABLE `dial`.lead_history ADD CONSTRAINT `lead_history_created_by_FK` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`);
--rollback SELECT 1 FROM DUAL;