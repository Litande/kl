--liquibase formatted sql

--changeset argent:20_15_18_add_new_setting_type
ALTER TABLE `dial`.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason', 'rtcconfiguration', 'leadimportdefaultstatus', 'leadstatistic','managerfinishcall');
--rollback SELECT 1 FROM DUAL;