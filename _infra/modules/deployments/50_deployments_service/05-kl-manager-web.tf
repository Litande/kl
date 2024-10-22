// DEPLOY BRAND GATEWAY PODS
locals {
  web-dial = "kl-manager-web"
}

// DEPLOY BACKOFFICE API DEPLOYMENT
resource "kubernetes_deployment" "web_dial" {
  timeouts {
    create = "60s"
  }

  metadata {
    name      = local.web-dial
    namespace = local.dial_namespace

    labels = {
      app = local.web-dial
    }
  }

  spec {
    replicas = 0

    selector {
      match_labels = {
        app = local.web-dial
      }
    }

    strategy {
      type = "Recreate"
    }

    template {
      metadata {
        labels = {
          app = local.web-dial
        }
      }

      spec {
        container {
          image             = "gcr.io/kl-manage/${local.web-dial}:${var.docker_tag}"
          name              = local.web-dial
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

resource "kubernetes_service" "web_dial_service" {
  metadata {
    name      = local.web-dial
    namespace = local.dial_namespace
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = kubernetes_deployment.web_kl.spec.0.template.0.metadata[0].labels.app
    }
    port {
      name        = "http"
      port        = 80
      target_port = 80
    }
    load_balancer_source_ranges = var.lb_source_ips

    type = "LoadBalancer"
  }
}

module "web_dialer_nginx_ingress" {
  source    = "./modules/ingress"
  host      = var.dial_manager_web_host_name
  name      = local.web-dial
  namespace = local.dial_namespace
}
#
#module "web_dialer_agent_nginx_ingress" {
#  source    = "./modules/ingress"
#  host      = var.dial_agent_web_host_name
#  name      = local.web-kl-agent
#  backend_service = local.web-dial
#  namespace = local.dial_namespace
#}
