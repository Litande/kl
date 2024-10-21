--liquibase formatted sql

--changeset argent:12_11_48_add_metadata_datasource
ALTER TABLE dial.`data_source`
    ADD COLUMN `metadata` json null;
--rollback SELECT 1 FROM DUAL;