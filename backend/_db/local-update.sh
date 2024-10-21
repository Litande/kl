#!/bin/bash
winpty docker run -it --network kl --mount type=bind,source="$(pwd)",target=/app --name liquibase-executor --rm gcr.io/kl-manage/liquibase:latest sh -c "cd /app/ && liquibase --url='jdbc:mysql://kl-mysql/sys' --changeLogFile=changelog/db.changelog-master.xml --username=root --password=masterkey --logLevel info update"
