resource "kubernetes_stateful_set_v1" "izpbx_asterisk" {
  metadata {
    name      = "izpbx-asterisk"
    namespace = "default"
    labels    = {
      app = "izpbx-asterisk"
    }
  }
  spec {
    service_name = kubernetes_service_v1.izpbx_service.metadata.0.name
    selector {
      match_labels = {
        app = "izpbx-asterisk"
      }
    }
    template {
      metadata {
        labels = {
          app = "izpbx-asterisk"
        }
      }
      spec {
        container {
          name              = "asterisk"
          image             = "izdock/izpbx-asterisk:20"
          image_pull_policy = "IfNotPresent"

          env {
            name = "MYSQL_SERVER"
            value_from {
              secret_key_ref {
                name = "db"
                key  = "DB_HOST"
              }
            }
          }
          env {
            name = "MYSQL_PASSWORD"
            value_from {
              secret_key_ref {
                name = "db"
                key  = "DB_PASS"
              }
            }
          }
          env {
            name = "MYSQL_USER"
            value_from {
              secret_key_ref {
                name = "db"
                key  = "DB_USER"
              }
            }
          }
          env {
            name  = "MYSQL_DATABASE"
            value = "asterisk"
          }

          env {
            name = "MYSQL_ROOT_USER"
            value_from {
              secret_key_ref {
                name = "db"
                key  = "DB_USER"
              }
            }
          }
          env {
            name = "MYSQL_ROOT_PASSWORD"
            value_from {
              secret_key_ref {
                name = "db"
                key  = "DB_PASS"
              }
            }
          }
          env {
            name  = "MYSQL_DATABASE_CDR"
            value = "asteriskcdrdb"
          }

          volume_mount {
            mount_path = "/data"
            name       = "izpbx-data"
          }

          port {
            container_port = 80
            name           = "freepbx"
          }
          port {
            container_port = 443
            name           = "freepbx-ssl"
          }
          port {
            container_port = 4569
            name           = "iax"
          }
          port {
            container_port = 4569
            name           = "iax-udp"
            protocol       = "UDP"
          }
          port {
            container_port = 5060
            name           = "pjsip"
          }
          port {
            container_port = 5060
            name           = "pjsip-udp"
            protocol       = "UDP"
          }
          port {
            container_port = 5160
            name           = "sip"
          }
          port {
            container_port = 5160
            name           = "sip-udp"
            protocol       = "UDP"
          }
          port {
            container_port = 8089
            name           = "webrtc"
          }
          port {
            container_port = 8001
            name           = "ucp"
          }
          port {
            container_port = 8003
            name           = "ucp-https"
          }
          dynamic "port" {
            for_each = toset(range(10000, 10200))
            content {
              container_port = port.key
              protocol       = "UDP"
            }
          }
        }
        volume {
          name = "izpbx-data"
          persistent_volume_claim {
            claim_name = "izpbx-data"
          }
        }

        image_pull_secrets {
          name = "registry-ro-secret"
        }
      }
    }
    volume_claim_template {
      metadata {
        name = "izpbx-data"
      }
      spec {
        access_modes       = ["ReadWriteOnce"]
        storage_class_name = "standard-rwo"
        resources {
          requests = {
            storage = "10Gi"
          }
        }
      }
    }
  }
}

resource "kubernetes_service_v1" "izpbx_service" {
  metadata {
    name   = "izpbx-asterisk"
    labels = {
      app = "izpbx-asterisk"
    }
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = "izpbx-asterisk"
    }
    port {
      port = 80
      name = "freepbx"
    }
    port {
      port = 443
      name = "freepbx-ssl"
    }
    port {
      port = 4569
      name = "iax"
    }

    port {
      port = 5060
      name = "pjsip"
    }

    port {
      port = 5160
      name = "sip"
    }

    port {
      port = 8089
      name = "webrtc"
    }
    port {
      port = 8001
      name = "ucp"
    }
    port {
      port = 8003
      name = "ucp-https"
    }

    load_balancer_source_ranges = var.lb_source_ips
    type                        = "LoadBalancer"
  }
}
resource "kubernetes_service_v1" "izpbx_service_udp" {
  metadata {
    name   = "izpbx-asterisk-udp"
    labels = {
      app = "izpbx-asterisk-udp"
    }
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = "izpbx-asterisk"
    }
    port {
      port         = 4569
      name         = "iax-udp"
      app_protocol = "UDP"
    }
    port {
      port         = 5060
      name         = "pjsip-udp"
      app_protocol = "UDP"
    }
    port {
      port         = 5160
      name         = "sip-udp"
      app_protocol = "UDP"
    }

    dynamic "port" {
      for_each = toset(range(10000, 10200))
      content {
        port         = port.key
        name = "rtp-${port.key}"
        app_protocol = "UDP"
      }
    }

    load_balancer_source_ranges = var.lb_source_ips
    type                        = "LoadBalancer"
  }
}