provider "google" {
  credentials = file("account.json")
  project     = var.gcp_project_id
  zone        = "europe-west1-d"
}

provider "google-beta" {
  credentials = file("account.json")
  project     = var.gcp_project_id
  zone        = "europe-west1-d"
}

terraform {
  backend "gcs" {
    bucket      = "kl_tfstate"
    credentials = "account.json"
  }
}
