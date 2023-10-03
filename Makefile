all: copy-all

copy-all:
	make copy-orders && make copy-payments && make copy-stock && make copy-delivery && make copy-gw

copy-orders:
	rsync -a releases/orders-linux-x64.tgz root@playbox:/root/csharp-grpc-microservices-docker/orders
copy-payments:
	rsync -a releases/payments-linux-x64.tgz root@playbox:/root/csharp-grpc-microservices-docker/payments
copy-stock:
	rsync -a releases/stock-linux-x64.tgz root@playbox:/root/csharp-grpc-microservices-docker/stock
copy-delivery:
	rsync -a releases/delivery-linux-x64.tgz root@playbox:/root/csharp-grpc-microservices-docker/delivery
copy-gw:
	rsync -a releases/api-gw-linux-x64.tgz root@playbox:/root/csharp-grpc-microservices-docker/gw

