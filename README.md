# fsharp-postgres-pusher
A service to push data to PostgreSQL... written in F#

Spin up PostgreSQL in Docker with https://github.com/anaerobic/docker-postgres-test

Add a foo table using:
```sql
CREATE TABLE public.foo
(
   id serial, 
   some_json json
) 
WITH (
  OIDS = FALSE
)
;
```

Push some JSON into PostgreSQL using:
```
docker run -i --rm --net host fsharp-postgres-pusher /s <ip_of_postgres_host> /p 5432 /pw LifeTime1 /db docker /tbl foo /col some_json

{ "fsharp": "is awesome" }
```

Or stream it from https://github.com/anaerobic/fsharp-kafka-simple using:
```
docker run -i --rm --net host fsharp-kafka-consumer results http://kafka.lacolhost.com:9092 | docker run -i --rm --net host fsharp-postgres-pusher /s <ip_of_postgres_host> /p 5432 /pw LifeTime1 /db docker /tbl foo /col some_json
```
