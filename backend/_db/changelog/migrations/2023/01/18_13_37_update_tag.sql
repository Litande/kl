--liquibase formatted sql

--changeset argent:18_13_37_update_user_tag
ALTER TABLE kl.user_tag
	DROP FOREIGN KEY `user_tag_tag_id_FK`,
	ADD COLUMN `expired_on` timestamp NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:18_13_38_update_tag
ALTER TABLE kl.tag
	MODIFY COLUMN `id` bigint unsigned auto_increment NOT NULL,
	MODIFY COLUMN `value` int DEFAULT 0,
	ADD COLUMN `lifetime_seconds` int NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:18_13_37_update_user_tag_constraints
ALTER TABLE kl.user_tag
	ADD CONSTRAINT `user_tag_tag_id_FK` foreign key (tag_id) references `tag` (id);
--rollback SELECT 1 FROM DUAL;
