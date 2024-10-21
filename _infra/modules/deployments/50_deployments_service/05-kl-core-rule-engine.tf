// DEPLOY BRAND GATEWAY PODS
locals {
  kl-core-rule-engine       = "kl-core-rule-engine"
  kl-core-rule-engine-image = "kl-rule-engine"
}

// DEPLOY BACKOFFICE API DEPLOYMENT
resource "kubernetes_deployment" "dial_rule_engine" {
  timeouts {
    create = "60s"
  }

  metadata {
    name      = local.kl-core-rule-engine
    namespace = local.dial_namespace

    labels = {
      app = local.kl-core-rule-engine
    }
  }

  spec {
    replicas = 0

    selector {
      match_labels = {
        app = local.kl-core-rule-engine
      }
    }

    strategy {
      type = "Recreate"
    }

    template {
      metadata {
        labels = {
          app = local.kl-core-rule-engine
        }
      }

      spec {

        volume {
          name = "cloud-storage-credentials-crm"

          secret {
            secret_name = "cloud-storage-credentials-crm"
          }
        }

        container {
          image             = "gcr.io/kl-manage/${local.kl-core-rule-engine-image}:${var.docker_tag}"
          name              = local.kl-core-rule-engine
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

          env {
            name  = "GOOGLE_APPLICATION_CREDENTIALS"
            value = "/secrets/cloudstorage/credentials.json"
          }
          volume_mount {
            name       = "cloud-storage-credentials-crm"
            read_only  = true
            mount_path = "/secrets/cloudstorage"
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
resource "kubernetes_service" "dial_rule_engine_service" {
  metadata {
    name      = local.kl-core-rule-engine
    namespace = local.dial_namespace
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = kubernetes_deployment.dial_rule_engine.spec.0.template.0.metadata[0].labels.app
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
