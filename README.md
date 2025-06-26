# PlatformWellSync

## Quick Start

1. **Setup environment**
   ```bash
   cp .env.example .env
   ```
   Edit `.env` with your API credentials

2. **Start database**
   ```bash
   docker compose up -d
   ```

3. **Initialize database**
   ```bash
   sh init.sh
   ```

4. **Run the app**
   ```bash
   dotnet run
   ```

## Requirements

- .NET 8
- Docker
- API credentials (username/password)

