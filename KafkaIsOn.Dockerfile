FROM busybox:1.36 AS base

FROM apache/kafka:3.7.0 AS build
WORKDIR /

FROM base as final
WORKDIR /script
COPY --from=build /opt/kafka/bin/kafka-topics.sh kafka-topics.sh
COPY kafka-wait-and-topics.sh kafka-wait-and-topics.sh
RUN chmod +x kafka-wait-and-topics.sh
ENTRYPOINT ["./kafka-wait-and-topics.sh"]