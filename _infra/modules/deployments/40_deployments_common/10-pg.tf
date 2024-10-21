resource "random_password" "grafana-admin" {
  length  = 16
  special = false
}

resource "helm_release" "prometheus_community" {
  name             = "prometheus"
  repository       = "https://prometheus-community.github.io/helm-charts"
  chart            = "kube-prometheus-stack"
  namespace        = "prometheus"
  create_namespace = true
  wait             = "false"
  timeout          = "30"

  set {
    name  = "grafana.adminPassword"
    value = random_password.grafana-admin.result
  }
  set {
    name  = "namespaceOverride"
    value = "prometheus"
  }

  set {
    name  = "grafana.ingress.enabled"
    value = true
  }

  set {
    name  = "grafana.ingress.ingressClassName"
    value = "nginx"
  }

  set {
    name  = "grafana.ingress.hosts"
    value = "{${var.grafana_domain}}"
  }
  set {
    name  = "grafana.ingress.annotations.cert-manager\\.io/cluster-issuer"
    value = "letsencrypt-prod"
  }
  set {
    name  = "grafana.ingress.tls[0].secretName"
    value = "${var.grafana_domain}-tls"
  }
  set {
    name  = "grafana.ingress.tls[0].hosts"
    value = "{${var.grafana_domain}}"
  }

}
