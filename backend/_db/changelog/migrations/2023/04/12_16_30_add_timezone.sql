--liquibase formatted sql

--changeset argent:12_16_30_add_timezone
CREATE TABLE kl.`timezone` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `city_name` varchar(255) NOT NULL,
  `city_name_en` varchar(255) NULL,
  `country_name` varchar(255) NULL,
  `country_code` varchar(255) NOT NULL,
  `timezone` varchar(255) NOT NULL,
  `longitude` double DEFAULT NULL,
  `latitude` double DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=458 DEFAULT CHARSET=utf8;
--rollback SELECT 1 FROM DUAL;