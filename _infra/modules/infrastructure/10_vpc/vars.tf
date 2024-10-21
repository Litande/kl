variable "gcp_project_id" {}

variable "project_name" {}

variable "vpc_name" {
  default = "vpc"
}
variable "network_name" {
  default = "subnet"
}
variable "sub_network_region" {
  default = "europe-west"
}

variable "subnet_cidr" {
  default = "10.0.0.0/22" // 1022 Machines
}
variable "subnet_cidr_services" {
  default = "10.0.4.0/22" // 1022 Services
}
variable "subnet_cidr_pods" {
  default = "10.0.16.0/20" // 4094 Pods
}

variable "nat_address" {
  default     = 1
  description = "Amount of nat addresses to attach to network"
}

variable "kubernetes_from_cloudflare_allowed_cidr" {
  default = [
    "173.245.48.0/20",
    "103.21.244.0/22",
    "103.22.200.0/22",
    "103.31.4.0/22",
    "141.101.64.0/18",
    "108.162.192.0/18",
    "190.93.240.0/20",
    "188.114.96.0/20",
    "197.234.240.0/22",
    "198.41.128.0/17",
    "162.158.0.0/15",
    "104.16.0.0/12",
    "172.64.0.0/13",
    "131.0.72.0/22"
  ]
}
