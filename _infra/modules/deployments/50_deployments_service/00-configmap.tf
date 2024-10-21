// DEPLOY BRAND GATEWAY PODS
locals {
  dial_shared_configmap = "kl-shared-configmap"
  dial_namespace        = "default"
  #  dial_redis_endpoint   = "tp-redis-cluster"
  dial_redis_endpoint = "kl-redis-master"
}

variable "wss_path" {
  default = "/ws"
  type    = string
}

resource "kubernetes_config_map" "dial_shared_configmap" {
  metadata {
    name      = local.dial_shared_configmap
    namespace = local.dial_namespace
  }

  data = {
    ASPNETCORE_ENVIRONMENT = "qa"

    STORAGE__DRIVER             = var.storage_type
    BUCKET_PROJECT_ID           = var.project_id
    STORAGE__GOOGLE__BUCKETNAME = var.google_cloud_storage_bucket

    CLIENTS__REDIS__HOST             = local.dial_redis_endpoint
    CLIENTS__REDIS__PORT             = "6379"
    CLIENTS__REDIS__NON_CLUSTER_MODE = 1

    CLIENTS__NATS__HOST = "kl-nats"
    CLIENTS__NATS__PORT = 4222

    NATS__NATSSTREAMINGCLUSTERID = "stan"

    SIPOptions__Provider   = var.sip_provider
    SIPOptions__Username   = var.sip_user_name
    SIPOptions__Password   = var.sip_password
    SIPOptions__ExternalIP = var.external_ip

    SessionOptions__AccessUrl    = "wss://${var.sip_bridge_hostname}${var.wss_path}"
    RTCOptions__IceCandidates__0 = var.sip_bridge_hostname

    RTCOptions__WebSocketPath = var.wss_path

    RTCOptions__RTPPortRangeStart = 40000
    RTCOptions__RTPPortRangeEnd   = 40200

    SIPOptions__RTPPortRangeStart = 41000

    SIPOptions__RTPPortRangeEnd  = 41100
    SIPOptions__UDPTransportPort = var.sip_transport_port
  }
}

