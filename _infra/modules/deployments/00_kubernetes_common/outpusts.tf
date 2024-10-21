output "external_ip" {
  value = google_compute_address.platform_external_ip.address
}