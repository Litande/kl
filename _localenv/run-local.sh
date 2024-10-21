#!/bin/bash

docker network create --driver bridge kl || true

docker-compose --env-file ./config/.env.qa -f docker-compose.yaml up -d