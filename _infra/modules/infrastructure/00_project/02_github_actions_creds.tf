resource "google_project_iam_member" "project" {
  project = var.gcp_project_id
  role    = "roles/container.admin"
  member  = "serviceAccount:github-actions@p3marketers-manage.iam.gserviceaccount.com"
}
