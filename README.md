# tool-system
system focused on registering and querying foundry tools

## Comandos Essenciais (Docker)

Abra o terminal nesta pasta raiz e utilize os comandos abaixo para controlar o servidor:

- **Ligar o servidor (em segundo plano):** `docker-compose up -d`
- **Ligar e forçar a recompilação do código:** `docker-compose up -d --build`
- **Desligar o servidor de forma segura:** `docker-compose down`
- **Desligar e APAGAR o banco de dados (Reset Total):** `docker-compose down -v`
- **Acompanhar as mensagens e erros (Logs):** `docker-compose logs -f`  