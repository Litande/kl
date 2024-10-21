#resource "kubernetes_service" "tp_redis" {
#  metadata {
#    name = "tp-redis"
#
#    labels = {
#      app = "tp-redis"
#    }
#    annotations = {
#      "cloud.google.com/neg" = "{\"ingress\":true}"
#    }
#  }
#
#  spec {
#    port {
#      name        = "tcp-redis"
#      protocol    = "TCP"
#      port        = 6379
#      target_port = "tcp-redis"
#    }
#
#    port {
#      name        = "tcp-redis-bus"
#      protocol    = "TCP"
#      port        = 16379
#      target_port = "tcp-redis-bus"
#    }
#
#
#    selector = {
#      "app.kubernetes.io/instance" = "tp-redis"
#    }
#
#  }
#}

resource "helm_release" "bitnami_redis_cluster" {
  name       = "tp"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "redis-cluster"
  namespace  = "default"
  version    = "7.6.4"
  wait       = "false"
  timeout    = "30"

  set {
    name  = "global.imagePullSecrets[0]"
    value = "registry-ro-secret"
  }
  set {
    name  = "image.registry"
    value = var.redis_image_registry
  }

  set {
    name  = "image.repository"
    value = var.redis_image_repo
  }

  set {
    name  = "image.tag"
    value = var.redis_image_tag
  }

  set {
    name  = "usePassword"
    value = false
  }

  set {
    name  = "tls.authClients"
    value = false
  }

  set {
    name  = "cluster.nodes"
    value = 3
  }

  set {
    name  = "cluster.replicas"
    value = 0
  }

  # set {
  #   name  = "redis.nodeAffinityPreset.type"
  #   value = "hard"
  # }
  # set {
  #   name  = "redis.nodeAffinityPreset.key"
  #   value = "type"
  # }
  # set {
  #   name  = "redis.nodeAffinityPreset.values"
  #   value = "regular"
  # }
}
resource "helm_release" "bitnami_redis_single" {
  name       = "dial"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "redis"
  namespace  = "default"
  version    = "17.8.2"
  wait       = "false"
  timeout    = "30"

  set {
    name  = "architecture"
    value = "standalone"
  }
  set {
    name  = "global.imagePullSecrets[0]"
    value = "registry-ro-secret"
  }
  set {
    name  = "image.registry"
    value = var.redis_image_registry
  }

  set {
    name  = "image.repository"
    value = "kl-manage/cnv-redis"
  }

  set {
    name  = "image.tag"
    value = "6.2.7-debian-11-r9"
  }

  set {
    name  = "usePassword"
    value = false
  }

  set {
    name  = "auth.enabled"
    value = "false"
  }

  set {
    name  = "tls.authClients"
    value = false
  }

  #  set {
  #    name  = "cluster.nodes"
  #    value = 3
  #  }
  #
  #  set {
  #    name  = "replica.replicaCount"
  #    value = 1
  #  }

  # set {
  #   name  = "redis.nodeAffinityPreset.type"
  #   value = "hard"
  # }
  # set {
  #   name  = "redis.nodeAffinityPreset.key"
  #   value = "type"
  # }
  # set {
  #   name  = "redis.nodeAffinityPreset.values"
  #   value = "regular"
  # }
}
