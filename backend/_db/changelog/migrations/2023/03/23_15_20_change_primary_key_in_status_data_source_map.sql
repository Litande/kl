--liquibase formatted sql

--changeset argent:23_15_20_change_primary_key_in_status_data_source_map
ALTER TABLE dial.status_data_source_map ADD COLUMN `id` bigint(20) NOT NULL;
ALTER TABLE dial.status_data_source_map DROP FOREIGN KEY `status_data_source_map_data_source_id_FK`;
ALTER TABLE dial.status_data_source_map DROP PRIMARY KEY, ADD PRIMARY KEY (`id`);
ALTER TABLE dial.status_data_source_map ADD FOREIGN KEY (data_source_id) REFERENCES dial.`data_source` (id);
ALTER TABLE dial.status_data_source_map MODIFY `id` bigint(20) AUTO_INCREMENT;
--rollback SELECT 1 FROM DUAL;