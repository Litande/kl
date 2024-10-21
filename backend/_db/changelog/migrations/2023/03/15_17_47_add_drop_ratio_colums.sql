--liquibase formatted sql

--changeset argent:15_17_47_add_drop_ratio_colums
ALTER TABLE dial.`lead_queue` ADD COLUMN `drop_rate_upper_threshold` DOUBLE NULL DEFAULT NULL,
    ADD COLUMN `drop_rate_lower_threshold` DOUBLE NULL DEFAULT NULL,
    ADD COLUMN `drop_rate_period` INT DEFAULT 0,
    ADD COLUMN `ratio_step` DOUBLE DEFAULT 0,
    ADD COLUMN `ratio_freeze_time` INT DEFAULT 0;
--rollback SELECT 1 FROM DUAL;