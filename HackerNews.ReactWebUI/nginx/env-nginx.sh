#!/bin/bash

/bin/bash ./env.sh $1
envsubst '$${UI_PORT} $${SERVER_NAME}' < /etc/nginx/templates/nginx.conf.template > ./nginx.conf
echo "Starting nginx web server on port: '${UI_PORT}' and server name: '${SERVER_NAME}' ..."
exec "${@:2}"
