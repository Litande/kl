--liquibase formatted sql

--changeset argent:02_11_25_add_timezone_to_lead_data_source_map
ALTER TABLE kl.lead_data_source_map MODIFY COLUMN destination_property
    enum ('phone','firstName','lastName','languageCode','countryCode',
            'statusId','lastTimeOnline','registrationTime','firstDepositTime',
            'externalId', 'timezone') NULL default NULL;
--rollback SELECT 1 FROM DUAL;