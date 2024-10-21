

resource "random_string" "username" {
  length           = 8
  special          = true
  override_special = "/@£$"
}

resource "random_password" "password" {
  length           = 16
  special          = true
  override_special = "_%@"
}

resource "google_sql_user" "application_user" {
  instance = google_sql_database_instance.database.name
  name     = random_string.username.result 
  password = random_password.password.result 
  host     = "%"
}

