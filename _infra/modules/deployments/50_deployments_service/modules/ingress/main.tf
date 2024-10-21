resource "kubernetes_ingress_v1" "ingress" {

  metadata {
    name        = var.name
    namespace   = var.namespace
    annotations = {
      "kubernetes.io/ingress.class"    = "nginx"
      "cert-manager.io/cluster-issuer" = "letsencrypt-prod"
      "nginx.ingress.kubernetes.io/whitelist-source-range" = var.nginx_whitelist
    }
  }

  spec {
    rule {
      host = var.host
      http {
        path {
          backend {
            service {
              name = var.name
              port {
                number = 80
              }
            }
          }

          path = "/"
        }
      }
    }
    tls {
      hosts = [var.host]
      secret_name = "${var.host}-tls"
    }
  }
}