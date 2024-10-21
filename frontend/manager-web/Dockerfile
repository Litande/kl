FROM node:16-slim as build

ARG GITHUB_TOKEN
WORKDIR /app
COPY ./.env.local ./.env
COPY ./package.json ./
COPY ./package-lock.json ./

RUN npm set @plat4me:registry https://npm.pkg.github.com && npm set //npm.pkg.github.com/:_authToken=$GITHUB_TOKEN && npm ci
COPY ./ ./

RUN npm run build


FROM nginx:mainline-alpine-perl

ARG BRANCH_NAME=""
ENV REACT_APP_BRANCH_NAME=$BRANCH_NAME

LABEL BRANCH_NAME = $BRANCH_NAME
COPY --from=build /app/build/ /var/www/html/public/
COPY nginx.conf /etc/nginx/nginx.conf
