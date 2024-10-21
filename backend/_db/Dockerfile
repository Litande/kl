FROM liquibase/liquibase:4.7
RUN lpm add mysql --global
ADD changelog /app/changelog
WORKDIR /app
