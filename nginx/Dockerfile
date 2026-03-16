FROM nginx:1.28-alpine

# openssl is needed to generate a temporary self-signed certificate on first
# boot, allowing nginx to start before certbot has obtained a real certificate.
RUN apk add --no-cache openssl

# Runs automatically via nginx's /docker-entrypoint.d/ before nginx starts.
COPY init-ssl.sh /docker-entrypoint.d/10-init-ssl.sh
RUN chmod +x /docker-entrypoint.d/10-init-ssl.sh
