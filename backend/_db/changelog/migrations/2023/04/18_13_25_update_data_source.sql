--liquibase formatted sql

--changeset argent:18_13_25_add_query_params_data_source
ALTER TABLE kl.data_source ADD COLUMN `query_params` json NULL DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;