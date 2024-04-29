FROM debian:stable-slim AS build
WORKDIR /script
COPY kafka-wait-and-topics.sh kafka-wait-and-topics.sh
RUN chmod +x kafka-wait-and-topics.sh

FROM apache/kafka:3.7.0 as final
WORKDIR /opt/kafka/bin
COPY --from=build kafka-wait-and-topics.sh kafka-wait-and-topics.sh
ENTRYPOINT ["./kafka-wait-and-topics.sh"]