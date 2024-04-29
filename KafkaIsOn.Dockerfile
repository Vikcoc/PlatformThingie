FROM apache/kafka:3.7.0
WORKDIR /opt/kafka/bin
COPY kafka-wait-and-topics.sh kafka-wait-and-topics.sh
#RUN chmod +x kafka-wait-and-topics.sh
ENTRYPOINT ["./kafka-wait-and-topics.sh"]