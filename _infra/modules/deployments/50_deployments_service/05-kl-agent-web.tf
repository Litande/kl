// DEPLOY BRAND GATEWAY PODS
locals {
  web-kl-agent = "kl-agent-web"
}

// DEPLOY BACKOFFICE API DEPLOYMENT
resource "kubernetes_deployment" "dial_agent_web" {
  timeouts {
    create = "60s"
  }

  metadata {
    name      = local.web-kl-agent
    namespace = local.dial_namespace

    labels = {
      app = local.web-kl-agent
    }
  }

  spec {
    replicas = 0

    selector {
      match_labels = {
        app = local.web-kl-agent
      }
    }

    strategy {
      type = "Recreate"
    }

    template {
      metadata {
        labels = {
          app = local.web-kl-agent
        }
      }

      spec {
        container {
          image             = "gcr.io/kl-manage/${local.web-kl-agent}:${var.docker_tag}"
          name              = local.web-kl-agent
          image_pull_policy = "Always"
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

resource "kubernetes_service" "dial_agent_web_service" {
  metadata {
    name      = local.web-kl-agent
    namespace = local.dial_namespace
    annotations = {
      "cloud.google.com/load-balancer-type" = "Internal"
    }
  }
  spec {
    selector = {
      app = kubernetes_deployment.dial_agent_web.spec.0.template.0.metadata[0].labels.app
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

module "dial_agent_web_nginx_ingress" {
  source    = "./modules/ingress"
  host      = var.dial_agent_web_host_name
  name      = local.web-kl-agent
  namespace = local.dial_namespace
}
