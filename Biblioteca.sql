IF DB_ID('Biblioteca') IS NULL
    CREATE DATABASE Biblioteca;
GO

USE Biblioteca;
GO

-- Remove tabelas da versao anterior (se existirem)
IF OBJECT_ID('Emprestimos', 'U') IS NOT NULL DROP TABLE Emprestimos;
IF OBJECT_ID('Livros', 'U') IS NOT NULL DROP TABLE Livros;
IF OBJECT_ID('Usuarios', 'U') IS NOT NULL DROP TABLE Usuarios;
IF OBJECT_ID('Clientes', 'U') IS NOT NULL DROP TABLE Clientes;
GO

CREATE TABLE Livros (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(150) NOT NULL,
    Autor VARCHAR(100) NOT NULL,
    Editora VARCHAR(100) NULL,
    Quantidade INT NOT NULL DEFAULT 1
);

CREATE TABLE Clientes (
    Cpf CHAR(11) PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Endereco VARCHAR(200) NULL,
    Telefone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL
);

CREATE TABLE Emprestimos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LivroId INT NOT NULL REFERENCES Livros(Id),
    ClienteCpf CHAR(11) NOT NULL REFERENCES Clientes(Cpf),
    DataEmprestimo DATETIME NOT NULL DEFAULT GETDATE(),
    DataDevolucao DATETIME NULL
);
GO
