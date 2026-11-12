-- 1. Crear la base de datos
CREATE DATABASE ZenithSkyLKJ;
GO
USE ZenithSkyLKJ;
GO

-- 2. Aeropuertos
CREATE TABLE airport (
    airport_id INT IDENTITY(1,1) PRIMARY KEY,
    iata CHAR(3) NOT NULL,
    icao CHAR(4) NOT NULL,
    name VARCHAR(100) NOT NULL,
    CONSTRAINT UQ_airport_iata UNIQUE (iata),
    CONSTRAINT UQ_airport_icao UNIQUE (icao)
);

-- 3. Aerolíneas (apunta a airport)
CREATE TABLE airline (
    airline_id INT IDENTITY(1,1) PRIMARY KEY,
    iata CHAR(2) NOT NULL,
    airlinename VARCHAR(100) NOT NULL,
    base_airport INT NOT NULL,
    CONSTRAINT FK_airline_airport
        FOREIGN KEY (base_airport) REFERENCES airport(airport_id)
);

-- 4. Tipos de avión
CREATE TABLE airplane_type (
    type_id INT IDENTITY(1,1) PRIMARY KEY,
    identifier VARCHAR(50) NOT NULL,
    description VARCHAR(250) NULL
);

-- 5. Aviones (apunta a airplane_type y airline)
CREATE TABLE airplane (
    airplane_id INT IDENTITY(1,1) PRIMARY KEY,
    capacity INT NOT NULL,
    type_id INT NOT NULL,
    airline_id INT NOT NULL,
    CONSTRAINT CK_airplane_capacity CHECK (capacity > 0),
    CONSTRAINT FK_airplane_type
        FOREIGN KEY (type_id) REFERENCES airplane_type(type_id),
    CONSTRAINT FK_airplane_airline
        FOREIGN KEY (airline_id) REFERENCES airline(airline_id)
);
GO

-- 6. Datos de prueba: 5 aeropuertos
INSERT INTO airport (iata, icao, name) VALUES
('SJO', 'MROC', 'Aeropuerto Internacional Juan Santamaría'),
('LIR', 'MRLB', 'Aeropuerto Internacional Daniel Oduber Quirós'),
('LAX', 'KLAX', 'Los Angeles International Airport'),
('MAD', 'LEMD', 'Aeropuerto Adolfo Suárez Madrid-Barajas'),
('JFK', 'KJFK', 'John F. Kennedy International Airport');

-- 4 aerolíneas
INSERT INTO airline (iata, airlinename, base_airport) VALUES
('ZS', 'ZenithSky Airlines', 1),
('AA', 'American Airlines', 3),
('IB', 'Iberia', 4),
('LR', 'Avianca Costa Rica', 2);

-- 5 tipos de avión
INSERT INTO airplane_type (identifier, description) VALUES
('B737-800', 'Boeing 737-800, avión de pasillo único para rutas medias'),
('A320', 'Airbus A320, avión de pasillo único para rutas cortas y medias'),
('A321', 'Airbus A321, versión alargada del A320'),
('B787-9', 'Boeing 787-9 Dreamliner, avión de pasillo doble para rutas largas'),
('E190', 'Embraer E190, avión regional de pasillo único');

-- 8 aviones
INSERT INTO airplane (capacity, type_id, airline_id) VALUES
(186, 1, 1),
(180, 2, 1),
(150, 2, 2),
(172, 1, 2),
(220, 3, 3),
(290, 4, 3),
(100, 5, 4),
(180, 2, 4);
GO

--Pruebas 

SELECT COUNT(*) FROM airport;        -- da 5
SELECT COUNT(*) FROM airline;        -- da 4
SELECT COUNT(*) FROM airplane_type;  -- da 5
SELECT COUNT(*) FROM airplane;       -- da 8