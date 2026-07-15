FROM node:24-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build -- --configuration production

FROM nginx:1.27-alpine AS runtime

RUN rm -rf /usr/share/nginx/html/*

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/frontend /tmp/frontend-dist

RUN if [ -d "/tmp/frontend-dist/browser" ]; then \
      cp -r /tmp/frontend-dist/browser/. /usr/share/nginx/html/; \
    else \
      cp -r /tmp/frontend-dist/. /usr/share/nginx/html/; \
    fi

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]