
resource "random_id" "db_name_suffix" {
  byte_length = 4
}


resource "google_compute_global_address" "db_private_ip_address" {
  name          = format("%s-db-ip-%s",  var.project_name, random_id.db_name_suffix.hex)
  purpose       = "VPC_PEERING"
  address_type  = "INTERNAL"
  prefix_length = 16
  network       = var.network.self_link
}


resource "google_service_networking_connection" "private_vpc_connection" {
  provider = google-beta

  network                 = var.network.self_link
  service                 = "servicenetworking.googleapis.com"
  reserved_peering_ranges = [google_compute_global_address.db_private_ip_address.name]
}


resource "google_sql_database_instance" "database" {
  depends_on = [google_service_networking_connection.private_vpc_connection]

  name             = format("%s-db-%s", var.project_name, random_id.db_name_suffix.hex)
  database_version = "MYSQL_5_7"
  region           = "europe-west1"

  deletion_protection = var.db_deletion_protection_enabled
  settings {
    tier              = var.db_machine_type
    activation_policy = "ALWAYS"

    user_labels = {
      ref_project      = var.project_name
      role             = var.network.name
      replication_role = "main"
    }

    database_flags {
      name  = "group_concat_max_len"
      value = 2048 # was 2048 # For prod 4294967295
    }
    database_flags {
      name  = "log_bin_trust_function_creators"
      value = "on"
    }
    database_flags {
      name  = "event_scheduler"
      value = "on"
    }
    database_flags {
      name  = "max_binlog_size"
      value = "536870912"
    }
    database_flags {
      name  = "binlog_row_image"
      value = "minimal"
    }

    database_flags {
      name  = "slow_query_log"
      value = "on"
    }

    database_flags {
      name  = "log_output"
      value = "FILE"
    }

    database_flags {
      name  = "long_query_time"
      value = 10
    }

    database_flags {
      name  = "table_open_cache"
      value = "8000"
    }

    database_flags {
      name  = "performance_schema"
      value = "on"
    }

    ip_configuration {
      ipv4_enabled    = "true"
      private_network = var.network.self_link
      dynamic "authorized_networks" {
        for_each = [for s in var.db_whitelist : {
          name  = s.name
          value = s.value
        }]

        content {
          name  = authorized_networks.value.name
          value = authorized_networks.value.value
        }
      }
    }

    maintenance_window {
      day  = 7
      hour = 7
    }

    backup_configuration {
      binary_log_enabled = var.db_backup_enabled
      enabled            = var.db_backup_enabled
      start_time         = "23:00"
    }

  }
}

resource "google_sql_database" "liqubase" {
  instance = google_sql_database_instance.database.name
  name     = "liquibase-changelog"
}