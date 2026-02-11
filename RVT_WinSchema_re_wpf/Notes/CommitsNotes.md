## Категории

Что будет видно пользователю:
- **feat** - новая фича
- **fix** - исправление багов

Будут скрыты:
- **refactor** - рефакторинг кода
- **build** - изменения связанные с сборкой проекта (допустим работа с csproj файлом)
- Все что без категории

## Примеры и советы

Примеры:
- feat: change naming based on cache + order by name
- fix: download by plugin name not by repo name
- refactor: rename command and delete unused files
- build: made test slow build with changelog

Старайтесь разбивать на мелкие коммиты чтобы было как можно меньше ситуаций `refactor: pluginClickHandler + fix: clicks on plugins`если не получается никак, то пофиг.