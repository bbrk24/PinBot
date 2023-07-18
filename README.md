# PinBot

A C\# Discord bot for allowing users to manage pins in their threads.

Use 📌 (`:pushpin:`) to pin a message, and 🚫 (`:no_entry_sign:`) to unpin a message.

## Environment variables

This bot uses environment variables for config:

- `TOKEN`: The Discord API token.
- `CONNECTION_STRING`: The Npgsql connection string.
- `LOG_LEVEL` (optional): The level to set the logs to.
- `GUILD` (optional): If set, only registers commands to that guild rather than globally.

## Migrations

Example migrations.json

```json
{
    "migrationPattern": "./migrations/*",
    "driver": "pg",
    "host": "localhost",
    "port": 5432,
    "database": "pinbot",
    "username": "postgres",
    "password": "password"
}
```
