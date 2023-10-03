# Preface

Creating a microservice architecture is always a challenging thing. First you have to analyze carefully the data flows and the work flows. There are many ways how you can implement microservice comminications. But there are two most popular procedures and those are:

* Event driven architecture
* RPC

### Event driven architecture

This architecture requires a message bus. Usually, that is some kind of MQ server or Kafka. In short: The microservices will communicate with each other by pushing/producing a message into the message bus. Other microservices will consume these messages and they will send the result back to the message bus for other services. Also if there is an error in the service the error is also pushed to message bus so the other services will get information about the problem. You can find event-driven demo implementations here:

* C# : https://github.com/pbakota/mt-microservice-kafka
* Java: https://github.com/pbakota/java-microservice-kafka

### RPC

We can say that the event driven architecure uses asyncronus communication, the RPC (Remote procedure call) method uses synchronus. When you work with RPC calls it almost will look like you are working with local methods. However, if you want to use this kind of comminication you have to have prepared *.proto files and generated client/server stubs. Fortunatelly, .NET has an excellent package that will generate for you automatically the client and the server stubs.

# The architecture

![Alt text](https://github.com/pbakota/csharp-microservices-grpc/blob/main/figures/figure-1.svg)

The demo implements RPC communication between microservices.

If you compare this architecture with the architecture that uses Event driven desing, you will find they are very simmilar. For RPC implementation I used gRPC with Protobuffer. https://protobuf.dev/

Except for the communication, other parts are almost exactly the same. I also implemented a simple API gw again with Ocelot. https://github.com/ThreeMammals/Ocelot. Again just to demonstrate how easy is to have an API gw in front of your microservices.

# Implementation

The whole demo project implemented in .NET 6. with EntityFramework Core, Google Protobuf, and Grpc ASP.NET Core. Each microservice has its own project folder. There is shared project called 'Common, which is shared between all microservices and it contains the common structures. There is another important folder and that is 'Proto', this folder contains all the required *.proto files, used for Protobuffer and Grpc tool to generate stubs. 

The serives are:

* *Orders* - Contains project for handling orders
* *Payments* - Contains project for handling payments
* *Stock* - The stock management
* *Delivery* - The delivery management. Currently only acknolege the delivery.
* *ApiGW* - Contains the project for API gw

In the folder 'http' in each project you can find HTTP request used by VSCode plugin `REST Client` https://marketplace.visualstudio.com/items?itemName=humao.rest-client

The microservices use SQLite3 databases. Each of them has its own separate databases, to demonstrate that they are completely independent from each other. 

NOTE: For production, you should always use a proper database system.

Also the project contains all required Docker files for deployment.


# Build & run the project

The project used GNU make files to build and package the services. There are a couple of targets that can be used:

* `$ make` - This will build each microservice
* `$ make run-orders` - This will run (with watch) the Orders microservice
* `$ make run-payments` - This will run (with watch) the Payments microservice
* `$ make run-stock` - This will run (with watch) the Stock microservice
* `$ make run-delivery` - This will run (with watch) the Delivery microservice
* `$ make run-api-gw` - This will run (with watch) the API gateway

To test the whole demo project you have to have all the services up and running.

To package each microservice separately you can use the following targets

`$ make package-all` - Will package all microservices seapratelly into 'release' folder.
`$ make package-orders` - Will package orders microservice
`$ make package-payments` - Will package payments microservice
`$ make package-stock` - Will package stock microservice
`$ make package-delivery` - Will package delivery microservice
`$ make package-api-gw` - Will package API gataway

Please note that the packages are for Linux x64 architecture and they will be deployed in Linux Docker containers.

# Deploying in Docker container

This is the trickiest part. First of all, HTTPS requires an SSL certificate. That I don't own. So, I had to make a workaround. That means the whole deployment is totally insecure and you **should not** use this method in production, use it only for testing and demonstration. For production **always use** a proper certificate issued by an authority.

## Step 1 - Create Self-Signed-Cerificate

You have to create your own Self-Signed-Cerificate. Since you are going to deploy the thing to the Linux docker container you can find the bash script in the folder which will generate for you the certificate. I took it from MS official site: https://learn.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide 
```
# ./make-ssc.sh
```

This script will generate two files. Both of them are needed.
```
contoso.com.crt
contoso.com.key
```
The file `contoso.com.crt` contains the certificate and `contoso.com.key` contains the secret (key) for the certificate.

Please make sure that you did not move to another place, the stack.yaml file expects to have them in place. If you want to move them then do not forget to update the stack.yaml file too.

## Step 2 - Generate DB

Generate the DB files with migrations on your dev machine in the `source` folder with:
```
$ make orders-update-db
$ make payments-update-db
$ make stock-update-db
$ make delivery-update-db
```

After this, you have to copy the generated DBs to your docker container `data` folder.

## Step 3 - Run the stack

Run the stack with the command
```
$ make start-stack
```

Or you can stop it with command
```
$ make stop-stack
```


