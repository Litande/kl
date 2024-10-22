--liquibase formatted sql

--changeset argent:lead_add_system_status
alter table kl.lead
	ADD COLUMN `system_status` enum('processing','inthecall') NULL Default NULL;
--rollback SELECT 1 FROM DUAL;

--changeset argent:lead_remove_system_statuses
alter table kl.lead
	MODIFY COLUMN `status`
	enum('busy','callagainpersonal','callagaingeneral','cannottalk','checknumber',
		'declinecap','declinenotcap','dnc','dnccountry','duplicate','excessivefailedtoconnect',
		'feedbackdefault','inthemoney','languagebarrier','maxcall','movetooldbrands','na',
		'neverregistered','newlead','nomoney','noteligible','notinterested','smallbarrier',
		'systemanswer','systemfailedtoconnect','systemvm','testlead','under18','wrongnumber') NOT NULL;
--rollback SELECT 1 FROM DUAL;