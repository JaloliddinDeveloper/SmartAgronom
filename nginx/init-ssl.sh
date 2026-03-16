#!/bin/sh
# Generates a temporary self-signed certificate if no real cert exists yet.
# This lets nginx start even before certbot has run for the first time.
# Once certbot obtains a real Let's Encrypt certificate, run: nginx -s reload

DOMAIN="${NGINX_DOMAIN:-localhost}"
CERT_DIR="/etc/letsencrypt/live/${DOMAIN}"

if [ ! -f "${CERT_DIR}/fullchain.pem" ]; then
    echo "[init-ssl] No TLS cert found for '${DOMAIN}' — creating temporary self-signed cert..."
    mkdir -p "${CERT_DIR}"
    openssl req -x509 -nodes -newkey rsa:2048 \
        -keyout "${CERT_DIR}/privkey.pem" \
        -out    "${CERT_DIR}/fullchain.pem" \
        -days 1 -subj "/CN=${DOMAIN}" 2>/dev/null
    echo "[init-ssl] Done. Run certbot to replace with a real Let's Encrypt certificate."
fi
