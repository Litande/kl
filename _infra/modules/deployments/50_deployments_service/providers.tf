# Retrieve an access token as the Terraform runner
data "google_client_config" "provider" {}

provider "kubernetes" {
  host                   = "https://${var.k8s_cluster_info.endpoint}"
  token                  = data.google_client_config.provider.access_token
  cluster_ca_certificate = base64decode(
    var.k8s_cluster_info.certificate,
  )
}

provider "helm" {
  kubernetes {
    host                   = "https://${var.k8s_cluster_info.endpoint}"
    token                  = data.google_client_config.provider.access_token
    cluster_ca_certificate = base64decode(
      var.k8s_cluster_info.certificate,
    )
  }
}
