variable "gcp_project_id" {}

variable "project_name" {}

variable "network" {
  type = object({
    name      = string
    self_link = string
  })
}

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
    }
  ]
}

variable "db_machine_type" {
  default = "db-n1-standard-2"
}

