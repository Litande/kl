output "db" {
  value = {
    public = google_sql_database_instance.database.public_ip_address,
    private = google_sql_database_instance.database.private_ip_address,
  }
}


output "db_app_access" {
  value = {
    username =  google_sql_user.application_user.name,
    password = google_sql_user.application_user.password
  }
  sensitive = true
}
