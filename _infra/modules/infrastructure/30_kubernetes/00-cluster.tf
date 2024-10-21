resource "google_container_cluster" "gcp_kubernetes" {
  provider                 = google-beta
  name                     = "${var.project_name}-k8s"
  initial_node_count       = 1
  network                  = var.network.self_link
  subnetwork               = var.subnetwork.self_link
  location                 = var.cluster_zone
  remove_default_node_pool = true
  logging_service          = "none"

  min_master_version = var.k8s_version

  release_channel {
    channel = "UNSPECIFIED"
  }

#  logging_config {
#    enable_components = []
#  }

  # For NAT
  pod_security_policy_config {
    enabled = false
  }

  private_cluster_config {
    enable_private_nodes    = true # For NAT
    enable_private_endpoint = false
    master_ipv4_cidr_block  = var.master_subnet_cidr # For NAT
  }

  ip_allocation_policy {
    cluster_secondary_range_name  = "pods"
    services_secondary_range_name = "services"
  }

  master_authorized_networks_config {
    dynamic "cidr_blocks" {
      for_each = [
      for s in var.master_access_whitelist : {
        display_name = s.name
        cidr_block   = s.block
      }
      ]

      content {
        display_name = cidr_blocks.value.display_name
        cidr_block   = cidr_blocks.value.cidr_block
      }
    }
  }

  node_config {
    preemptible  = false
    machine_type = "n1-standard-2"

    oauth_scopes = [
      "storage-ro",
      "logging-write",
      "monitoring",
    ]

    workload_metadata_config {
      mode = "GCE_METADATA"
    }

    tags = ["kubernetes", var.project_name, var.network.name]
  }

  lifecycle {
    ignore_changes = [
      node_config
    ]
  }
}


resource "google_container_node_pool" "kube_node_pool" {
  provider   = google-beta
  name       = "${var.project_name}-kube-node-pool"
  location   = var.cluster_zone
  cluster    = google_container_cluster.gcp_kubernetes.name
  node_count = var.machine_count


  node_config {
    preemptible  = false
    machine_type = "n1-standard-2"

    oauth_scopes = [
      "storage-ro",
      "logging-write",
      "monitoring",
    ]

    workload_metadata_config {
      mode = "GCE_METADATA"
    }

    tags   = ["kubernetes", var.project_name, var.network.name]
    labels = {
      type = "regular"
    }
  }

  management {
    auto_repair  = true
    auto_upgrade = true
  }

  lifecycle {
    ignore_changes = [
      node_count,
    ]
  }
}