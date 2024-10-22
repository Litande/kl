--liquibase formatted sql

--changeset argent:user_lead_queue_add_fk
alter table kl.user_lead_queue 
	ADD CONSTRAINT `user_id_FK` foreign key (user_id) references `user` (id),
	ADD CONSTRAINT `lead_queue_id_FK` foreign key (lead_queue_id) references `lead_queue` (id);
--rollback SELECT 1 FROM DUAL;

--changeset argent:lead_add_statuses
alter table kl.lead
	MODIFY COLUMN `status`
	enum('busy','callagainpersonal','callagaingeneral','cannottalk','checknumber',
		'declinecap','declinenotcap','dnc','dnccountry','duplicate','excessivefailedtoconnect',
		'feedbackdefault','inthemoney','languagebarrier','maxcall','movetooldbrands','na',
		'neverregistered','newlead','nomoney','noteligible','notinterested','smallbarrier',
		'systemanswer','systemfailedtoconnect','systemvm','testlead','under18','wrongnumber',
		'processing','inthecall') NOT NULL;
--rollback SELECT 1 FROM DUAL;