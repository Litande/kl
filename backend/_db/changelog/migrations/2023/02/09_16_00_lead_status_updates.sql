--liquibase formatted sql

--changeset argent:09_16_00_lead_status_updates
ALTER TABLE dial.`lead` MODIFY COLUMN
	`system_status` enum('processing','inthecall','waitingfeedback','postprocessing','imported')
	COLLATE utf8_unicode_ci DEFAULT NULL;	
--rollback SELECT 1 FROM DUAL;