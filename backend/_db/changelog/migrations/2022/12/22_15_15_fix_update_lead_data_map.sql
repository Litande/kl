--liquibase formatted sql

--changeset argent:update_lead_data_source_map
ALTER TABLE dial.lead_data_source_map MODIFY COLUMN destination_property enum('phone','firstName','lastName','languageCode','countryCode','statusId','lastTimeOnline','registrationTime','firstDepositTime','externalId' ) NULL default NULL;
--rollback SELECT 1 FROM DUAL;
