variable "gcp_project_id" {
  type        = string
  description = "The ID of the GCP Project"
}

variable "project_name" {
  type        = string
  description = "The TF item prefix"
}

variable "network" {
  type = object({
    name      = string
    self_link = string
  })
}

variable "subnetwork" {
  type = object({
    name      = string
    self_link = string
  })
}

variable "cluster_zone" {
  type    = string
  default = "europe-west1-d"
}



variable "k8s_version" {
  type        = string
  description = "Kubernetes version"
  default     = "1.20.11-gke.1300"
}

variable "machine_version" {
  type        = string
  description = "Kubernetes version"
  default     = "1.20.11-gke.1300"
}


variable "machine_count" {
  description = "Number of machines to run node pool"
  type = number
  default = 5
}

variable "master_subnet_cidr" {
  default     = "10.0.8.0/28" // 14 IP for Master Nodes
  description = "CIDR for K8S master nodes"
}

variable "master_access_whitelist" {
  type = list(object({
    block = string
    name  = string
  }))
  default = [
    {
      block = "34.76.130.253/32"
      name  = "work-vpn"
    }, 
    {
      block = "34.79.80.237/32"
      name = "github-action-runner-0"
    }, 
    {
      block = "34.76.188.9/32"
      name = "github-action-runner-1"
    }, 
    {
      block = "34.78.8.100/32"
      name = "github-action-runner-2"
    }
  ]
}

