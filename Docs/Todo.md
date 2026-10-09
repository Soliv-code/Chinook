# 📋 Chinook Project TODO

Этот документ отражает текущий статус, историю и план развития проекта Chinook API (.NET 10, Clean Architecture, Dapper).

## ✅ Выполнено (Готово)

### ️ Архитектура и Инфраструктура
- [x] **Инициализация решения**: Создан Solution `Chinook` в Visual Studio 2026 (.NET 10).
- [x] **Архитектура**: Реализована Clean Architecture из 4-х проектов:
  - `Chinook.Domain` (без зависимостей)
  - `Chinook.Application` (ссылается на Domain)
  - `Chinook.Infrastructure` (ссылается на Domain и Application)
  - `Chinook.API` (ссылается на Application и Infrastructure, Controller-based)
- [x] **Git**: Репозиторий инициализирован, ветка по умолчанию изменена на `main`.
- [x] **Инфраструктура (Docker)**:
  - Настроен `Docker/docker-compose.yml`.
  - Поднят PostgreSQL 17 с расширением `pgvector` (`pgvector/pgvector:pg17`).
  - Настроен современный `init.sql` с `GENERATED ALWAYS AS IDENTITY` (без legacy-последовательностей).
  - Настроен `Dockerfile` для multi-stage сборки API.

### ️ Инструменты и Исследование
- [x] **Инструменты разработки**:
  - DBeaver: подключен к локальной БД, схема проверена.
  - Bruno: коллекция API создана внутри папки проекта, успешный пинг тестового эндпоинта.
- [x] **SQL-исследование (DBeaver)**:
  - Изучена бизнес-логика схемы Chinook (цифровой магазин музыки).
  - Освоены преобразования типов, работа с `NULL` (`COALESCE`, `CASE WHEN`) и форматирование времени.
- [x] **Domain**: Определена первая сущность `Artist` и интерфейс `IArtistRepository`.

### 🌿 Git Workflow
- [x] Создана feature-ветка `feature/artist-repository` для разработки первого CRUD.

---

## 🚧 В работе / Следующие шаги

### Этап 1: Базовый CRUD на Dapper (Текущий фокус)
- [x] Добавить NuGet-пакеты в `Chinook.Infrastructure`: `Dapper` и `Npgsql`.
- [x] Реализовать `ArtistRepository` в `Infrastructure` (используя `NpgsqlDataSource` и Dapper).
- [ ] Настроить регистрацию зависимостей (DI) в `Program.cs`.
- [ ] Создать `ArtistsController` в `Chinook.API`.
- [ ] Протестировать эндпоинты `GET /api/artists` и `GET /api/artists/{id}` через Bruno.

### Этап 2: Кэширование (Redis)
- [ ] Добавить сервис `redis` в `docker-compose.yml`.
- [ ] Добавить NuGet-пакет `StackExchange.Redis` в `Infrastructure`.
- [ ] Создать интерфейс `ICacheService` в `Domain` и его реализацию `RedisCacheService` в `Infrastructure`.
- [ ] Реализовать паттерн Cache-Aside в `Application` (например, для запроса списка артистов).
- [ ] Протестировать в Bruno (первый запрос медленный, второй мгновенный).

### Этап 3: Авторизация и безопасность (Keycloak)
- [ ] Добавить сервис `keycloak` в `docker-compose.yml`.
- [ ] Настроить аутентификацию по JWT Bearer токенам в `Chinook.API`.
- [ ] Добавить роли (например, `Customer` и `Employee`).
- [ ] Закрыть эндпоинты атрибутом `[Authorize]` и настроить политику доступа.

### Этап 4: Продвинутые фичи (Опционально / На будущее)
- [ ] Реализовать более сложные запросы Dapper (например, `Album` с вложенным списком `Track` через `QueryMultiple` или маппинг).
- [ ] Добавить векторный поиск (`pgvector`): поиск треков или плейлистов по семантическому описанию.
- [ ] Добавить фоновую обработку событий (RabbitMQ/Kafka): отправка email-чека при создании новой `Invoice`.

---

##  Заметки и правила проекта
1. **Источник истины**: Структура БД определяется только через `Docker/SQL/init.sql`. Любые изменения в DBeaver должны быть немедленно перенесены в этот файл.
2. **Docker Workflow**: Для локальной разработки API запускается через Visual Studio (F5). Пересборка Docker-образа API (`docker-compose up -d --build`) делается только для финальной проверки или деплоя.
3. **База данных**: Никогда не использовать `docker-compose down -v` без крайней необходимости (это удаляет все данные).
4. **Dapper & Postgres**: Используем `NpgsqlDataSource` для пула соединений. В SQL-запросах используем явные алиасы (`AS ArtistId`) для предсказуемого маппинга.