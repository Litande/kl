FROM docker.io/bitnami/redis:6.2.7-debian-11-r9
RUN mkdir /opt/bitnami/redis/lib/
COPY --from=redis/redis-stack-server:6.2.6-v4 /opt/redis-stack/lib/redisearch.so /opt/redis-stack/lib/rejson.so /opt/bitnami/redis/lib/
RUN echo $'loadmodule /opt/bitnami/redis/lib/redisearch.so \n\
loadmodule /opt/bitnami/redis/lib/rejson.so' >> /opt/bitnami/redis/etc/redis.conf