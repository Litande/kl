--liquibase formatted sql

--changeset argent:20_16_41_lead_status_updates
ALTER TABLE kl.`lead` MODIFY COLUMN
	`system_status` enum('processing','dialing','inthecall','waitingfeedback','postprocessing','imported')
	COLLATE utf8_unicode_ci DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;