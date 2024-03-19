# Stage 1: Build the React Application
FROM node:18-alpine as build
WORKDIR /app
COPY ./package.json .
RUN npm i
COPY . .
RUN npm run build

# Stage 2: Setup the Nginx Server to serve the React Application
FROM nginx:1.24.0-alpine as production

# Default values for known public environment variables
ENV UI_PORT=8080
ENV UI_ALT_PORT=8000
ENV SERVER_NAME=localhost
ENV UI_HOST_PORT=8080

RUN apk add --no-cache bash
COPY --from=build /app/dist /usr/share/nginx/html

WORKDIR /etc/nginx

# Setup nginx main configuration template file and script to substitute env variables
COPY ./nginx/nginx.conf.template ./templates/
COPY ./nginx/env-nginx.sh .

# Copy over the main website environment variables substitute script and default environment variables
COPY ./env.sh .
COPY ./.env .

# Ensure scripts are executable
RUN chmod +x env.sh
RUN chmod +x env-nginx.sh

EXPOSE $UI_PORT
EXPOSE $UI_ALT_PORT

ENTRYPOINT ["./env-nginx.sh", "/usr/share/nginx/html"]
CMD ["nginx", "-g", "daemon off;"]
