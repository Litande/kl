--liquibase formatted sql

--changeset argent:update_lead_data_source_map
ALTER TABLE kl.lead_data_source_map MODIFY COLUMN destination_property enum('phone','firstName','lastName','languageCode','countryCode','statusId','lastTimeOnline','registrationTime','firstDepositTime','externalId' ) CHARACTER SET utf8 COLLATE utf8_unicode_ci NULL;
--rollback SELECT 1 FROM DUAL;
