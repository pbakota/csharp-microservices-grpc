vim: ts=8 sts=8 sw=8 noet ai

.PHONY: protobuf
all: build

build:
	dotnet build -c Debug

clean:
	dotnet clean

run-orders:
	dotnet watch run -c Debug --project Orders/Orders.csproj

run-payment:
	dotnet watch run -c Debug --project Payments/Payments.csproj

run-stock:
	dotnet watch run -c Debug --project Stock/Stock.csproj

run-delivery:
	dotnet watch run -c Debug --project Delivery/Delivery.csproj

run-api-gw:
	dotnet watch run -c Debug --project ApiGW/ApiGW.csproj

add-migration:
	@echo dotnet ef migrations add Initial --project Orders/Orders.csproj

orders-update-db:
	dotnet ef database update --project=Orders/Orders.csproj

payments-update-db:
	dotnet ef database update --project=Payments/Payments.csproj

stock-update-db:
	dotnet ef database update --project=Stock/Stock.csproj

delivery-update-db:
	dotnet ef database update --project Delivery/Delivery.csproj

package-orders:
	rm -rf ../releases/linux-x64 && dotnet publish -f "net6.0" -c Release Orders/Orders.csproj -r linux-x64 --output ../releases/linux-x64 --no-self-contained && \
        cd ../releases/linux-x64 && tar -zcf ../orders-linux-x64.tgz * && cd -

package-payments:
	rm -rf ../releases/linux-x64 && dotnet publish -f "net6.0" -c Release Payments/Payments.csproj -r linux-x64 --output ../releases/linux-x64 --no-self-contained && \
        cd ../releases/linux-x64 && tar -zcf ../payments-linux-x64.tgz * && cd -

package-stock:
	rm -rf ../releases/linux-x64 && dotnet publish -f "net6.0" -c Release Stock/Stock.csproj -r linux-x64 --output ../releases/linux-x64 --no-self-contained && \
        cd ../releases/linux-x64 && tar -zcf ../stock-linux-x64.tgz * && cd -

package-delivery:
	rm -rf ../releases/linux-x64 && dotnet publish -f "net6.0" -c Release Delivery/Delivery.csproj -r linux-x64 --output ../releases/linux-x64 --no-self-contained && \
        cd ../releases/linux-x64 && tar -zcf ../delivery-linux-x64.tgz * && cd -

package-api-gw:
	rm -rf ../releases/linux-x64 && dotnet publish -f "net6.0" -c Release ApiGW/ApiGW.csproj -r linux-x64 --output ../releases/linux-x64 --no-self-contained && \
        cd ../releases/linux-x64 && tar -zcf ../api-gw-linux-x64.tgz * && cd -

package-all:
	make package-orders && make package-payments && make package-stock && make package-delivery && make package-api-gw

