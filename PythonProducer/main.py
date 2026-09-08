from producer import KafkaProducer
from load_data import json_reader

def main(topic, data_path):
    producer = KafkaProducer(bootstrap_servers="kafka:9092",topic=topic)
    producer.send_from_reader(json_reader, data_path)

if __name__ == "__main__":
    main("reports", "data/field_reports.json")
