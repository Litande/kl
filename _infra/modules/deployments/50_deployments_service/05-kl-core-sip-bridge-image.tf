// DEPLOY BRAND GATEWAY PODS
locals {
  kl-core-sip-bridge       = "kl-core-sip-bridge"
  kl-core-sip-bridge-image = "kl-sip-bridge"
  volume_claim_name        = "stun-records"
}
resource "kubernetes_storage_class_v1" "ssd-rwo" {
  #  storage_provisioner = "pd.csi.storage.gke.io"
  storage_provisioner = "kubernetes.io/gce-pd"
  metadata {
    name = "custom-premium-rwo"
    labels = {
      "addonmanager.kubernetes.io/mode" = "EnsureExists"
      "k8s-app"                         = "gcp-compute-persistent-disk-csi-driver"
    }
  }
  allow_volume_expansion = true
  allowed_topologies {
    match_label_expressions {
      key    = "topology.gke.io/zone"
      values = ["europe-west1-d"]
    }
  }
  volume_binding_mode = "WaitForFirstConsumer"
  parameters = {
    type             = "pd-ssd"
    replication-type = "regional-pd"
  }
}

#resource "kubernetes_persistent_volume_v1" "stun_recordings_volume" {
#  metadata {
#    name = local.volume_claim_name
#  }
#
#  spec {
#    capacity = {
#      storage = "50Gi"
#    }
#    access_modes       = ["ReadWriteOnce"]
#    storage_class_name = "premium-rwo"
#    node_affinity {
#      required {
#        node_selector_term {
#          match_expressions {
#            key      = "topology.gke.io/zone"
#            operator = "In"
#            values   = ["europe-west1-d"]
#          }
#        }
#      }
#    }
#    persistent_volume_source {
#      gce_persistent_disk {
#        pd_name = "${local.volume_claim_name}-disk"
#      }
#    }
#  }
#}
resource "kubernetes_persistent_volume_claim_v1" "stun_recordings_claim" {
  metadata {
    name = local.volume_claim_name
  }
  spec {
    resources {
      requests = {
        storage = "50Gi"
      }
    }
    access_modes       = ["ReadWriteOnce"]
    storage_class_name = "premium-rwo"
    #    storage_class_name = kubernetes_storage_class_v1.ssd-rwo.metadata.0.name
    #    volume_name        = "${local.volume_claim_name}-disk"
  }

  wait_until_bound = false
}
// DEPLOY BACKOFFICE API DEPLOYMENT
resource "kubernetes_deployment" "dial_sip_bridge" {
  timeouts {
    create = "60s"
  }

  metadata {
    name      = local.kl-core-sip-bridge
    namespace = local.dial_namespace

    labels = {
      app = local.kl-core-sip-bridge
    }
  }

  spec {
    replicas = 0

    selector {
      match_labels = {
        app = local.kl-core-sip-bridge
      }
    }

    strategy {
      type = "Recreate"
    }

    template {
      metadata {
        labels = {
          app = local.kl-core-sip-bridge
        }
      }

      spec {
        volume {
          name = "recordings"
          persistent_volume_claim {
            claim_name = local.volume_claim_name
          }
        }

        volume {
          name = "cloud-storage-credentials-crm"

          secret {
            secret_name = "cloud-storage-credentials-crm"
          }
        }

        container {
          image             = "gcr.io/kl-manage/${local.kl-core-sip-bridge-image}:${var.docker_tag}"
          name              = local.kl-core-sip-bridge
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
            container_port = 80
            name           = "web"
          }
          port {
            container_port = 7790
            name           = "ws"
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

          volume_mount {
            mount_path = "/app/temp"
            name       = "recordings"
          }

          resources {
            limits = {
              memory = "500Mi"
              cpu    = "400m"
            }
            requests = {
              memory = "500Mi"
              cpu    = "400m"
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
resource "kubernetes_service" "dial_sip_bridge_service" {
  metadata {
    name      = local.kl-core-sip-bridge
    namespace = local.dial_namespace
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = kubernetes_deployment.dial_sip_bridge.spec.0.template.0.metadata[0].labels.app
    }
    port {
      name        = "http"
      port        = 80
      target_port = 80
    }
    port {
      name        = "wss"
      port        = 7790
      target_port = 7790
    }
    #    load_balancer_source_ranges = var.lb_source_ips

    type = "LoadBalancer"
  }
}

resource "kubernetes_ingress_v1" "ingress-wss" {

  metadata {
    name      = "${local.kl-core-sip-bridge}-wss"
    namespace = local.dial_namespace
    annotations = {
      "kubernetes.io/ingress.class"                    = "nginx"
      "nginx.ingress.kubernetes.io/proxy-read-timeout" = "3600"
      "nginx.ingress.kubernetes.io/proxy-send-timeout" = "3600"
      "nginx.ingress.kubernetes.io/server-snippets"    = <<EOF
      location /ws {
      proxy_set_header Upgrade $http_upgrade;
      proxy_http_version 1.1;
      proxy_set_header X-Forwarded-Host $http_host;
      proxy_set_header X-Forwarded-Proto $scheme;
      proxy_set_header X-Forwarded-For $remote_addr;
      proxy_set_header Host $host;
      proxy_set_header Connection "upgrade";
      proxy_cache_bypass $http_upgrade;
      }
      EOF
    }
  }

  spec {
    rule {
      host = var.sip_bridge_hostname
      http {
        path {
          backend {
            service {
              name = local.kl-core-sip-bridge
              port {
                number = 7790
              }
            }
          }
          path = "/ws"
        }
      }
    }
  }

  #  spec {
  #    rule {
  #      host = var.sip_bridge_hostname
  #      http {
  #        path {
  #          backend {
  #            service_name = local.kl-core-sip-bridge
  #            service_port = 7790
  #          }
  #          path = "/ws"
  #        }
  #      }
  #    }
  #  }
}

module "dial_sip_bridge_nginx_ingress" {
  source    = "./modules/ingress"
  host      = var.sip_bridge_hostname
  name      = local.kl-core-sip-bridge
  namespace = local.dial_namespace
}
