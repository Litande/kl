--liquibase formatted sql

--changeset argent:27_08_20_add_call_recording
create table `dial`.call_detail_record (
  id					bigint unsigned auto_increment			not null,
  session_id          varchar(36)                             not null,
  client_id           bigint unsigned                         null default null,
  brand_name          varchar(200)                            null default null,
  lead_id             bigint unsigned                         null default null,
  lead_name           varchar(200)                            null, 
  lead_phone          varchar(50)                             not null, 
  lead_country        varchar(200)                            null default null, 
  call_type           enum('manual', 'predictive')            not null,
  call_hangup_status  enum('unknown','callfinishedbylead','callfinishedbyagent',
  'sessiontimeout','noavailableagents','leadlinebusy','leadinvalidphone',
  'agentnotanswerleadhangup','agentreconnecttimeout','siptransporterror',
  'rtctransporttimeout' )                                  null default null,
  queue_id            bigint unsigned                         null,
  queue_name          varchar(200)                            null,
  user_id             bigint unsigned                         null default null,
  user_name           varchar(200)                            null default null,
  started_at          timestamp                               null,
  call_hangup_at      timestamp                               null,
  lead_answer_at      timestamp                               null,
  user_answer_at      timestamp                               null,
  lead_status_after enum('busy','callagainpersonal','callagaingeneral','cannottalk','checknumber',
  'declinecap','declinenotcap','dnc','dnccountry','duplicate','excessivefailedtoconnect',
  'feedbackdefault','inthemoney','languagebarrier','maxcall','movetooldbrands','na',
  'neverregistered','newlead','nomoney','noteligible','notinterested','smallbarrier',
  'systemanswer','systemfailedtoconnect','systemvm','testlead','under18','wrongnumber') 	null default null,
  lead_status_before enum('busy','callagainpersonal','callagaingeneral','cannottalk','checknumber',
  'declinecap','declinenotcap','dnc','dnccountry','duplicate','excessivefailedtoconnect',
  'feedbackdefault','inthemoney','languagebarrier','maxcall','movetooldbrands','na',
  'neverregistered','newlead','nomoney','noteligible','notinterested','smallbarrier',
  'systemanswer','systemfailedtoconnect','systemvm','testlead','under18','wrongnumber')   null default null,
  caller_id           varchar(200)                            null,
  record_user_files    text,
  record_lead_file    text,
  record_manager_files text,
  record_mixed_file   text,
  is_replaced_user    tinyint(1)                              not null default 0,
  metadata            json                                    null,

  primary key (id),
  unique key `SessionIndex` (`session_id`),

  constraint `cdr_client_id_FK` foreign key (client_id) references `client` (id) on delete set null,
  constraint `cdr_lead_id_FK` foreign key (lead_id) references `lead` (id) on delete set null,
  constraint `cdr_user_id_FK` foreign key (user_id) references `user` (user_id) on delete set null,
  constraint `cdr_queue_id_FK` foreign key (queue_id) references `lead_queue` (id) on delete set null

) ENGINE=InnoDB default charset=utf8 collate=utf8_unicode_ci;
--rollback SELECT 1 FROM DUAL;
