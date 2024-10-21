--liquibase formatted sql

--changeset argent:05_17_37_add_new_setting_type
ALTER TABLE `dial`.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason', 'rtcconfiguration', 'leadimportdefaultstatus', 'leadstatistic','managerfinishcall', 'callna');
--rollback SELECT 1 FROM DUAL;