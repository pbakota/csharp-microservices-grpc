# vim: ts=4 sw=4 sts=4 noet ai

all: build

build:
	make build-orders && make build-payments && make build-stock && make build-delivery && make build-gw

build-orders:
	cd orders && docker build --progress=plain --no-cache . --tag=grpc-microservice-orders:dev

build-payments:
	cd payments && docker build --progress=plain --no-cache . --tag=grpc-microservice-payments:dev

build-stock:
	cd stock && docker build --progress=plain --no-cache . --tag=grpc-microservice-stock:dev

build-delivery:
	cd delivery && docker build --progress=plain --no-cache . --tag=grpc-microservice-delivery:dev

build-gw:
	cd gw && docker build --progress=plain --no-cache . --tag=grpc-microservice-gw:dev

run-orders:
	docker run --rm --name orders-service -v ./data:/app/db -e ASPNETCORE_URLS=http://*:5000 -p 5001:5000 grpc-microservice-orders:dev

run-payments:
	docker run --rm --name payments-service -v ./data:/app/db -e ASPNETCORE_URLS=http://*:5000 -p 5002:5000 grpc-microservice-payments:dev

run-stock:
	docker run --rm --name stock-service -v ./data:/app/db -e ASPNETCORE_URLS=http://*:5000 -p 5003:5000 grpc-microservice-stock:dev

run-delivery:
	docker run --rm --name delivery-service -v ./data:/app/db -e ASPNETCORE_URLS=http://*:5000 -p 5004:5000 grpc-microservice-delivery:dev

run-api-gw:
	docker run --rm --name api-gw-service -v ./config/ocelot.json:/app/ocelot.json -e ASPNETCORE_URLS=http://*:5000 -p 5161:5000 grpc-microservice-gw:dev

start-stack:
	docker compose -f stack.yaml up --remove-orphans -d

stop-stack:
	docker compose -f stack.yaml down


