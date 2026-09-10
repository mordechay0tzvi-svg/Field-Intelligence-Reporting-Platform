from confluent_kafka import Producer
import json
class KafkaProducer:

    def __init__(self, bootstrap_servers, topic):
        self.topic = topic
        self.producer = Producer({"bootstrap.servers": bootstrap_servers})

    def send_from_reader(self, loading_method, path):
        data = loading_method(path)
        for item in data:
            self.producer.produce(topic=self.topic,value=json.dumps(item))
            self.producer.poll(0)
        self.producer.flush()
