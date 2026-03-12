# Step 1: Build Angular app
FROM node:20-alpine AS build
ARG CONFIGURATION=development
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration=${CONFIGURATION}

# Step 2: Run with Nginx
FROM nginx:alpine
COPY --from=build /app/dist/Hr.Manager.Web/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
