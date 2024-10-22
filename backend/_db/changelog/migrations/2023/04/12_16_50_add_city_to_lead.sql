--liquibase formatted sql

--changeset argent:12_16_50_add_city_to_lead
ALTER TABLE kl.`lead` ADD COLUMN `city` varchar(50) NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;