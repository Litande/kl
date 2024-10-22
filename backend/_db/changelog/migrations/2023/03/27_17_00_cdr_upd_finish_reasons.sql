--liquibase formatted sql

--changeset argent:27_17_00_cdr_upd_finish_reasons
ALTER TABLE kl.`call_detail_record` MODIFY COLUMN
	  `call_hangup_status`  enum('unknown','callfinishedbylead','callfinishedbyagent',
  'sessiontimeout','noavailableagents','leadlinebusy','leadinvalidphone',
  'agentnotanswerleadhangup','agentreconnecttimeout','leadnotanswer','agenttimeout','exceededmaxcallduration','siptransporterror',
  'rtctransporttimeout', 'bridgefailed' )    
	COLLATE utf8_unicode_ci DEFAULT NULL;	
--rollback SELECT 1 FROM DUAL;
