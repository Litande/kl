--liquibase formatted sql

--changeset argent:21_16_00_update_call_recording
ALTER TABLE kl.`call_detail_record` MODIFY COLUMN
	  `call_hangup_status`  enum('unknown','callfinishedbylead','callfinishedbyagent',
  'sessiontimeout','noavailableagents','leadlinebusy','leadinvalidphone',
  'agentnotanswerleadhangup','agentreconnecttimeout','leadnotanswer','agenttimeout','siptransporterror',
  'rtctransporttimeout' )    
	COLLATE utf8_unicode_ci DEFAULT NULL;	
--rollback SELECT 1 FROM DUAL;
