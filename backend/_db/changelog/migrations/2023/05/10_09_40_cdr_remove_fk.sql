--liquibase formatted sql

--changeset argent:10_09_40_cdr_remove_fk
ALTER TABLE kl.`call_detail_record` 
    DROP FOREIGN KEY `call_detail_record_client_id_FK`,
    DROP FOREIGN KEY `cdr_lead_id_FK`,
    DROP FOREIGN KEY `cdr_queue_id_FK`,
    DROP FOREIGN KEY `cdr_user_id_FK`;
--rollback SELECT 1 FROM DUAL;
