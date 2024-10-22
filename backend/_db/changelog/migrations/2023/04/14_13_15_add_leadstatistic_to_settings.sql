--liquibase formatted sql

--changeset argent:14_13_15_add_leadstatistic_to_settings
ALTER TABLE kl.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason', 'rtcconfiguration', 'leadimportdefaultstatus', 'leadstatistic');
--rollback SELECT 1 FROM DUAL;