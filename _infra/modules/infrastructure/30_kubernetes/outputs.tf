output "k8s_cluster_info" {
  value = { 
    name = google_container_cluster.gcp_kubernetes.name
    location = var.cluster_zone
    endpoint = google_container_cluster.gcp_kubernetes.endpoint
    certificate = google_container_cluster.gcp_kubernetes.master_auth[0].cluster_ca_certificate
  } 
}