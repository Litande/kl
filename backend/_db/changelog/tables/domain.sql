--liquibase formatted sql

--changeset argent:create_table_client
create table `dial`.client (
	id					 	bigint unsigned	auto_increment			 not null,
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_user
create table `dial`.user (
    id                       bigint unsigned auto_increment			 not null,
	client_id                bigint unsigned						 not null,
	role					 enum('agent','manager')				 not null,
	first_name               varchar(191)							 not null,
    last_name                varchar(191)							 not null,
	username                 varchar(64)                             default null,
    password                 varchar(191)							 not null,
	remember_token           varchar(100)							 default null,
	status					 enum('active','suspended','deleted')	 default 'active',
	created_at               timestamp								 not null,
	password_last_updated_at datetime                                null,
	deleted_at               timestamp null							 default null,
	timezone				 varchar(50)							 default null,
	enable_two_fa            tinyint(1)								 default 0,
	last_agent_status		 enum('WaitingForTheCall','InTheCall',
	'FillingFeedback','Offline','Dialing') null						 default null,
	last_agent_status_updated_at timestamp null						 default null,	
	primary key (id),
	constraint `user_client_id_FK` foreign key (client_id) references `client` (id),
	index users_id_deleted_IDX (id, deleted_at)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_data_source
create table `dial`.data_source (
	id                       bigint unsigned auto_increment			 not null,
	client_id                bigint unsigned 						 not null,
	name					 varchar(100)							 not null,
	api_key					 varchar(200)							 not null,
	endpoint				 varchar(200)							 not null,
	source_type				 enum('lead','user')				 	 not null,
	status					 enum('enable','disable')				 not null default 'enable',
	iframe_template			 varchar(200)							 default null,
	min_update_date			 timestamp null							 default null,
	primary key (id),
	constraint `data_source_client_id_FK` foreign key (client_id) references `client` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_group
create table `dial`.group (
	id	                	 bigint unsigned 						 not null,
	name				 	 varchar(200)							 not null,
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_user_group
create table `dial`.user_group (
	user_id                	 bigint unsigned 						 not null,
	group_id				 bigint unsigned						 not null,
	primary key (user_id, group_id),
	constraint `user_group_user_id_FK` foreign key (user_id) references `user` (id),
	constraint `user_group_group_id_FK` foreign key (group_id) references `group` (id)	
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_tag
create table `dial`.tag (
	id	                	 bigint unsigned 						 not null,
	name				 	 varchar(200)							 not null,
	value				 	 varchar(200)							 not null,
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_user_tag
create table `dial`.user_tag (
	user_id                	 bigint unsigned 						 not null,
	tag_id				 	 bigint unsigned						 not null,
	primary key (user_id, tag_id),
	constraint `user_tag_user_id_FK` foreign key (user_id) references `user` (id),
	constraint `user_tag_tag_id_FK` foreign key (tag_id) references `tag` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_user_data_source_map
create table `dial`.user_data_source_map (
	user_id                	 bigint unsigned 						 not null,
	data_source_id			 bigint unsigned						 not null,
	employee_id			 	 bigint unsigned						 not null,
	primary key (user_id, data_source_id),
	constraint `user_data_source_map_user_id_FK` foreign key (user_id) references `user` (id),
	constraint `user_data_source_map_data_source_id_FK` foreign key (data_source_id) references `data_source` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_status_data_source_map
create table `dial`.status_data_source_map (
	data_source_id			 bigint unsigned						 not null,
	status					 enum('busy','callagainpersonal',
	'callagaingeneral','cannottalk','checknumber','declinecap',
	'declinenotcap','dnc','dnccountry','duplicate',
	'excessivefailedtoconnect','feedbackdefault','inthemoney',
	'languagebarrier','maxcall','movetooldbrands','na',
	'neverregistered','newlead','nomoney','noteligible',
	'notinterested','smallbarrier','systemanswer',
	'systemfailedtoconnect','systemvm','testlead','under18',
	'wrongnumber') null						 						 default null,
	external_status_id		 varchar(50)							 not null,
	primary key (data_source_id),
	constraint `status_data_source_map_data_source_id_FK` foreign key (data_source_id) references `data_source` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_lead_data_source_map
create table `dial`.lead_data_source_map (
	id                       bigint unsigned auto_increment			 not null,
	data_source_id			 bigint unsigned						 not null,
	destination_property	 enum('phone','firstName','lastName',
	'languageCode','countryCode','statusId','lastTimeOnline',
	'registrationTime','firstDepositTime') null						 default null,
	source_property			 varchar(250)							 not null,
	primary key (id),
	constraint `lead_data_source_data_source_id_FK` foreign key (data_source_id) references `data_source` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_lead
create table `dial`.lead (
	id                       bigint unsigned auto_increment			 not null,
	client_id                bigint unsigned 						 not null,
	data_source_id			 bigint unsigned						 not null,
	duplicate_of_id			 bigint unsigned						 default null,
	user_id					 bigint unsigned 						 default null,
	phone					 varchar(50)							 not null,
	external_id				 varchar(191)							 default null,
	first_name               varchar(200)							 not null,
    last_name                varchar(200)							 not null,
	language_code			 varchar(191)							 default null,
	country_code			 varchar(191)							 default null,
	status					 enum('busy','callagainpersonal',
	'callagaingeneral','cannottalk','checknumber','declinecap',
	'declinenotcap','dnc','dnccountry','duplicate',
	'excessivefailedtoconnect','feedbackdefault','inthemoney',
	'languagebarrier','maxcall','movetooldbrands','na',
	'neverregistered','newlead','nomoney','noteligible',
	'notinterested','smallbarrier','systemanswer',
	'systemfailedtoconnect','systemvm','testlead','under18',
	'wrongnumber')							 		 				 not null,
	registration_time        timestamp								 not null,
	last_time_online		 timestamp null							 default null,
	last_update_time		 timestamp null							 default null,
	is_fixed_assigned		 tinyint(1)								 not null default 0,
	first_deposit_time		 timestamp null							 default null,
	remind_on				 timestamp null							 default null,
	primary key (id),
	constraint `lead_client_id_FK` foreign key (client_id) references `client` (id),
	constraint `lead_data_source_id_FK` foreign key (data_source_id) references `data_source` (id),
	constraint `lead_duplicate_of_id_FK` foreign key (duplicate_of_id) references `lead` (id),
	constraint `lead_user_id_FK` foreign key (user_id) references `user` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_lead_index
create index data_source_id_external_id_index on `dial`.lead (data_source_id,external_id);
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_rule_group
create table `dial`.rule_group (
	id               	 	 bigint unsigned auto_increment 		 not null,
	name					 varchar(200)							 not null,
	status			 		 enum('enable','disable')				 not null default 'enable',
	group_type               enum('newleads','main','leadscoring',
	                              'agentscoring','status',
								  'leadspreview')                    not null,
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_rule
create table `dial`.rule (
	id               	 	 bigint unsigned auto_increment 		 not null,
	rule_group_id		     bigint unsigned					     not null,
	name					 varchar(200)							 not null,
	status			 		 enum('enable','disable')				 not null default 'enable',
	rules			 		 json									 not null,
	primary key (id),
	constraint `rule_rule_group_id_FK` foreign key (rule_group_id) references `rule_group` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_lead_queue
create table `dial`.lead_queue (
	id               	 	 bigint unsigned auto_increment			 not null,
	name					 varchar(200)							 not null,
	status			 		 enum('enable','disable')				 not null default 'enable',
	primary key (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_forward_lead_queue_rule
create table `dial`.forward_lead_queue_rule (
	queue_id               	 bigint unsigned						 not null,
	rule_id					 bigint unsigned						 not null,
	ordinal			 		 bigint unsigned						 not null default 0,
	primary key (queue_id, rule_id),
	constraint `forward_lead_queue_rule_queue_id_FK` foreign key (queue_id) references `lead_queue` (id),
	constraint `forward_lead_queue_rule_rule_id_FK` foreign key (rule_id) references `rule` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_score_lead_rule
create table `dial`.score_lead_rule (
	id               	 	 bigint unsigned auto_increment			 not null,
	queue_id               	 bigint unsigned null					 default null,
	rule_id					 bigint unsigned						 not null,
	ordinal			 		 bigint unsigned						 not null default 0,
	primary key (id),
	constraint `score_lead_rule_queue_id_FK` foreign key (queue_id) references `lead_queue` (id),
	constraint `score_lead_rule_rule_id_FK` foreign key (rule_id) references `rule` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;

--changeset argent:create_table_behaviour_lead_rule
create table `dial`.behaviour_lead_rule (
	id               	 	 bigint unsigned auto_increment			 not null,
	queue_id               	 bigint unsigned null					 default null,
	rule_id					 bigint unsigned						 not null,
	ordinal			 		 bigint unsigned						 not null default 0,
	primary key (id),
	constraint `behaviour_lead_rule_queue_id_FK` foreign key (queue_id) references `lead_queue` (id),
	constraint `behaviour_lead_rule_rule_id_FK` foreign key (rule_id) references `rule` (id)
) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;