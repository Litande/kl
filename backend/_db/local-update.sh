#!/bin/bash

# Build the Docker image
docker build -t kl-db .

# Check if a container with the name 'trad-db' is already running and remove it
if [ "$(docker ps -aq -f name=kl-db)" ]; then
    echo "Stopping and removing existing 'kl-db' container..."
    docker rm -f kl-db
fi
# Define schema name (you can modify this as needed)
SCHEMA_NAME="migrations"

# Run the Docker container with --rm to automatically remove it after completion
docker run --rm --name kl-db --network host -p 3306:3306 kl-db bash -c "liquibase \
--liquibase-schema-name=$SCHEMA_NAME \
--database-changelog-lock-table-name=kl_lock \
--database-changelog-table-name=kl_changelog \
--headless=true \
--url=jdbc:mysql://localhost:3306/ \
--username=root --password=masterkey \
--changelog-file=changelog/db.changelog-master.xml \
updateSQL"

# Create schema and apply Liquibase migrations
docker run --rm --name kl-db --network host -p 3306:3306 kl-db bash -c "liquibase \
--liquibase-schema-name=$SCHEMA_NAME \
--database-changelog-lock-table-name=kl_lock \
--database-changelog-table-name=kl_changelog \
--headless=true \
--url=jdbc:mysql://localhost:3306/ \
--username=root --password=masterkey \
--changelog-file=changelog/db.changelog-master.xml \
update"