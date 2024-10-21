resource "google_compute_firewall" "prometheus_from_master" {
  name    = "${var.network.name}-from-gke-master-to-prometheus"
  network = var.network.self_link

  allow {
    protocol = "tcp"
    ports = ["9090"]
  }

  source_ranges = [google_container_cluster.gcp_kubernetes.private_cluster_config.0.master_ipv4_cidr_block]
  target_tags = ["kubernetes"]
}