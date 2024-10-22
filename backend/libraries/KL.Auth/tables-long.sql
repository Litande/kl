CREATE TABLE asp_net_users (
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
PRIMARY KEY (`id`),
UNIQUE KEY `user_name_index` (`normalized_user_name`),
KEY `email_index` (`normalized_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE asp_net_user_claims (
`id` int(11) NOT NULL AUTO_INCREMENT,
`user_id` BIGINT UNSIGNED  NOT NULL,
`claim_type` text,
`claim_value` text,
PRIMARY KEY (`Id`),
KEY `IX_user_claims_user_id` (`user_id`),
CONSTRAINT `FK_user_claims_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE asp_net_user_logins (
`login_provider` varchar(256) NOT NULL,
`provider_key` varchar(256) NOT NULL,
`provider_display_name` text,
`user_id` BIGINT UNSIGNED  NOT NULL,
PRIMARY KEY (`login_provider`,`provider_key`),
KEY `IX_user_logins_user_id` (`user_id`),
CONSTRAINT `FK_user_logins_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE asp_net_user_tokens (
`user_id` BIGINT UNSIGNED  NOT NULL,
`login_provider` varchar(256) NOT NULL,
`name` varchar(256) NOT NULL,
`value` text,
PRIMARY KEY (`user_id`,`login_provider`,`name`),
CONSTRAINT `FK_user_tokens_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



CREATE TABLE asp_net_roles (
`id` BIGINT UNSIGNED AUTO_INCREMENT NOT NULL,
`name` varchar(256) DEFAULT NULL,
`normalized_name` varchar(256) DEFAULT NULL,
`concurrency_stamp` text,
PRIMARY KEY (`id`),
UNIQUE KEY `role_name_index` (`normalized_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE asp_net_role_claims (
`id` int(11) NOT NULL AUTO_INCREMENT,
`role_id` BIGINT UNSIGNED NOT NULL,
`claim_type` text,
`claim_value` text,
PRIMARY KEY (`Id`),
KEY `IX_role_claims_role_id` (`role_id`),
CONSTRAINT `FK_role_claims_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `asp_net_roles` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=306 DEFAULT CHARSET=utf8;



CREATE TABLE asp_net_user_roles (
`user_id` BIGINT UNSIGNED NOT NULL,
`role_id` BIGINT UNSIGNED NOT NULL,
PRIMARY KEY (`user_id`,`role_id`),
KEY `IX_user_roles_role_id` (`role_id`),
CONSTRAINT `FK_user_roles_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `asp_net_roles` (`id`) ON DELETE CASCADE,
CONSTRAINT `FK_user_roles_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

