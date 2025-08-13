# Local Development

## Prerequisites

- Docker Desktop (or compatible)
- .NET SDK (per `src/global.json`)
- Node 20+ (for `TossErp.Web`)
- Flutter SDK (optional for mobile)

## One-shot bring-up

```bash
docker compose -f infra/docker/docker-compose.yml up -d --build
```

Services:

- Gateway: http://localhost:8080
- Web (Nuxt dev): http://localhost:3000
- Inventory API (Stock): http://localhost:5001 (container internal 8080)
- Postgres: localhost:5432 (user/pass in `.env`)
- Redis: localhost:6379
- RabbitMQ: http://localhost:15672 (guest/guest)

## Nuxt Web app (if running outside compose)

```bash
cd TossErp.Web
npm ci
npm run dev
```

## Notes

- The Inventory service reuses existing implementation under `src/Services/Stock`.
- Other services are scaffolded as stubs and can be iterated later.



