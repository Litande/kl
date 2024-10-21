// CRM SERVICES STORAGE
resource "google_service_account" "crm-storage-sa" {
  project      = var.gcp_project_id
  account_id   = "${var.project_name}-crm-storage"
  display_name = "${var.gcp_project_id}-crm-storage"
  description  = "CRM Storage Service Account"
  depends_on   = [google_project_service.iam_api]
}

resource "google_service_account_key" "crm-storage-sa-key" {
  service_account_id = google_service_account.crm-storage-sa.name
  depends_on = [
    google_service_account.crm-storage-sa
  ]
}

resource "google_storage_bucket" "crm-storage" {
  project       = var.gcp_project_id
  name          = "${var.gcp_project_id}-crm-files-storage"
  location      = "EUROPE-WEST1"
  storage_class = "COLDLINE"
}

resource "google_storage_bucket_iam_member" "member" {
  bucket = google_storage_bucket.crm-storage.name
  role   = "roles/storage.admin"
  member = "serviceAccount:${google_service_account.crm-storage-sa.email}"
}