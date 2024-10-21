--liquibase formatted sql

--changeset argent:lead_add_system_status
alter table `dial`.data_source
	ADD COLUMN `brand` varchar(200) NULL Default NULL;
--rollback SELECT 1 FROM DUAL;
