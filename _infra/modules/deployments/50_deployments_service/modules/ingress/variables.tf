variable "name" {
  type = string
}
variable "namespace" {
  type    = string
  default = "default"
}
variable "host" {
  type = string
}

variable "backend_service" {
  type    = string
  default = ""
}

variable "nginx_whitelist" {
  default = "0.0.0.0/0"
}