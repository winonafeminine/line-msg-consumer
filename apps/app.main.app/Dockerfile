FROM node:20-alpine3.17 AS builder

ARG CRM_APP_ENV_FILE=""
# Set working directory
WORKDIR /app
# Copy file endwith .json
COPY package*.json /app/
# install node modules and build assets
RUN npm install
# copy all files into image
COPY ./ /app/

# remove the development env for avoid the conflict
# copy the compile args (to be able to switch btw dev and prod)
# build the app
# RUN rm -rf .env .env.local && \
#     cp ./apps/app.main.app/${CRM_APP_ENV_FILE} ./apps/app.main.app/.env && \
#     npx nx run app.main.app:build
RUN npx nx run app.main.app:build

FROM nginx:1.25.1-alpine 

RUN apk update && \
    apk add --no-cache tzdata

ENV TZ=Asia/Bangkok

# delete all files inside /usr/share/nginx/html/ and in /etc/nginx/conf.d/
RUN rm -rf /usr/share/nginx/html/* && \
    rm -rf /etc/nginx/conf.d/*

COPY --from=builder /app/dist/apps/app.main.app /usr/share/nginx/html

COPY ./apps/app.main.app/nginx/nginx.conf /etc/nginx/conf.d

# Copy .env file and shell script to container
WORKDIR /usr/share/nginx/html


# need to expose to port 80
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]