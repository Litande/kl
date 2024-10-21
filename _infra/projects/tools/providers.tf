provider "google" {
  project     = var.gcp_project_id
  zone        = "europe-west1-d"
}

provider "google-beta" {
  project     = var.gcp_project_id
  zone        = "europe-west1-d"
}

terraform {
  backend "gcs" {
    bucket      = "plat4me-dial_tfstate_service"
    prefix      = "terraform/tfstate_prod"
  }
}
