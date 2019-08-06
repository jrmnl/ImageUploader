# ImageUploader
Service for uploading images. Development in prgoress

# ТЗ

Реализовать простое REST API с одним единственным методом, который загружает изображения.

Требования:
- Возможность загружать несколько файлов.
- Возможность принимать multipart/form-data запросы.
- Возможность принимать JSON запросы с BASE64 закодированными изображениями.
- Возможность загружать изображения по заданному URL (изображение размещено где-то в интернете).
- Создание квадратного превью изображения размером 100px на 100px.
- Наличие модульных/интеграционных тестов.

Следующее будет плюсом:
- Корректное завершение приложения при получении сигнала ОС (graceful shutdown).
- Dockerfile и docker-compose.yml, которые позволяют поднять приложение единой docker-compose up командой.
- CI интеграция (Travis CI, Circle CI, другие).

# Documentation
RAML - https://github.com/jrmnl/ImageUploader/blob/master/api-contract/api.raml
Postman Examples - https://documenter.getpostman.com/view/1302271/SVYrse28
