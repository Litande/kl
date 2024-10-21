variable "gcp_project_id" {
  type        = string
  description = "The ID of the GCP Project"
  default     = "plat4me-dial"
}

variable "project_name" {
  type        = string
  description = "The TF item prefix"
  default     = "kl-qa"
}

variable "project_region" {
  type    = string
  default = "europe-west1"
}

variable "project_zone" {
  type    = string
  default = "europe-west1-d"
}

// NETWORK CONFIGURATIONS
variable "subnet_cidr_main" {
  type        = string
  default     = "10.0.0.0/22" // 1022 Machines
  description = "CIDR for all servers in network"
}

variable "subnet_cidr_services" {
  type        = string
  default     = "10.0.4.0/22" // 1022 Services
  description = "CIDR for K8S Services"
}

variable "subnet_cidr_pods" {
  type        = string
  default     = "10.0.16.0/20" // 4094 Pods
  description = "CIDR for K8S Pods"
}


// SQL CONFIGURATIONS
variable "db_deletion_protection_enabled" {
  type    = bool
  default = false
}

variable "db_backup_enabled" {
  description = "Sets if to database backups are enabled"
  type        = bool
  default     = true
}

variable "db_whitelist" {
  description = "A list of whitelisted IP addresses."
  type        = list(any)
  default = [
    {
      name  = "Work VPN"
      value = "34.76.130.253/32"
    },
    {
      name  = "Klips VPN"
      value = "31.154.78.226/32"
    }
  ]
}

variable "db_machine_type" {
  default = "db-n1-standard-4"
}



// KUBERNETES CONFIGURATIONS




variable "k8s_version" {
  type        = string
  description = "Kubernetes version"
  default     = "1.21.14-gke.2700"
}

variable "machine_version" {
  type        = string
  description = "Kubernetes version"
  default     = "1.21.14-gke.2700"
}

variable "machine_count" {
  description = "Number of machines to run node pool"
  type        = number
  default     = 3
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
      name  = "github-action-runner-0"
    },
    {
      block = "34.76.188.9/32"
      name  = "github-action-runner-1"
    },
    {
      block = "34.78.8.100/32"
      name  = "github-action-runner-2"
    },
    {
      block = "35.240.77.98/32"
      name  = "github-arc-access"
    }
  ]
}
