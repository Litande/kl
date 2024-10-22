--liquibase formatted sql

--changeset argent:24_13_15_add_new_type_setting
ALTER TABLE kl.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason', 'rtcconfiguration');
--rollback SELECT 1 FROM DUAL;