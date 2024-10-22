--liquibase formatted sql

--changeset argent:24_09_18_cdr_add_sip_info
ALTER TABLE kl.`call_detail_record` 
    ADD COLUMN `sip_provider_id` bigint unsigned null default null,
    ADD COLUMN `sip_error_code` bigint unsigned	 null default null;
--rollback SELECT 1 FROM DUAL;
