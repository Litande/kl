--liquibase formatted sql

--changeset argent:20_11_06_add_new_status_cdr
ALTER TABLE dial.`call_detail_record`
    MODIFY COLUMN `call_hangup_status` enum (
        'unknown','callfinishedbylead','callfinishedbyagent',
        'callfinishedbymanager','sessiontimeout','noavailableagents',
        'leadlinebusy','leadinvalidphone','agentnotanswerleadhangup',
        'agentreconnecttimeout','leadnotanswer','agenttimeout',
        'exceededmaxcallduration','siptransporterror',
        'rtctransporttimeout','bridgefailed')
        COLLATE utf8_unicode_ci DEFAULT NULL;
--rollback SELECT 1 FROM DUAL;