#!/bin/bash

#unfortunately they need to be separately maintaned from the usage in code
topics="permissions templates"
MAX_TRIES=50
count=0
kafka_address="dev-kafka-service:30011"

while ! ./kafka-topics.sh --bootstrap-server $kafka_address --list > /dev/null 2>&1 && [[ $count -lt $MAX_TRIES ]]; do
  echo "Waiting for Kafka to start... (Attempt $((count + 1))/$MAX_TRIES)"
  sleep 5
  ((count++))
done

if [[ $count -eq $MAX_TRIES ]]; then
  echo "Error: Couldn't connect to Kafka after $MAX_TRIES tries. Exiting."
  exit 1
fi

echo "Kafka is running. Checking topics..."

old_topics=$(./kafka-topics.sh --bootstrap-server $kafka_address --list)

# Check and create each topic
for topic in $topics; do
  if ! echo $old_topics | grep -q -w "$topic"; then
    echo "Creating topic: $topic"
    ./kafka-topics.sh --bootstrap-server $kafka_address --create --topic "$topic" --partitions 1 --replication-factor 1
  else
    echo "Topic exists: $topic"
  fi
done

# Get existing topics (assuming success)
./kafka-topics.sh --bootstrap-server $kafka_address --list

#kafka-topics.sh --delete --bootstrap-server $kafka_address --topic topic2
