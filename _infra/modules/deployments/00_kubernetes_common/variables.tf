variable "project_name" {
  type        = string
  description = "The TF item prefix"
}

variable "project_region" {
  type    = string
  default = "europe-west1"
}

variable "k8s_cluster_info" {
  description = "Kubernetes Target"
  type        = object({
    name        = string
    location    = string
    endpoint    = string
    certificate = string
  })
}

variable "database" {
  description = "Database access credentials and endpoint"
  type        = object({
    host = string
    user = string
    pass = string
  })
}

variable "registry_account" {
  type        = string
  description = "Json service account for container registry access"
}

variable "ingress_source_ips" {
  default = "0.0.0.0/0"
  type    = string
}

variable "crm_storage_info" {
  description = "CRM google cloud storage location in access"
  type        = object({
    name = string
    key  = string
  })
}

variable "cloudflare_dns_token" {
  type    = string
  default = ""
}