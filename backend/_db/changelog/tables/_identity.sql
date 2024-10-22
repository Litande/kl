--liquibase formatted sql

--changeset argent:create_table_client
create table kl.client (
id					 	bigint unsigned	auto_increment			 not null,
primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:_identity
-- kl.roles definition
CREATE TABLE kl.role (
`id` BIGINT UNSIGNED AUTO_INCREMENT NOT NULL,
`name` varchar(256) DEFAULT NULL,
`normalized_name` varchar(256) DEFAULT NULL,
`concurrency_stamp` text,
PRIMARY KEY (`id`),
UNIQUE KEY `role_name_index` (`normalized_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- kl.users definition
CREATE TABLE kl.user (
`id` BIGINT UNSIGNED AUTO_INCREMENT NOT NULL,
`user_name` varchar(256) DEFAULT NULL,
`normalized_user_name` varchar(256) DEFAULT NULL,
`email` varchar(256) DEFAULT NULL,
`normalized_email` varchar(256) DEFAULT NULL,
`email_confirmed` bit(1) NOT NULL,
`password_hash` text,
`security_stamp` text,
`concurrency_stamp` text,
`phone_number` text,
`phone_number_confirmed` bit(1) NOT NULL,
`two_factor_enabled` bit(1) NOT NULL,
`lockout_end` timestamp NULL DEFAULT NULL,
`lockout_enabled` bit(1) NOT NULL,
`access_failed_count` int(11) NOT NULL,
client_id                bigint unsigned						 not null,
role					 enum('agent','manager')				 not null,
first_name               varchar(191)							 not null,
last_name                varchar(191)							 not null,
status					 enum('active','suspended','deleted')	 default 'active',
timezone				 varchar(50)							 default null,
last_agent_status		 enum('WaitingForTheCall','InTheCall','FillingFeedback','Offline','Dialing') null default null,
last_agent_status_updated_at timestamp null						 default null,
created_at               timestamp								 not null default current_timestamp,
deleted_at               timestamp null							 default null,
PRIMARY KEY (`id`),
UNIQUE KEY `user_name_index` (`normalized_user_name`),
KEY `email_index` (`normalized_email`),
index users_id_deleted_IDX (id, deleted_at),
constraint `FK_user_client_id` foreign key (client_id) references kl.`client` (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- kl.role_claims definition
CREATE TABLE kl.role_claim (
  `id` BIGINT UNSIGNED AUTO_INCREMENT NOT NULL,
  `role_id` BIGINT UNSIGNED NOT NULL,
  `claim_type` text,
  `claim_value` text,
  PRIMARY KEY (`Id`),
  KEY `IX_role_claim_role_id` (`role_id`),
  CONSTRAINT `FK_role_claim_role_id` FOREIGN KEY (`role_id`) REFERENCES kl.`role` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=306 DEFAULT CHARSET=utf8;

-- kl.user_claims definition
CREATE TABLE kl.user_claim (
  `id` BIGINT UNSIGNED AUTO_INCREMENT NOT NULL,
  `user_id`  BIGINT UNSIGNED NOT NULL,
  `claim_type` text,
  `claim_value` text,
  PRIMARY KEY (`Id`),
  KEY `IX_user_claim_user_id` (`user_id`),
  CONSTRAINT `FK_user_claim_user_id` FOREIGN KEY (`user_id`) REFERENCES kl.`user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- kl.user_logins definition
CREATE TABLE kl.user_login (
`login_provider` varchar(256) NOT NULL,
`provider_key` varchar(256) NOT NULL,
`provider_display_name` text,
`user_id` BIGINT UNSIGNED NOT NULL,
PRIMARY KEY (`login_provider`,`provider_key`),
KEY `IX_user_login_user_id` (`user_id`),
CONSTRAINT `FK_user_login_user_id` FOREIGN KEY (`user_id`) REFERENCES kl.`user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- kl.user_role definition
CREATE TABLE kl.user_role (
`user_id` BIGINT UNSIGNED NOT NULL,
`role_id` BIGINT UNSIGNED NOT NULL,
PRIMARY KEY (`user_id`,`role_id`),
KEY `IX_user_role_role_id` (`role_id`),
CONSTRAINT `FK_user_role_role_id` FOREIGN KEY (`role_id`) REFERENCES kl.`role` (`id`) ON DELETE CASCADE,
CONSTRAINT `FK_user_role_user_id` FOREIGN KEY (`user_id`) REFERENCES kl.`user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- kl.user_token definition
CREATE TABLE kl.user_token (
`user_id` BIGINT UNSIGNED NOT NULL,
`login_provider` varchar(256) NOT NULL,
`name` varchar(256) NOT NULL,
`value` text,
PRIMARY KEY (`user_id`,`login_provider`,`name`),
CONSTRAINT `FK_user_token_user_id` FOREIGN KEY (`user_id`) REFERENCES kl.`user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
