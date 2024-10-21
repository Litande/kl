// DEPLOY NATS CLUSTER
resource "helm_release" "system_nats_cluster" {
  name       = "kl-nats"
  namespace  = "default"
  repository = "https://nats-io.github.io/k8s/helm/charts/"
  chart      = "nats"
  wait       = "false"
  version    = "0.8.4"
  timeout    = "30"

  set {
    name  = "cluster.enabled"
    value = true
  }

  set {
    name  = "natsbox.enabled"
    value = false
  }

  set {
    name  = "nats.limits.maxPayload"
    value = "5Mb"
  }
}
