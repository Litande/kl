--liquibase formatted sql

--changeset argent:21_12_18_add_order_to_lead_queue
ALTER TABLE kl.`lead_queue` ADD COLUMN `display_order` INT NOT NULL DEFAULT 0;
--rollback SELECT 1 FROM DUAL;