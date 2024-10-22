--liquibase formatted sql

--changeset argent:05_17_40_add_callback_endpoints_to_datasource
ALTER TABLE kl.data_source ADD COLUMN callback_endpoints JSON NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;