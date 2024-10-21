variable "k8s_cluster_info" {
  description = "Kubernetes Target"
  type        = object({
    name        = string
    location    = string
    endpoint    = string
    certificate = string
  })
}

variable "lb_source_ips" {
  default = []
  type    = list(string)
}