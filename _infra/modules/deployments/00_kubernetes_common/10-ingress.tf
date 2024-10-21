resource "google_compute_address" "platform_external_ip" {
  name   = "${var.project_name}-external-ip"
  region = var.project_region
}


resource "helm_release" "brand_gateway_helm_nginx_ingress" {
  name             = "main-ingress"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  namespace        = "ingress-nginx"
  version          = "4.0.18"
  create_namespace = true
  wait             = "false"
  timeout          = "30"

  set {
    name  = "controller.service.loadBalancerIP"
    value = google_compute_address.platform_external_ip.address
  }

  set {
    name  = "controller.config.use-forwarded-headers"
    value = true
  }

  set {
    name  = "controller.admissionWebhooks.enabled"
    value = false
  }

  set {
    name  = "tcp.7790"
    value = "default/kl-core-sip-bridge:7790"
  }

  set {
    name  = "controller.service.loadBalancerSourceRanges"
    value = "{${var.ingress_source_ips}}"
  }

}
