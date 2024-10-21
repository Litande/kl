resource "google_compute_firewall" "kubernetes-from-cloudflare" {
  name        = "${var.network_name}-from-cloudflare-to-kubernetes"
  description = "Firewall for access http"
  network     = google_compute_network.network.self_link

  allow {
    protocol = "tcp"
    ports    = ["80"]
  }

  source_ranges = var.kubernetes_from_cloudflare_allowed_cidr
  target_tags   = ["kubernetes"]
}
