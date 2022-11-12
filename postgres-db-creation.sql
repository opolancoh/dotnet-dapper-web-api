REVOKE CONNECT ON DATABASE books_dapper_db FROM PUBLIC;

SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE datname = 'books_dapper_db';

DROP DATABASE books_dapper_db;

CREATE DATABASE books_dapper_db;
