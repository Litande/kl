
locals {
  root_domain = "kollink.ai"
}


module "project" {
  source         = "../../modules/infrastructure/00_project"
  gcp_project_id = var.gcp_project_id
  project_name   = var.project_name
}

module "vpc" {
  depends_on         = [module.project]
  source             = "../../modules/infrastructure/10_vpc"
  gcp_project_id     = var.gcp_project_id
  project_name       = var.project_name
  sub_network_region = var.project_region
}

module "sql" {
  depends_on     = [module.vpc]
  source         = "../../modules/infrastructure/20_sql"
  gcp_project_id = var.gcp_project_id
  project_name   = var.project_name
  network        = module.vpc.network

  // LOCAL CONFIGURATIONS
  db_deletion_protection_enabled = var.db_deletion_protection_enabled
  db_backup_enabled              = var.db_backup_enabled
  db_machine_type                = var.db_machine_type
  db_whitelist                   = var.db_whitelist
}

module "kubernetes" {
  depends_on     = [module.vpc, module.sql]
  source         = "../../modules/infrastructure/30_kubernetes"
  gcp_project_id = var.gcp_project_id
  project_name   = var.project_name

  // NETWORK AND LOCATION CONFIGURATIONS
  network      = module.vpc.network
  subnetwork   = module.vpc.subnetwork
  cluster_zone = var.project_zone

  // NODE POOL CONFIGURATION
  k8s_version     = var.k8s_version
  machine_version = var.machine_version
  machine_count   = var.machine_count

  // MASTER CONTROLLERS
  master_subnet_cidr      = var.master_subnet_cidr
  master_access_whitelist = var.master_access_whitelist
}

module "kubernetes_deployments_common" {
  source         = "../../modules/deployments/00_kubernetes_common"
  project_name   = var.project_name
  project_region = var.project_region

  // KUBERNETES PROVIDER
  k8s_cluster_info = module.kubernetes.k8s_cluster_info

  // SECRETS - DATABASE
  database = {
    host = module.sql.db.private,
    user = module.sql.db_app_access.username
    pass = module.sql.db_app_access.password
  }

  // SECRETS - CONTAINER REGISTRY
  registry_account = file("${path.module}/files/registry-authentication.json")

  ingress_source_ips = ""
  crm_storage_info   = module.project.crm_storage_info

  cloudflare_dns_token = ""
}

module "kubernetes_deployments_common_services" {
  source = "../../modules/deployments/40_deployments_common"

  // KUBERNETES PROVIDER
  k8s_cluster_info = module.kubernetes.k8s_cluster_info

  grafana_domain = "qa.grafana.${local.root_domain}"
}

# This module must be disabled until the K8S cluster created
module "kubernetes_cert_manager" {
  source = "../../modules/deployments/45_CertManager"
  // KUBERNETES PROVIDER
  k8s_cluster_info          = module.kubernetes.k8s_cluster_info
  letsencrypt_account_email = ""
}


module "kubernetes_deployments_service" {
  source = "../../modules/deployments/50_deployments_service"

  // KUBERNETES PROVIDER
  k8s_cluster_info = module.kubernetes.k8s_cluster_info

  // DEPLOYMENTS CONFIGURATIONS
  docker_tag = "qa"

  dial_manager_web_host_name = "qa.manager.${local.root_domain}"
  dial_agent_web_host_name   = "qa.agent.${local.root_domain}"
  dial_manager_api_host_name = "qa.manager.api.${local.root_domain}"
  dial_agent_api_host_name   = "qa.agent.api.${local.root_domain}"


  external_ip = module.kubernetes_deployments_common.external_ip

  sip_bridge_hostname = "qa.sip.bridge.${local.root_domain}"
  sip_user_name       = ""
  sip_password        = ""
  sip_provider        = ""

  lb_source_ips = [""]

  google_cloud_storage_bucket = module.project.crm_storage_info.name
  project_id                  = var.gcp_project_id
  dial_static_api_host_name   = "qa.statistic.api.${local.root_domain}"
}

#module "izpbx-asterisk" {
#  source           = "../../modules/deployments/60_asterisk"
#  k8s_cluster_info = module.kubernetes.k8s_cluster_info
#  lb_source_ips    = [""]
#
#}
