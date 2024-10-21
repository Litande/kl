--liquibase formatted sql

--changeset argent:07_14_10_add_queue_fk
ALTER TABLE dial.rule ADD CONSTRAINT queue_id_FK FOREIGN KEY (queue_id) REFERENCES dial.lead_queue(id);
--rollback SELECT 1 FROM DUAL;