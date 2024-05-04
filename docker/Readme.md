Install kafka:
``docker-compose up -d
``
install mongo:
``
docker run -it -d --name mongo-container -p 27017:27017 --network mydockernetwork --restart always -v mongodb_data_container:/data/db mongo:latest

``
Download Client Tools – Robo 3T:
https://robomongo.org/download

install sql server
``
docker run -d --name sql-container --network mydockernetwork --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=$tr0ngS@P@ssw0rd02' -e 'MSSQL_PID=Express' -p 1433:1433 mcr.microsoft.com/mssql/server:2017-latest-ubuntu 
``

``
docker run --network mydockernetwork --restart always --platform=linux/amd64 --name RealSQL -e ACCEPT_EULA=1 -e MSSQL_SA_PASSWORD=$tr0ngS@P@ssw0rd02 -p 1433:1433 -d  mcr.microsoft.com/mssql/server:2022-latest
  ``