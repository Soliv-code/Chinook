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
- [x] **Git**: Репозиторий инициализирован, ветка по умолчанию — `main`.
  - Ветка `feature/artist-repository` успешно слита.
  - Ветка `feat/sql-logging` успешно слита.
  - Ветка `feat/album-crud` успешно слита.
  - Текущая ветка: `feat/simple-entities-crud`.
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
- [x] Реализован `AlbumRepository` с фильтрацией по `artistId` и защитой от дубликатов.
- [x] Настроена регистрация зависимостей (DI) в `Program.cs`.
- [x] Создан `ArtistsController` с полным набором REST-эндпоинтов (GET, POST, PUT, DELETE).
- [x] Создан `AlbumsController` с полным набором REST-эндпоинтов + фильтрация `?artistId=`.
- [x] Реализована безопасная серверная сортировка через query-параметры (`?sortBy=name`).
- [x] **Защита целостности БД**:
  - Добавлен составной UNIQUE constraint `uq_album_title_artist` на таблицу `album`.
  - Реализована защита от удаления `Artist` с привязанными альбомами (FK violation).
  - Реализована защита от создания дубликатов `Album` (UNIQUE violation).
- [x] **Логирование бизнес-правил**: внедрён `ILogger` в репозитории для аудита попыток нарушения целостности.
- [x] Протестированы все сценарии в Bruno (включая обработку 400, 404 и 204 статусов).

---

## 📁 Структура проекта

```text
Chinook/
├── BrunoCollection/
│   └── Chinook API/
│       ├── Albums/ (Create, GetAll, GetAll_with_artistId, GetById)
│       ├── Artists/ (Create, Delete, GetAll, GetAll_Sort_by_name, GetById, Update)
│       ── Tests/
├── Chinook.API/
│   ├── Controllers/ (AlbumsController, ArtistsController, PingController)
│   ├── Properties/launchSettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── Dockerfile
│   └── Program.cs
├── Chinook.Application/
│   └── Interfaces/ (IAlbumRepository.cs, IArtistRepository.cs)
├── Chinook.Domain/
│   └── Entities/ (Album.cs, Artist.cs)
├── Chinook.Infrastructure/
│   └── Repositories/ (AlbumRepository.cs, ArtistRepository.cs)
├── Docker/
│   ├── SQL/init.sql
│   ── docker-compose.yml
├── Docs/
│   ├── ProjectStructure.md
│   └── Todo.md
├── .gitignore
── Chinook.slnx
└── README.md
```

---

## 🚀 Следующие шаги

### ⚡ Этап 2: Кэширование (Redis)
- [ ] Добавить сервис `redis` в `docker-compose.yml`.
- [ ] Добавить NuGet-пакет `StackExchange.Redis` в `Infrastructure`.
- [ ] Создать интерфейс `ICacheService` в `Application` и реализацию `RedisCacheService` в `Infrastructure`.
- [ ] Реализовать паттерн Cache-Aside (например, для `GetAllArtists`).
- [ ] Протестировать в Bruno (первый запрос медленный, второй мгновенный).

### 🔒 Этап 3: Авторизация и безопасность (Keycloak)
- [ ] Добавить сервис `keycloak` в `docker-compose.yml`.
- [ ] Настроить аутентификацию по JWT Bearer токенам в `Chinook.API`.
- [ ] Добавить роли (например, `Customer` и `Employee`).
- [ ] Закрыть эндпоинты атрибутом `[Authorize]` и настроить политику доступа.

### 🔗 Этап 4: Продвинутые фичи и связи
- [ ] Реализовать CRUD для `Track` (3 внешних ключа: `album_id`, `genre_id`, `media_type_id`).
- [ ] Написать сложный запрос с `JOIN` (например, получить Альбом со списком Треков в одном запросе через Dapper).
- [ ] Добавить векторный поиск (`pgvector`): поиск треков по семантическому описанию.
- [ ] Добавить фоновую обработку событий (RabbitMQ/Kafka): отправка email-чека при создании `Invoice`.

###  Этап 5: Мониторинг и Профилирование SQL
- [x] **Логирование запросов**: Настроено через `ILoggerFactory` в `NpgsqlDataSource` + `EnableParameterLogging(true)`. Вывод чистого SQL и параметров в консоль.
- [ ] **EXPLAIN ANALYZE**: Реализовать возможность получения плана выполнения запроса (например, через специальный отладочный эндпоинт `GET /api/.../debug-plan`).
- [ ] **Анализ BUFFERS**: Использовать `EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)` для мониторинга чтений с диска и из кеша PostgreSQL.
- [ ] **Автоматические варнинги**: Реализовать парсинг вывода `EXPLAIN` в режиме разработки (Development) для генерации предупреждений или ошибок при обнаружении `Seq Scan` на больших таблицах.

### 🔄 Этап 6: Стандартизация Query Parameters (Идеи на будущее)
- [ ] **Единый стандарт для всех `GetAll`**: Унифицировать подход к параметрам запроса для всех сущностей.
- [ ] **Универсальная сортировка**: Реализовать параметры `sortBy` и `sortDirection` (`asc`/`desc`) с защитой от SQL-инъекций (белый список полей).
- [ ] **Пагинация**: Добавить параметры `page` и `pageSize` (реализовать при интеграции с фронтендом).
- [ ] **Чистка**: Привести `GetAllArtists` и `GetAllAlbums` к единому стандарту.

---

## 📝 Заметки и правила проекта
1. **Источник истины**: Структура БД определяется только через `Docker/SQL/init.sql`. Любые изменения в DBeaver должны быть немедленно перенесены в этот файл.
2. **Docker Workflow**: Для локальной разработки API запускается через Visual Studio (F5). Пересборка Docker-образа API (`docker-compose up -d --build`) делается только для финальной проверки.
3. **База данных**: Никогда не использовать `docker-compose down -v` без крайней необходимости (это удаляет все данные).
4. **Dapper & Postgres**: 
   - Используем `NpgsqlDataSource` (Singleton) с явной передачей `ILoggerFactory`.
   - В SQL-запросах используем явные алиасы (`AS ArtistId`) и Raw String Literals (`"""`).
   - Параметры передаются только через `@Parameter` (защита от SQL-инъекций).
5. **REST API**: 
   - Пустой список возвращает `200 OK` с `[]`, а не `404`.
   - ID ресурса берётся из URL, в теле запроса (PUT/POST) передаются только данные.
   - Foreign Key (например, `artistId`) передаётся как query-параметр для фильтрации, а не как часть пути.
6. **Defense in Depth (Глубокая защита)**:
   - Целостность данных гарантируется на уровне БД (UNIQUE constraints, FK с `ON DELETE NO ACTION`).
   - Приложение перехватывает `PostgresException` и превращает их в понятные бизнес-исключения:
     - `23503` (foreign_key_violation) → `InvalidOperationException` → `400 Bad Request`.
     - `23505` (unique_violation) → `InvalidOperationException` → `400 Bad Request`.
   - Все перехваченные бизнес-исключения логируются через `ILogger` на уровне `Warning` для аудита.
7. **Git Workflow**: Каждая логическая задача — в отдельной ветке (`feat/...`). В `main` мержится только протестированный и рабочий код.