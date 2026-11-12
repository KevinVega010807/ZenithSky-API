# Database

Aquí va `ZenithSky.sql` (Luis): creación de BD, tablas, FKs, UNIQUE y datos de prueba.

El código de la API espera exactamente estos nombres:

| Tabla | Columnas |
|---|---|
| airport | airport_id INT IDENTITY PK, iata CHAR(3) UNIQUE, icao CHAR(4) UNIQUE, name VARCHAR(100) |
| airline | airline_id INT IDENTITY PK, iata CHAR(2), airlinename VARCHAR(100), base_airport INT FK → airport |
| airplane_type | type_id INT IDENTITY PK, identifier VARCHAR(50), description VARCHAR(250) |
| airplane | airplane_id INT IDENTITY PK, capacity INT CHECK (> 0), type_id INT FK → airplane_type, airline_id INT FK → airline |
