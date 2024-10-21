// DEPLOY BRAND GATEWAY PODS
locals {
  kl-stun = "kl-stun"
}

// DEPLOY BACKOFFICE API DEPLOYMENT
resource "kubernetes_deployment" "dial_stun" {
  timeouts {
    create = "60s"
  }

  metadata {
    name      = local.kl-stun
    namespace = local.dial_namespace

    labels = {
      app = local.kl-stun
    }
  }

  spec {
    replicas = 0

    selector {
      match_labels = {
        app = local.kl-stun
      }
    }

    strategy {
      type = "Recreate"
    }

    template {
      metadata {
        labels = {
          app = local.kl-stun
        }
      }

      spec {
        container {
          image             = "gcr.io/kl-manage/${local.kl-stun}:${var.docker_tag}"
          name              = local.kl-stun
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

          port {
            name           = "stun-primary"
            container_port = 3478
            protocol       = "UDP"

          }
          port {
            name           = "stun-alt"
            container_port = 3479
            protocol       = "UDP"
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

resource "kubernetes_service_v1" "dial_stun_service" {
  metadata {
    name      = local.kl-stun
    namespace = local.dial_namespace
    annotations = {
      "networking.gke.io/load-balancer-type" = "Cluster"
    }
  }
  spec {
    selector = {
      app = kubernetes_deployment.dial_stun.spec.0.template.0.metadata[0].labels.app
    }
    port {
      name     = "stun-primary"
      port     = 3478
      protocol = "UDP"
    }
    port {
      name     = "stun-alt"
      port     = 3479
      protocol = "UDP"
    }
    external_traffic_policy     = "Local"
    load_balancer_source_ranges = var.lb_source_ips

    type = "LoadBalancer"
  }
}
#resource "kubernetes_service_v1" "dial_stun_service_v2" {
#  metadata {
#    name      = "${ local.kl-stun }-v2"
#    namespace = local.dial_namespace
#    #    annotations = {
#    #      "networking.gke.io/load-balancer-type" = "Cluster"
#    #    }
#  }
#  spec {
#    selector = {
#      app = kubernetes_deployment.dial_stun.spec.0.template.0.metadata[0].labels.app
#    }
#    port {
#      name     = "stun-primary"
#      port     = 3478
#      protocol = "UDP"
#    }
#    port {
#      name     = "stun-alt"
#      port     = 3479
#      protocol = "UDP"
#    }
#
#    #    load_balancer_source_ranges = var.lb_source_ips
#    type = "ClusterIP"
#    #    type = "NodePort"
#    #    external_traffic_policy = "Local"
#  }
#}
#resource "kubernetes_service_v1" "dial_stun_service_v3" {
#  metadata {
#    name      = "${ local.kl-stun }-v3"
#    namespace = local.dial_namespace
#    #    annotations = {
#    #      "networking.gke.io/load-balancer-type" = "Cluster"
#    #    }
#  }
#  spec {
#    selector = {
#      app = kubernetes_deployment.dial_stun.spec.0.template.0.metadata[0].labels.app
#    }
#    port {
#      name     = "stun-primary"
#      port     = 3478
#      protocol = "UDP"
#    }
#    port {
#      name     = "stun-alt"
#      port     = 3479
#      protocol = "UDP"
#    }
#
#    #    load_balancer_source_ranges = var.lb_source_ips
#    #    type = "ClusterIP"
#    type                    = "NodePort"
#    external_traffic_policy = "Local"
#  }
#}

#module "dial_stuner_nginx_ingress" {
#  source    = "./modules/ingress"
#  host      = var.dial_manager_web_host_name
#  name      = local.kl-stun
#  namespace = local.dial_namespace
#}
