--liquibase formatted sql

--changeset argent:13_16_28_redesign_settings
ALTER TABLE `dial`.settings ADD COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivitydialer', 'droppedcall', 'voicemail', 'callfinishedreason') DEFAULT NULL;
UPDATE `dial`.settings SET type = CASE id
                                   WHEN 1 THEN 'telephony'
                                   WHEN 2 THEN 'feedback'
                                   WHEN 3 THEN 'agentpermanentleadassignment'
                                   WHEN 4 THEN 'callhours'
                                   WHEN 5 THEN 'productivitydialer'
                                   WHEN 6 THEN 'droppedcall'
                                   WHEN 7 THEN 'voicemail'
                                   WHEN 8 THEN 'callfinishedreason'
                                END;
ALTER TABLE `dial`.settings MODIFY COLUMN type ENUM('telephony', 'feedback', 'agentpermanentleadassignment', 'callhours', 'productivitydialer', 'droppedcall', 'voicemail', 'callfinishedreason') NOT NULL;
CREATE INDEX client_id_type_idx ON `dial`.settings (client_id, type);
ALTER TABLE `dial`.settings MODIFY COLUMN id INT AUTO_INCREMENT;
--rollback SELECT 1 FROM DUAL;