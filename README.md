# Postie - личный блог

## Описание проекта

Данный проект представляет собой учебное микросервисное API приложение для ведения личных блогов. Он создан для демонстрации навыков разработки на ASP.NET Core, работы с Docker, Kafka и JWT-аутентификацией.

## Оглавление
1. [Технологии](#технологии)
2. [Основные функции](#основные-функции)
3. [Схема взаимодействия микросервисов](#схема-взаимодействия-микросервисов)
4. [Как запустить проект](#как-запустить-проект)
5. [Примеры запросов](#примеры-запросов)
6. [Тестирование](#тестирование)
7. [Используемые лицензии](#Используемые-лицензии)

## Технологии

- **Backend:** ASP.NET Core
- **База данных:** PostgreSQL
- **Кэш:** Redis
- **Брокер сообщений:** Kafka + ZooKeeper
- **API Gateway:** Ocelot
- **Логирование:** NLog
- **Аутентификация:** JWT
- **Контейнеризация:** Docker, Docker Compose

## Основные функции

### Сервис управления аккаунтами (accountService)

- Регистрация;
- просмотр информации об аккаунте по id;
- просмотр списка всех аккаунтов;
- обновление информации об аккаунте, в т. ч. смена пароля;
- удаление аккаунта;
- валидация данных при регистрации и обновлении информации.

Функции по изменению состояния объекта (н-р удаление, обновление) ограничены пользователем, который его создал. Остальные не имеют к нему доступа.

Все функции (кроме регистрации) требуют токена аутентификации.

### Сервис управления постами (postService)

- Создание поста;
- просмотр информации о посте по id;
- просмотр списка всех постов;
- просмотр списка постов конкретного пользователя по id;
- изменение текста поста;
- удаление поста;
- валидация данных при создании и изменении поста.

Функции по изменению состояния объекта (н-р удаление, обновление) ограничены пользователем, который его создал. Остальные не имеют к нему доступа.

Все функции требуют токена аутентификации.

### Сервис аутентификации (authService)

Сервис предоставляет возможность входа в аккаунт и выдает jwt токен.

Функция сервиса не требуют токена аутентификации.

### Сервис логирования (loggingService)

Данный сервис записывает логи, полученные от остальных сервисов через Kafka, в файлы.

### API Gateway

Через API Gateway осуществляется доступ ко всем остальным сервисам. Он выполняет аутентификацию пользователей, проверяя их JWT-токены, перенаправляет их запросы к сервисам и логирует их.

## Схема взаимодействия микросервисов

Проект включает несколько микросервисов, которые взаимодействуют друг с другом и с внешними сервисами. Ниже представлена схема взаимодействия между ними:

![схема](https://github.com/user-attachments/assets/f1f1c311-a9e0-422e-94c0-9c0e0f90168a)

## Как запустить проект

### Зависимости

Для корректной работы приложения требуются следующие внешние сервисы:

- **PostgreSQL:** используется для хранения данных
- **Kafka + ZooKeeper:** используются для передачи логов
- **Redis:** используется для кэширования сессий

### 1. Локальный запуск **в Visual Studio**

1. Клонируйте репозиторий
    
    ```bash
    git clone https://github.com/Binbogamee/Postie.git
    cd my-project
    ```
    
2. При необходимости измените параметры конфигурации:
    - Проект `Shared` → файл `appConfig.json`: параметры подключения к базе данных и к Kafka для всех микросервисов;
    - проект `AuthHelper` → файл `auth.json`: параметры jwt токена и параметры подключения к Redis для API Gateway;
    - Проект `LoggingService` → файл `nlog.config`: параметры логирования;
    - Проект `ApiGateway` → файл `ocelot.json`: настройки API Gateway.
3. Соберите и запустите необходимые проекты.

### 2. Запуск в Docker (локальная сборка)

1. Клонируйте репозиторий
    
    ```bash
    git clone https://github.com/Binbogamee/Postie.git
    cd my-project
    ```
    
2. Соберите и запустите микросервисы через Docker Compose.
    
    ```bash
    docker-compose up --build
    ```
    

### 3. Запуск в Docker (изображения из Docker Hub)

1. Создайте docker compose файл.

Приведенный ниже пример файла создаст контейнеры для всех микросервисов приложения и необходимых внешних сервисов:

```yaml
services:
    apigateway:
      image: binbogamee/postie-apigateway:latest
      depends_on:
         redis:
           condition: service_started
      environment:
        Redis__ConnectionString: "redis:${REDIS_PORT}"
        JwtOptions__ExpiratesMinutes: "${JWT_EXPMIN}"
        JwtOptions__Key: "${JWT_SECRETKEY}"
        Kafka__BootstrapServers: "kafka:${KAFKA_PORT}"
        GlobalConfiguration__Hosts__PostService: "http://post-service:${POST_SERVICE_PORT}"
        GlobalConfiguration__Hosts__AccountService: "http://account-service:${ACCOUNT_SERVICE_PORT}"
        GlobalConfiguration__Hosts__AuthService: "http://auth-service:${AUTH_SERVICE_PORT}"
        ASPNETCORE_URLS: "http://0.0.0.0:${APIGATEWAY_PORT}"
      ports:
        - "${APIGATEWAY_PORT}:${APIGATEWAY_PORT}"
      networks:
        - postie-network
    
    account-service:
      image: binbogamee/postie-account-service:latest
      environment:
        Kafka__BootstrapServers: "kafka:${KAFKA_PORT}"
        ConnectionStrings__PostieDbContext: "User ID=${DB_USER};Password=${DB_PASSWORD};Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};"
        ASPNETCORE_URLS: "http://0.0.0.0:${ACCOUNT_SERVICE_PORT}"
      ports:
        - "${ACCOUNT_SERVICE_PORT}:${ACCOUNT_SERVICE_PORT}"
      networks:
        - postie-network
  
    auth-service:
      image: binbogamee/postie-auth-service:latest
      environment:
        Kafka__BootstrapServers: "kafka:${KAFKA_PORT}"
        ConnectionStrings__PostieDbContext: "User ID=${DB_USER};Password=${DB_PASSWORD};Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};"
        JwtOptions__ExpiratesMinutes: "${JWT_EXPMIN}"
        JwtOptions__Key: "${JWT_SECRETKEY}"
        ASPNETCORE_URLS: "http://0.0.0.0:${AUTH_SERVICE_PORT}"
      ports:
        - "${AUTH_SERVICE_PORT}:${AUTH_SERVICE_PORT}"
      networks:
        - postie-network
  
    logging-service:
      image: binbogamee/postie-logging-service:latest
      depends_on:
         kafka:
           condition: service_started
      environment:
        Kafka__BootstrapServers: "kafka:${KAFKA_PORT}"
        Log_Path: "${LOG_PATH}"
        ASPNETCORE_URLS: "http://0.0.0.0:${LOGGING_SERVICE_PORT}"
      ports:
        - "${LOGGING_SERVICE_PORT}:${LOGGING_SERVICE_PORT}"
      networks:
        - postie-network
  
    post-service:
      image: binbogamee/postie-post-service:latest
      environment:
        Kafka__BootstrapServers: "kafka:${KAFKA_PORT}"
        ConnectionStrings__PostieDbContext: "User ID=${DB_USER};Password=${DB_PASSWORD};Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};"
        ASPNETCORE_URLS: "http://0.0.0.0:${POST_SERVICE_PORT}"
      ports:
        - "${POST_SERVICE_PORT}:${POST_SERVICE_PORT}"
      networks:
        - postie-network
  
    db-migration:
      image: binbogamee/postie-db-migration:latest
      environment:
        ConnectionStrings__PostieDbContext: "User ID=${DB_USER};Password=${DB_PASSWORD};Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};"
      depends_on:
         postgres:
           condition: service_started
      networks:
        - postie-network
      entrypoint: 
        - /bin/sh
        - -c
        - |
          dotnet ef database update -s /src/Postie -p /src/Postie.DAL
          tail, -f, /dev/null
    
    postgres:
      container_name: "${DB_HOST}"
      restart: no
      image: postgres:latest
      environment:
        POSTGRES_DB: "${DB_NAME}"
        POSTGRES_USER: "${DB_USER}"
        POSTGRES_PASSWORD: "${DB_PASSWORD}"
      volumes:
        - postgres-data:/var/lib/postgresql/data
      ports:
        - "${DB_PORT}:${DB_PORT}"
      networks:
        - postie-network
  
    redis:
      container_name: redis
      restart: no
      image: redis:alpine
      command: ["redis-server","/etc/redis/redis.conf"]
      volumes:
        - /redis/redis-data:/var/lib/redis
        - /redis/redis.conf:/etc/redis/redis.conf
      ports:
        - "${REDIS_PORT}:${REDIS_PORT}"
      networks:
        - postie-network
  
    zookeeper:
      image: confluentinc/cp-zookeeper:latest
      hostname: zookeeper
      container_name: zookeeper
      ports:
        - "2181:2181"
      networks:
        - postie-network
      environment:
        ZOOKEEPER_CLIENT_PORT: 2181
        ZOOKEEPER_TICK_TIME: 2000
  
    kafka:
      image: confluentinc/cp-kafka:latest
      hostname: kafka
      container_name: kafka
      ports:
        - "2${KAFKA_PORT}:2${KAFKA_PORT}"
      environment:
        KAFKA_BROKER_ID: 1
        KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
        KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:${KAFKA_PORT},PLAINTEXT_HOST://localhost:2${KAFKA_PORT}
        KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
        KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
        KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
        KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS: 0
        KAFKA_CONFLUENT_LICENSE_TOPIC_REPLICATION_FACTOR: 1
        CONFLUENT_SUPPORT_CUSTOMER_ID: 'anonymous'
      depends_on:
        - zookeeper
      networks:
        - postie-network
      
    kafka-init:
      image: confluentinc/cp-kafka:latest
      depends_on:
        - kafka
      networks:
        - postie-network
      entrypoint: 
        - /bin/sh
        - -c
        - |
          while ! nc -z kafka ${KAFKA_PORT};
          do
            echo "Waiting Kafka..."
            sleep 1
          done
          echo "Start creating topics"
          kafka-topics --create --topic Audit --bootstrap-server kafka:${KAFKA_PORT} --partitions 1 --replication-factor 1
          kafka-topics --create --topic Errors --bootstrap-server kafka:${KAFKA_PORT} --partitions 1 --replication-factor 1
          kafka-topics --create --topic Heartbeat --bootstrap-server kafka:${KAFKA_PORT} --partitions 1 --replication-factor 1
  
volumes:
    postgres-data:
    
networks:
    postie-network:
      driver: bridge
```

1. Создайте файл .env, в котором прописаны значения необходимых переменных окружения.
    
    Пример:
    
    ```yaml
    DB_NAME=postiedb # название базы данных
    DB_USER=postgres # имя пользователя
    DB_PASSWORD=1234 # пароль пользователя
    DB_HOST=postgres # хост базы данных
    DB_PORT=5432 # порт, на котором развернута БД
    
    REDIS_PORT=6379 # порт, на котором развернут Redis
    KAFKA_PORT=9092 # порт, на котором развернута Kafka
    JWT_EXPMIN=720 # время истечения jwt токена (в минутах)
    JWT_SECRETKEY=9f16549951fd2b69af299e431e97b3bde2311e5311f446d62d76707df5d547a1 # секретный ключ, для подписи jwt токена
    
    APIGATEWAY_PORT=5010 # порт, по которому будет доступен api gateway
    ACCOUNT_SERVICE_PORT=5011 # порт, по которому будет доступен account service
    AUTH_SERVICE_PORT=5012 # порт, по которому будет доступен auth service
    LOGGING_SERVICE_PORT=5013 # порт, по которому будет доступен logging service
    POST_SERVICE_PORT=5014 # порт, по которому будет доступен post service
    
    LOG_PATH=/var/log/Postie # директория, в которую будут писаться логи приложения
    ```
    

1. Соберите и запустите сервисы через Docker Compose
    
    ```bash
    docker-compose up --build
    ```
    

## Примеры запросов

### 1. Регистрация (POST)

**Запрос:**

```bash
POST /api/Account
```

**Тело запроса:**

```json
{
"username": "newuser",
"email": "newuser@mail.com",
"password": "password"
}
```

**Ответ:**

```bash
"dcea1148-d2d4-4b2e-af70-b0b2bbb6013b” # Guid нового аккаунта
```

**Статус код:** 200 OK

### 2. Вход в аккаунт (POST)

**Запрос:**

```bash
POST /api/Login
```

**Тело запроса:**

```json
{
"email": "newuser@mail.com",
"password": "password"
}
```

**Ответ:**

```bash
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyZXF1ZXN0ZXJJZCI6ImRjZWExMTQ4LWQyZDQtNGIyZS1hZjcwLWIwYjJiYmI2MDEzYiIsImV4cCI6MTczNDYzODg4NX0.nE2mfdXIt0sOaEdx4oeBDxARYPLGDpLTRdBaNpKeZuQ # jwt токен
```

**Статус код:** 200 OK

### 3. Создать пост (POST)

**Запрос:**

```bash
POST /api/Post
```

**Тело запроса:**

```json
{
    "text": "текст нового поста"
}
```

**Ответ:**

```bash
"c11b474b-f600-4a34-9b4f-da7aa1448873" # Guid нового поста
```

**Статус код:** 200 OK

### 4. Получить список всех постов (GET)

**Запрос:**

```bash
GET /api/Post
```

**Ответ:**

```json
[
    {
        "id": "c11b474b-f600-4a34-9b4f-da7aa1448873",
        "accountId": "dcea1148-d2d4-4b2e-af70-b0b2bbb6013b",
        "text": "текст нового поста",
        "createdBy": "2024-12-19T08:10:26.303688+00:00",
        "modifiedBy": null
    }
]
```

**Статус код:** 200 OK

### 5. Редактировать пост (PUT)

**Запрос:**

```bash
PUT /api/Post/c11b474b-f600-4a34-9b4f-da7aa1448873
```

**Тело запроса:**

```json
{
    "text": "текст обновленного поста"
}
```

**Ответ:**

```bash
"c11b474b-f600-4a34-9b4f-da7aa1448873" # Guid обновленного поста
```

**Статус код:** 200 OK

### 6. Удалить пост (DELETE)

**Запрос:**

```bash
DELETE /api/Post/c11b474b-f600-4a34-9b4f-da7aa1448873
```

**Ответ:**

```bash
true
```

**Статус код:** 200 OK

## Тестирование

Проект включает для типа тестов:

### 1. Модульные тесты

Модульные тесты написаны с использованием **MSTest** и находятся в папке `/Tests`. Тесты разделены по микросервисам и покрывают основные функциональные компоненты:

- логику бизнес-слоя;
- валидацию данных;
- взаимодействие с базой данных;
- авторизацию.

### 2. Интеграционный тест с использованием JMeter

Данный тест позволяет проверить:

- взаимодействие микросервисов с API Gateway;
- доступ к защищенным эндпоинтам с действительным и недействительным токеном аутентификации.

**Настройка и запуск JMeter**

1. **Предварительные требования:** убедитесь, что у вас установлен [JMeter](https://jmeter.apache.org/) (версии 5.6.3 или выше).
2. **Настройка тестов:** интеграционный тест находится в файле `/Test Plan.jmx`. Для настройки:
    - Откройте JMeter.
    - Перейдите в `File > Open` и выберите указанный выше файл.
    - В разделе Test Plan указаны следующие параметры, используемые в тестировании:
        - **SRV_HOST** — адрес API Gateway;
        - **SRV_PORT** — порт сервиса API Gateway;
        - **SRV_PROTOCOL** — протокол, по которому доступен API Gateway;
        - **USR_NAME_1** — первоначальное имя пользователя;
        - **USR_EMAIL_1** — адрес электронный почты пользователя;
        - **USR_PASSWORD** — пароль пользователя;
        - **USR_NAME_2** — обновленное имя пользователя;
        - **POST_TEXT** — текст создаваемого поста.
3. **Запуск тестов:** нажмите на кнопку **Запуск** (или используйте сочетание клавиш `Ctrl + R`).
4. **Анализ результатов:** для проверки результатов тестов используется **View Result Tree**. В случае правильной работы микросервисов и их взаимодействия все запросы должны быть успешными.

## Используемые лицензии

В данном проекте используются следующие сторонние пакеты:

- [**BCrypt.Net-Next**](https://github.com/BcryptNet/bcrypt.net) — MIT License
- [**Confluent.Kafka**](https://github.com/confluentinc/confluent-kafka-dotnet) — Apache 2.0 License
- [**Microsoft.AspNetCore.Authentication.JwtBearer**](https://github.com/dotnet/aspnetcore) — MIT License
- [**Microsoft.EntityFrameworkCore](https://github.com/dotnet/efcore)** — MIT License
- [**Microsoft.EntityFrameworkCore.Design**](https://github.com/dotnet/efcore) — MIT License
- [**Microsoft.NET.Test.Sdk**](https://github.com/microsoft/vstest) — MIT License
- [**MSTest.TestAdapter**](https://github.com/microsoft/testfx) — MIT License
- [**MSTest.TestFramework**](https://github.com/microsoft/testfx) — MIT License
- [**Newtonsoft.Json**](https://github.com/JamesNK/Newtonsoft.Json) — MIT License
- [**NLog**](https://github.com/NLog/NLog) — BSD-3-Clause
- [**NLog.Web.AspNetCore**](https://github.com/NLog/NLog.Web) — BSD-3-Clause
- [**Npgsql.EntityFrameworkCore.PostgreSQL**](https://github.com/npgsql/efcore.pg) — PostgreSQL License
- [**Ocelot**](https://github.com/ThreeMammals/Ocelot) — MIT License
- [**StackExchange.Redis**](https://github.com/StackExchange/StackExchange.Redis/) — MIT License
- [**Swashbuckle.AspNetCore**](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — MIT License
