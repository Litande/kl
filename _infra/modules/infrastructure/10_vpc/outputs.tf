output "network" {
  value = {
    name      = google_compute_network.network.name
    self_link = google_compute_network.network.self_link
  }
}

output "subnetwork" {
  value = {
    name      = google_compute_subnetwork.subnetwork.name
    self_link = google_compute_subnetwork.subnetwork.self_link
  }
}

output "nat_ips" {
  value = google_compute_address.address.*.address
}
