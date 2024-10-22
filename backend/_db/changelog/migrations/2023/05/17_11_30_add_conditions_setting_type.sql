--liquibase formatted sql

--changeset argent:17_11_30_add_conditions_setting_type
ALTER TABLE kl.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours',
    'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason', 'rtcconfiguration', 'leadimportdefaultstatus',
    'leadstatistic','managerfinishcall', 'callna', 'ruleengineconditions');
--rollback SELECT 1 FROM DUAL;
