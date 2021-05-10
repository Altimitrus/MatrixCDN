# Установка на Debian 9.x
wget -O - http://nserv.host/dl/MatrixCDN/install.sh | bash

# MatriX.CDN
Работает на Debian 9.x<br>
Использует TorrServer MatriX.93 (30 экземпляров)<br>
Общий расход памяти ~600MB RAM

# Настройки по умолчанию
Cache size: 192<br>
Reader readahead: 45<br>
Torrent disconnect timeout: 240<br>
Connections limit: 23<br>
Dht connection limit: 400

# Изменить настройки
1. Заменить значения в settings.json<br>
2. Выполнить curl -s "http://127.0.0.1:5000/cron/UpdateSettings"

# Доступ через авторизацию
Создайте accs.db в таком формате<br>
{<br>
  "User1": "Pass1",<br>
  "User2": "Pass2"<br>
}<br>
<br>
Для обновления accs.db без перезагрузки<br>
curl -s "http://127.0.0.1:5000/cron/UpdateAccs"

# Особенности авторизаци
У каждого пользователя своя личная база, в которую он может добавлять и удалять свои торренты


