output "dial_stun_proxy" {
  value = kubernetes_service_v1.dial_stun_service.status.0.load_balancer.0.ingress.0.ip
}