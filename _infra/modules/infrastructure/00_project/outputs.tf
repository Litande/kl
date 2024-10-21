output "crm_storage_info" {
  value = {
    name = google_storage_bucket.crm-storage.name
    key  = google_service_account_key.crm-storage-sa-key.private_key
  }
  sensitive = true
}