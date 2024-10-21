output "project" {
  value     = module.project
  sensitive = true
}

output "vpc" {
  value = module.vpc
}

output "sql" {
  value = module.sql
  sensitive = true
}

output "kubernetes" {
  value = module.kubernetes
  sensitive = true
}

output "stun_proxy" {
  value = module.kubernetes_deployments_service.dial_stun_proxy
}