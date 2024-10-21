resource "google_project_service" "cloudresourcemanager" {
  project = var.gcp_project_id
  service = "cloudresourcemanager.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false
}

resource "time_sleep" "delay_for_cloudresourcemanager_activation" {
  create_duration = "2m"
  triggers        = {
    cloudresourcemanager_id = google_project_service.cloudresourcemanager.id
  }
}

resource "google_project_service" "iam_api" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "iam.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false
}
resource "google_project_service" "cloud_resouyrce_manager" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "cloudresourcemanager.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false
}

resource "google_project_service" "container_api" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "container.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false
}

resource "google_project_service" "project-services-compute" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "compute.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false

}

resource "google_project_service" "project-services-sql" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "sqladmin.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false

}

resource "google_project_service" "project-services-networking" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "servicenetworking.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false

}

resource "google_project_service" "project-services-cloudapis" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "cloudapis.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false

}

resource "google_project_service" "storage-transfer-api" {
  depends_on = [time_sleep.delay_for_cloudresourcemanager_activation]
  project    = var.gcp_project_id
  service    = "storagetransfer.googleapis.com"

  disable_dependent_services = true
  disable_on_destroy         = false

}
