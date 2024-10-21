variable "k8s_cluster_info" {
  description = "Kubernetes Target"
  type = object({
    name        = string
    location    = string
    endpoint    = string
    certificate = string
  })
}

variable "grafana_domain" {
  type = string
}

variable "cert_manager_version" {
  default = "1.9.1"
}

variable "redis_image_registry" {
  default = "gcr.io"
}
variable "redis_image_repo" {
  default = "kl-manage/cnv-redis-cluster-cnv"
}
variable "redis_image_tag" {
  default = "6.2.7-debian-11-r9"
}
