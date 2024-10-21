variable "k8s_cluster_info" {
  description = "Kubernetes Target"
  type        = object({
    name        = string
    location    = string
    endpoint    = string
    certificate = string
  })
}

variable "letsencrypt_account_email" {
  type = string
}