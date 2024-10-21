#!/bin/bash


docker-compose --env-file ./config/.env.qa -f docker-compose.yaml down
docker network rm kl || true
