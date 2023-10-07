#!/usr/bin/env bash

/opt/mssql/bin/sqlservr &
    sleep 40 #waiting SQL Server to be available
    for script in /scripts/**/*.sql
    do 
        /opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -l 30 -e -i $script
    done
    sleep infinity