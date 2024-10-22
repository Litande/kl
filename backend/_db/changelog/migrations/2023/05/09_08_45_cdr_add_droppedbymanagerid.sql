--liquibase formatted sql

--changeset argent:09_08_45_cdr_add_droppedbymanagerid
ALTER TABLE kl.`call_detail_record` 
    ADD COLUMN `dropped_by_manager_id` bigint unsigned null default null;
--rollback SELECT 1 FROM DUAL;
