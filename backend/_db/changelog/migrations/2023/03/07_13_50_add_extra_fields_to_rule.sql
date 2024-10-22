--liquibase formatted sql

--changeset argent:07_13_50_add_queue_id_to_rule
ALTER TABLE kl.rule ADD COLUMN `queue_id` BIGINT UNSIGNED NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:07_13_55_add_ordinal_to_rule
ALTER TABLE kl.rule ADD COLUMN `ordinal` int NOT NULL DEFAULT 0;
--rollback SELECT 1 FROM DUAL;
