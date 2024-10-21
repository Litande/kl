resource "kubernetes_secret" "registry_secret" {
  metadata {
    name = "registry-ro-secret"
  }

  data = {
    ".dockerconfigjson" = var.registry_account
  }

  type = "kubernetes.io/dockerconfigjson"
}

resource "kubernetes_secret" "db" {
  metadata {
    name = "db"
  }

  data = {
    CLIENTS__MYSQL__HOST = var.database.host
    CLIENTS__MYSQL__PORT = 3306
    CLIENTS__MYSQL__USER = var.database.user
    CLIENTS__MYSQL__PASS = var.database.pass

    DB_HOST = var.database.host
    DB_PORT = 3306
    DB_USER = var.database.user
    DB_PASS = var.database.pass
  }
}

resource "kubernetes_secret" "cloud-storage-credentials-crm" {
  metadata {
    name = "cloud-storage-credentials-crm"
  }

  data = {
    "credentials.json" = base64decode(var.crm_storage_info.key)
  }
}

resource "kubernetes_secret_v1" "cloudflare-dns-token" {
  metadata {
    name = "cloudflare-dns-token"
    namespace = "cert-manager"
  }
  data = {
    api-token : var.cloudflare_dns_token
  }
}