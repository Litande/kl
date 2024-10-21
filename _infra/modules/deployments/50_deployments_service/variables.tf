variable "k8s_cluster_info" {
  description = "Kubernetes Target"
  type        = object({
    name        = string
    location    = string
    endpoint    = string
    certificate = string
  })
}
variable "docker_tag" {
  description = "Docker Tag For Docker Image Usage"
}

variable "dial_manager_web_host_name" {
  type = string
}

variable "dial_manager_api_host_name" {
  type = string
}

variable "dial_agent_web_host_name" {
  type = string
}

variable "dial_agent_api_host_name" {
  type = string
}

variable "lb_source_ips" {
  default = []
  type    = list(string)
}

variable "external_ip" {
  type = string
}

variable "sip_bridge_hostname" {
  type = string
}
variable "sip_password" {
  type = string
}
variable "sip_user_name" {
  type = string
}
variable "sip_provider" {
  type = string
}

variable "sip_transport_port" {
  default = 6060
  type    = number
}

variable "storage_type" {
  default = "google"
  type    = string
}

variable "project_id" {
  type = string
}
variable "google_cloud_storage_bucket" {
  type = string
}

variable "dial_static_api_host_name" {
}