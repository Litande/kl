// DEPLOY BRAND GATEWAY PODS
locals {
  kl-statistic = "kl-statistic"
}

// DEPLOY BACKOFFICE API DEPLOYMENT
resource "kubernetes_deployment_v1" "dial_static_api" {
  timeouts {
    create = "60s"
  }

  metadata {
    name      = local.kl-statistic
    namespace = local.dial_namespace

    labels = {
      app = local.kl-statistic
    }
  }

  spec {
    replicas = 0

    selector {
      match_labels = {
        app = local.kl-statistic
      }
    }

    strategy {
      type = "Recreate"
    }

    template {
      metadata {
        labels = {
          app = local.kl-statistic
        }
      }

      spec {
        container {
          image             = "gcr.io/kl-manage/${local.kl-statistic}:${var.docker_tag}"
          name              = local.kl-statistic
          image_pull_policy = "Always"

          env_from {
            secret_ref {
              name = "db"
            }
          }

          env_from {
            config_map_ref {
              name = local.dial_shared_configmap
            }
          }
        }

        image_pull_secrets {
          name = "registry-ro-secret"
        }
      }
    }
  }

  wait_for_rollout = false

  lifecycle {
    ignore_changes = [
      spec[0].replicas,
    ]
  }
}

// DEPLOY BACKOFFICE API SERVICE
resource "kubernetes_service_v1" "dial_static_api_service" {
  metadata {
    name      = local.kl-statistic
    namespace = local.dial_namespace
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = kubernetes_deployment_v1.dial_static_api.spec.0.template.0.metadata[0].labels.app
    }
    port {
      name        = "http"
      port        = 80
      target_port = 80
    }


    type = "LoadBalancer"
  }
}

module "dial_static_api_nginx_ingress" {
  source    = "./modules/ingress"
  host      = var.dial_static_api_host_name
  name      = local.kl-statistic
  namespace = local.dial_namespace
}
