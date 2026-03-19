CREATE DATABASE IF NOT EXISTS fundicao;
USE fundicao;

CREATE TABLE empresa (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cidade VARCHAR(100)
);

CREATE TABLE categoria (
    id INT AUTO_INCREMENT PRIMARY KEY,
    setor VARCHAR(100) NOT NULL
);

CREATE TABLE macho (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL
);

CREATE TABLE molde (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    codigo VARCHAR(50) NOT NULL,
    prateleira VARCHAR(50),
    empresa_id INT,
    categoria_id INT,
    FOREIGN KEY (empresa_id) REFERENCES empresa(id),
    FOREIGN KEY (categoria_id) REFERENCES categoria(id)
);

CREATE TABLE molde_usa_macho (
    id INT AUTO_INCREMENT PRIMARY KEY,
    molde_id INT NOT NULL,
    macho_id INT NOT NULL,
    FOREIGN KEY (molde_id) REFERENCES molde(id),
    FOREIGN KEY (macho_id) REFERENCES macho(id)
);

CREATE TABLE estado (
    id INT AUTO_INCREMENT PRIMARY KEY,
    estado_atual VARCHAR(50) NOT NULL,
    data_entrada DATE,
    data_saida DATE,
    molde_id INT NOT NULL,
    FOREIGN KEY (molde_id) REFERENCES molde(id)
);