CREATE TABLE IF NOT EXISTS server_settings (
    id BIGINT PRIMARY KEY,
    pin_emoji TEXT NOT NULL,
    unpin_emoji TEXT NOT NULL,
    forums_only BOOLEAN NOT NULL
);
