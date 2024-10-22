--liquibase formatted sql

--changeset argent:13_16_28_redesign_settings
ALTER TABLE kl.settings ADD COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivitydialer', 'droppedcall', 'voicemail', 'callfinishedreason') DEFAULT NULL;
UPDATE kl.settings SET type = CASE id
                                   WHEN 1 THEN 'telephony'
                                   WHEN 2 THEN 'feedback'
                                   WHEN 3 THEN 'agentpermanentleadassignment'
                                   WHEN 4 THEN 'callhours'
                                   WHEN 5 THEN 'productivitydialer'
                                   WHEN 6 THEN 'droppedcall'
                                   WHEN 7 THEN 'voicemail'
                                   WHEN 8 THEN 'callfinishedreason'
                                END;
ALTER TABLE kl.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivitydialer', 'droppedcall', 'voicemail', 'callfinishedreason') NOT NULL;
CREATE INDEX client_id_type_idx ON kl.settings (client_id, type);
ALTER TABLE kl.settings MODIFY COLUMN id INT AUTO_INCREMENT;
--rollback SELECT 1 FROM DUAL;