resource "kubernetes_manifest" "lets-encrypt" {
  manifest = {
    "apiVersion" = "cert-manager.io/v1"
    "kind"       = "ClusterIssuer"
    "metadata"   = {
      "name" = "letsencrypt-prod"
    }
    "spec" = {
      "acme" = {
        "email"               = var.letsencrypt_account_email
        "privateKeySecretRef" = {
          "name" = "letsencrypt-keys-prod"
        }
        "server"  = "https://acme-v02.api.letsencrypt.org/directory"
        "solvers" = [
#          {
#            "http01" = {
#              "ingress" = {
#                "class" = "nginx"
#              }
#            }
#          },
          {
            "dns01" = {
              "cloudflare" = {
                "apiTokenSecretRef" = {
                  "key"  = "api-token"
                  "name" = "cloudflare-dns-token"
                }
              }
            }
          },
        ]
      }
    }
  }
}