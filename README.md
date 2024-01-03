# CS2-Duels

# RU:
# Описание
Плагин добавляет систему дуэлей 1 на 1 для двух последних живых игроков.
Дуэль начинается как только остаётся 2 последних игрока из разных команд. У каждого из них есть возможность отказаться от дуэли. Игроки могут выбрать оружие для дуэли, если они выберут разное оружие, то будет выдано случайно одно из них обоим игрокам. После начала дуэли игроки будут телепортированы на настроенные вами ранее позиции(если таких нет, телепорт не произойдёт). В начале раунда игрокам вернётся их старое оружие(только тому кто победил, либо обоим, если была ничья)
Позиции для телепорта игроков устанавливаются с помощью команды duel_setpos <1 или 2>. После написания команды с нужным вам номером позиция, где вы стоите будет записана как одна из позиций для телепорта.
Чтобы сохранить позиции необходимо после установки написать duel_savepos

# Команды
* duel_setpos <1 или 2> - установка первой и второй позиции для телепорта дуэлянтов после начала дуэли.
* duel_savepos - сохранение позиций, установленных на карте.
* duel_load - загрузка позиций карты(данные сами загружаются при каждом начале карты)

# Особенности
1. Бессмертие во время голосования за дуэль
2. Восстановление здоровья до 100 при начале дуэли
3. Телепорт дуэлянтов на заранее установленные позиции на карте
4. Выбор любого огнестрельного оружия или ножа в качестве оружия для дуэли

# Требования:
* CounterStrikeSharp v142+
* Linux ONLY

# Установка
Распаковать папку из архива Duels в папку plugins

# EN:
# Description
The plugin adds a  1vs 1 duel system for the last two live players.
The duel begins as soon as the last 2 players from different teams remain. Each of them has the opportunity to refuse the duel. Players can choose weapons for the duel, if they choose different weapons, then one of them will be randomly given to both players. After the start of the duel, the players will be teleported to the positions you set up earlier (if there are no such positions, teleportation will not happen). At the beginning of the round, the players will return their old weapons (only to the one who won, or both, if there was a draw)
The positions for teleporting players are set using the duel_setpos <1 or 2> command. After writing the command with the number you need, the position where you are standing will be recorded as one of the teleport positions.
To save the positions, you need to write duel_savepos after installation

# Commands
* duel_setpos <1 или 2> - setting the first and second positions for teleporting duelists after the start of the duel.
* duel_savepos - saving the positions set on the map.
* duel_load - loading of map positions (data is loaded by itself each time the map is started)

# Features
1. Immortality while voting for a duel
2. Restore health to 100 at the start of the duel
3. Teleport duelists to predetermined positions on the map
4. Choice of any firearm or knife as a duel weapon

# Requirements:
* CounterStrikeSharp v142+
* Linux ONLY

# Installation
Extract the folder from the Duels release archive to the plugins folder
