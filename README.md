# Tube Company Telegram Mini App

Telegram Mini App для автоматизации клиентских заказов трубной продукции. Приложение цифровизирует и автоматизирует процесс консультаций и заказов трубной продукции, предоставляя пользователям удобный интерфейс для поиска, выбора и оформления заказов.

## 🎯 Функциональность

### Основные возможности
- **Фильтрация продукции** по складу, виду продукции, диаметру, стенке, ГОСТу, марке стали
- **Умная корзина** с возможностью указания количества в метрах и тоннах
- **Динамические скидки** в зависимости от объема заказа
- **Актуальные цены** с возможностью обновления несколько раз в день
- **Оформление заказов** с вводом персональных данных

### Дополнительные функции
- **Система обновления данных** для синхронизации каталога продукции
- **Оперативное обновление цен и остатков**
- **Интеграция с Telegram** через WebApp

## 🏗️ Архитектура

### Backend (C#)
- **ASP.NET Core Web API**
- **Entity Framework Core** для работы с базой данных
- **Docker** контейнеризация
- **Swagger** для документации API

### Frontend (на выбор команды)
- **React** или любой другой фреймворк
- **Telegram WebApp API** для интеграции с Telegram
- **Адаптивный дизайн** для мобильных устройств

## 📋 API Endpoints

### 🛒 Управление корзиной (`/api/Cart`)

#### Получение данных корзины
- **GET** `/api/Cart/{userId}/paged` - Получить корзину с пагинацией
  - Параметры: `from`, `to` (пагинация)
- **GET** `/api/Cart/{userId}/total` - Получить сумму корзины
- **GET** `/api/Cart/{userId}/count` - Получить количество товаров
- **GET** `/api/Cart/{userId}/validate` - Проверить существование пользователя

#### Управление товарами в корзине
- **POST** `/api/Cart/{userId}/items` - Добавить товар в корзину
  - Body: `AddToCartRequest`
- **PUT** `/api/Cart/{userId}/items/{stockId}/{productId}` - Обновить количество товара
  - Body: `UpdateQuantityRequest`
- **DELETE** `/api/Cart/{userId}/items/{stockId}/{productId}` - Удалить товар из корзины
- **DELETE** `/api/Cart/{userId}` - Очистить корзину

#### Поиск в корзине
- **GET** `/api/Cart/{userId}/search` - Поиск товаров в корзине
  - Параметры: `term` (поисковый запрос)
- **GET** `/api/Cart/{userId}/search/paged` - Поиск товаров в корзине с пагинацией
  - Параметры: `term`, `from`, `to`

### 📦 Управление номенклатурой (`/api/Nomenclature`)

#### Основные операции
- **GET** `/api/Nomenclature` - Получить весь список номенклатуры
- **GET** `/api/Nomenclature/paged` - Получить список с пагинацией
  - Параметры: `page`, `pageSize`
- **GET** `/api/Nomenclature/{id}` - Получить позицию по ID
- **POST** `/api/Nomenclature` - Создать новую позицию
  - Body: `CreateNomenclatureRequest`
- **PUT** `/api/Nomenclature/{id}` - Обновить позицию
  - Body: `UpdateNomenclatureRequest`
- **DELETE** `/api/Nomenclature/{id}` - Удалить позицию

#### Фильтрация и поиск
- **GET** `/api/Nomenclature/type/{typeId}` - Получить по типу продукции
- **GET** `/api/Nomenclature/type/{typeId}/paged` - Получить по типу продукции с пагинацией
  - Параметры: `page`, `pageSize`
- **GET** `/api/Nomenclature/search` - Поиск по номенклатуре
  - Параметры: `term` (обязательный)
- **GET** `/api/Nomenclature/search/paged` - Поиск по номенклатуре с пагинацией
  - Параметры: `term` (обязательный), `page`, `pageSize`
- **GET** `/api/Nomenclature/test` - Тест API

### 💰 Управление ценами (`/api/Prices`)

#### Операции с ценами
- **GET** `/api/Prices/{productId}/{stockId}` - Получить цену для конкретного товара и склада
- **PUT** `/api/Prices/{productId}/{stockId}` - Полное обновление цены для товара на складе
  - Body: `UpdatePriceRequest`
- **PATCH** `/api/Prices/{productId}/{stockId}` - Частичное обновление цены
  - Body: `PricePatchRequest`
- **POST** `/api/Prices` - Создать новую цену для товара на складе
  - Body: `CreatePriceRequest`

#### Получение списков цен
- **GET** `/api/Prices/product/{productId}` - Получить все цены для конкретного товара
- **GET** `/api/Prices/stock/{stockId}` - Получить цены на конкретном складе по диапазону товаров
  - Параметры: `fromId`, `toId`, `pageLimit`, `page`

### 👥 Управление пользователями Telegram (`/api/TelegramUsers`)

#### Основные операции
- **GET** `/api/TelegramUsers` - Получить всех пользователей
- **GET** `/api/TelegramUsers/{telegramUserId}` - Получить пользователя по ID
- **POST** `/api/TelegramUsers` - Создать нового пользователя
  - Body: `CreateTelegramUserRequest`
- **PUT** `/api/TelegramUsers/{telegramUserId}` - Обновить пользователя
  - Body: `UpdateTelegramUserRequest`
- **DELETE** `/api/TelegramUsers/{telegramUserId}` - Удалить пользователя

#### Поиск и фильтрация
- **GET** `/api/TelegramUsers/inn/{inn}` - Получить пользователя по ИНН
- **GET** `/api/TelegramUsers/search` - Поиск пользователей
  - Параметры: `term` (обязательный)
- **GET** `/api/TelegramUsers/test` - Тест API

### 📦 Управление заказами (`/api/Orders`)

- **POST** `/api/Orders` - Создать новый заказ
  - Body: `CreateOrderRequest`
- **GET** `/api/Orders/user/{telegramUserId}` - Получить заказы пользователя
- **GET** `/api/Orders/{orderId}` - Получить заказ по ID
- **GET** `/api/Orders/test` - Тест API

### 🔄 Система обновлений (`/api/Update`)

#### Управление обновлениями
- **POST** `/api/Update/process-pending` - Обработать все ожидающие обновления
- **GET** `/api/Update/status` - Получить статус обновлений
- **POST** `/api/Update/prices` - Загрузить обновления цен
  - Body: `PriceUpdateRequest`
- **POST** `/api/Update/remnants` - Загрузить обновления остатков
  - Body: `RemnantUpdateRequest`
- **POST** `/api/Update/force-sync` - Принудительная полная синхронизация
- **POST** `/api/Update/process-specific` - Обработать конкретный тип обновлений
  - Параметры: `updateType`

#### Мониторинг и логи
- **GET** `/api/Update/logs` - Получить логи обновлений
  - Параметры: `from`, `to` (дата-время)
- **POST** `/api/Update/cleanup` - Очистить обработанные обновления
  - Параметры: `daysOld` (по умолчанию 7)

### 🤖 Telegram Integration (`/api/Telegram`)

- **POST** `/api/Telegram/webhook` - Webhook для Telegram бота
  - Body: `Update`
- **GET** `/api/Telegram/test` - Тест API

### 🛠️ Системные endpoints

#### Backend управление
- **GET** `/setup-admin` - Настройка администратора
- **GET** `/health` - Проверка здоровья сервиса
- **GET** `/test-bot` - Тест бота
- **GET** `/setup-webhook` - Настройка webhook
- **GET** `/webhook-info` - Информация о webhook

#### MiniApp
- **GET** `/miniapp` - Основной endpoint для MiniApp

## 🚀 Запуск проекта

### Предварительные требования
- .NET 6.0+
- Docker и Docker Compose
- База данных (PostgreSQL)

### Развертывание

1. **Клонирование репозитория**
```bash
git clone <https://github.com/Shusha228/TubeCompanyApp>
cd tube-company
```
2. **Переменные окружения**

***Настройки Telegram бота***
```env
TELEGRAM_BOT_TOKEN=your_token
TELEGRAM_BOT_NAME=your_name
```
***Конфигурация PostgreSQL***
```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
POSTGRES_DB=postgres
POSTGRES_HOST=postgres_container #from docker-compose.yml
POSTGRES_PORT=5432 #from docker-compose.yml
```

***URL приложения***
```env
APP_BASE_URL=your_url
FRONTEND_URL=http://localhost:3000 #for local start
BACKEND_URL=http://localhost:8080 #for local start
```

***Настройки ASP.NET Core***
```env
ASPNETCORE_ENVIRONMENT=Development
```

3. **Запуск**
```bash
docker-compose up --build
```