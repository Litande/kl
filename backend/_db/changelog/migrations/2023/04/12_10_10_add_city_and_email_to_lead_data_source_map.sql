--liquibase formatted sql

--changeset argent:12_10_05_add_city_and_email_to_lead_data_source_map
ALTER TABLE kl.lead_data_source_map MODIFY COLUMN destination_property
    enum ('phone','firstName','lastName','languageCode','countryCode',
            'statusId','lastTimeOnline','registrationTime','firstDepositTime',
            'externalId', 'timezone', 'city', 'email') NULL default NULL;
--rollback SELECT 1 FROM DUAL;