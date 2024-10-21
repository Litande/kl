resource "google_service_account" "terra1form-sa" {
  account_id   = "terraform-sa"
  display_name = "Terraform SA"
}

resource "google_service_account_key" "tfstate_sa_key" {
  service_account_id = google_service_account.terra1form-sa.name
}

resource "local_file" "save_sa_key" {
  filename = "account.json"
  content = base64decode(google_service_account_key.tfstate_sa_key.private_key)
}

resource "google_project_iam_member" "terraform-sa-binding" {
  project = var.gcp_project_id
  role    = "roles/owner"
  member  = "serviceAccount:${google_service_account.terra1form-sa.email}"
}

resource "google_storage_bucket" "tfstate_buckets" {
  for_each      = var.gcs_buckets
  location      = each.value.location
  name          = "${var.gcp_project_id}_${each.key}"
  storage_class = each.value.class
  versioning {
    enabled = each.value.versioning
  }
  lifecycle_rule {
    action {
      type = "Delete"
    }
    condition {
      num_newer_versions = 3
      with_state         = "ARCHIVED"
    }
  }
}