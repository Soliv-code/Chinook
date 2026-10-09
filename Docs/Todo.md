#  Chinook Project TODO

Этот документ отражает текущий статус, историю и план развития проекта Chinook API (.NET 10, Clean Architecture, Dapper).

## ✅ Выполнено (Готово)

### 🏗️ Архитектура и Инфраструктура
- [x] **Инициализация решения**: Создан Solution `Chinook` в Visual Studio 2026 (.NET 10).
- [x] **Архитектура**: Реализована Clean Architecture из 4-х проектов:
  - `Chinook.Domain` (только сущности)
  - `Chinook.Application` (интерфейсы и контракты)
  - `Chinook.Infrastructure` (реализации, Dapper, Npgsql)
  - `Chinook.API` (контроллеры, DI, Program.cs)
- [x] **Git**: Репозиторий инициализирован, ветка по умолчанию — `main`. Ветка `feature/artist-repository` успешно слита.
- [x] **Инфраструктура (Docker)**:
  - Настроен `Docker/docker-compose.yml`.
  - Поднят PostgreSQL 17 с расширением `pgvector` (`pgvector/pgvector:pg17`).
  - Настроен современный `init.sql` с `GENERATED ALWAYS AS IDENTITY`.
  - Настроен `Dockerfile` для multi-stage сборки API.

### 🛠️ Инструменты и Исследование
- [x] **Инструменты разработки**:
  - DBeaver: подключен к локальной БД, схема проверена.
  - Bruno: коллекция API (`BrunoCollection`) создана внутри проекта, все CRUD-операции протестированы.
- [x] **SQL-исследование (DBeaver)**:
  - Изучена бизнес-логика схемы Chinook (цифровой магазин музыки).
  - Освоены преобразования типов, работа с `NULL` (`COALESCE`, `CASE WHEN`) и форматирование времени.
  - Освоены Raw String Literals (`"""`) для чистого SQL в C#.

### 🎯 Этап 1: Базовый CRUD на Dapper (Завершён)
- [x] Добавлены NuGet-пакеты в `Chinook.Infrastructure`: `Dapper` и `Npgsql`.
- [x] Реализован `ArtistRepository` (используя `NpgsqlDataSource`, параметризованные запросы, `RETURNING`).
- [x] Настроена регистрация зависимостей (DI) в `Program.cs`.
- [x] Создан `ArtistsController` с полным набором REST-эндпоинтов (GET, POST, PUT, DELETE).
- [x] Реализована безопасная серверная сортировка через query-параметры (`?sortBy=name`).
- [x] Протестированы все сценарии в Bruno (включая обработку 404 и 204 статусов).

---

## 📁 Структура проекта

```text
Chinook/
├── BrunoCollection/
│   └── Chinook API/
│       ├── Artists/ (Create, Delete, GetAll, GetById, Update)
│       └── Tests/
── Chinook.API/
│   ├── Controllers/ (ArtistsController, PingController)
│   ├── Properties/launchSettings.json
│   ├── appsettings.json
│   ├── Dockerfile
│   └── Program.cs
├── Chinook.Application/
│   └── Interfaces/IArtistRepository.cs
├── Chinook.Domain/
│   └── Entities/Artist.cs
├── Chinook.Infrastructure/
│   └── Repositories/ArtistRepository.cs
├── Docker/
│   ├── SQL/init.sql
│   └── docker-compose.yml
├── Docs/
│   ├── ProjectStructure.md
│   └── Todo.md
└── Chinook.slnx
```

---

##  Следующие шаги

### Этап 2: Кэширование (Redis)
- [ ] Добавить сервис `redis` в `docker-compose.yml`.
- [ ] Добавить NuGet-пакет `StackExchange.Redis` в `Infrastructure`.
- [ ] Создать интерфейс `ICacheService` в `Application` и реализацию `RedisCacheService` в `Infrastructure`.
- [ ] Реализовать паттерн Cache-Aside (например, для `GetAllArtists`).
- [ ] Протестировать в Bruno (первый запрос медленный, второй мгновенный).

### Этап 3: Авторизация и безопасность (Keycloak)
- [ ] Добавить сервис `keycloak` в `docker-compose.yml`.
- [ ] Настроить аутентификацию по JWT Bearer токенам в `Chinook.API`.
- [ ] Добавить роли (например, `Customer` и `Employee`).
- [ ] Закрыть эндпоинты атрибутом `[Authorize]` и настроить политику доступа.

### Этап 4: Продвинутые фичи и связи
- [ ] Реализовать CRUD для `Album` и `Track`.
- [ ] Написать сложный запрос с `JOIN` (например, получить Альбом со списком Треков в одном запросе через Dapper).
- [ ] Добавить векторный поиск (`pgvector`): поиск треков по семантическому описанию.
- [ ] Добавить фоновую обработку событий (RabbitMQ/Kafka): отправка email-чека при создании `Invoice`.

### 🧠 Этап 5: Мониторинг и Профилирование SQL (Идеи)
- [ ] **Логирование запросов**: Настроить уровень логирования `"Npgsql": "Debug"` в `appsettings.json` для вывода чистого SQL в консоль (полная замена прозрачности EF Core).
- [ ] **EXPLAIN ANALYZE**: Реализовать возможность получения плана выполнения запроса (например, через специальный отладочный эндпоинт `GET /api/.../debug-plan`).
- [ ] **Анализ BUFFERS**: Использовать `EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)` для мониторинга чтений с диска и из кеша PostgreSQL.
- [ ] **Автоматические варнинги**: Реализовать парсинг вывода `EXPLAIN` в режиме разработки (Development) для генерации предупреждений или ошибок при обнаружении `Seq Scan` на больших таблицах.

---

##  Заметки и правила проекта
1. **Источник истины**: Структура БД определяется только через `Docker/SQL/init.sql`. Любые изменения в DBeaver должны быть немедленно перенесены в этот файл.
2. **Docker Workflow**: Для локальной разработки API запускается через Visual Studio (F5). Пересборка Docker-образа API (`docker-compose up -d --build`) делается только для финальной проверки.
3. **База данных**: Никогда не использовать `docker-compose down -v` без крайней необходимости (это удаляет все данные).
4. **Dapper & Postgres**: 
   - Используем `NpgsqlDataSource` для пула соединений.
   - В SQL-запросах используем явные алиасы (`AS ArtistId`) и Raw String Literals (`"""`).
   - Параметры передаются только через `@Parameter` (защита от SQL-инъекций).
5. **REST API**: 
   - Пустой список возвращает `200 OK` с `[]`, а не `404`.
   - ID ресурса берётся из URL, в теле запроса (PUT/POST) передаются только данные.