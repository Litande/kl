--liquibase formatted sql

--changeset argent:21_13_27_change_type_in_settings
ALTER TABLE kl.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivitydialer', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason');
UPDATE kl.settings SET type = 'productivedialer' where type = 'productivitydialer';
ALTER TABLE kl.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivedialer', 'droppedcall', 'voicemail', 'callfinishedreason');
--rollback SELECT 1 FROM DUAL;