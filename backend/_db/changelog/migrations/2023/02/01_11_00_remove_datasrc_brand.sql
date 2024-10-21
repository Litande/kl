--liquibase formatted sql

--changeset argent:01_11_00_remove_datasrc_brand
ALTER TABLE `dial`.data_source DROP COLUMN brand;
--rollback SELECT 1 FROM DUAL;
