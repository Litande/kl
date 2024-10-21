
resource "google_compute_network" "network" {
  project                 = var.gcp_project_id
  name                    = "${var.project_name}-${var.vpc_name}"
  auto_create_subnetworks = "false"
}

resource "google_compute_subnetwork" "subnetwork" {
  project                  = var.gcp_project_id
  name                     = "${var.project_name}-${var.network_name}" # for NAT
  region                   = var.sub_network_region
  network                  = google_compute_network.network.self_link
  private_ip_google_access = true # for NAT

  // RANGES
  ip_cidr_range = var.subnet_cidr
  secondary_ip_range {
    range_name    = "pods"
    ip_cidr_range = var.subnet_cidr_pods
  }
  secondary_ip_range {
    range_name    = "services"
    ip_cidr_range = var.subnet_cidr_services
  }
}

// Implementation of NAT
resource "google_compute_router" "router" {
  project = var.gcp_project_id
  name    = "${var.network_name}-router-core"
  network = google_compute_network.network.self_link
  region  = google_compute_subnetwork.subnetwork.region

  bgp {
    asn = 64514
  }
}

resource "google_compute_address" "address" {
  count  = var.nat_address
  name   = "${var.network_name}-nat-manual-ip-${count.index}"
  region = google_compute_subnetwork.subnetwork.region
}

resource "google_compute_router_nat" "nat_manual" {
  name   = "${var.network_name}-router-nat-core"
  router = google_compute_router.router.name
  region = google_compute_router.router.region

  nat_ip_allocate_option = "MANUAL_ONLY"
  nat_ips                = google_compute_address.address.*.self_link

  source_subnetwork_ip_ranges_to_nat = "LIST_OF_SUBNETWORKS"
  subnetwork {
    name                    = google_compute_subnetwork.subnetwork.self_link
    source_ip_ranges_to_nat = ["ALL_IP_RANGES"]
  }
}
