FROM node as node
WORKDIR /app
COPY ./package.json /app/
RUN npm install
COPY ClientApp/ /app/
RUN npm run build

FROM nginx:1.13
COPY --from=node /app/dist/ /usr/share/nginx/html
COPY ./nginx-custom.conf /etc/nginx/conf.d/default.conf
EXPOSE 80