--liquibase formatted sql

--changeset argent:27_15_10_add_lead_import_default_status
ALTER TABLE `dial`.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason', 'rtcconfiguration', 'leadimportdefaultstatus');
--rollback SELECT 1 FROM DUAL;