<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="./Assets/Repo/banner.png">
    <img src="./Assets/Repo/banner.png" alt="Banner">
  </picture>
</p>

<div id="header" align="center">
  <div id="badges">
    <img alt="GitHub License" src="https://img.shields.io/github/license/WaysoonProgramms/VoiceAssistantLeo?style=for-the-badge">
    <img alt="GitHub watchers" src="https://img.shields.io/github/watchers/WaysoonProgramms/VoiceAssistantLeo?style=for-the-badge">
    <img alt="GitHub Repo stars" src="https://img.shields.io/github/stars/WaysoonProgramms/VoiceAssistantLeo?style=for-the-badge">
    <a href="https://www.donationalerts.com/r/waysoon"><img alt="Static Badge" src="https://img.shields.io/badge/DonationAlerts-Waysoon-red?style=for-the-badge"></a>
  </div>
</div>

<h1> Описание </h1>
<p>
Лео — голосовой помощник для Windows. Написанный на C#. 
Четко распознает голос с помощью Vosk API. Был начат как 
проект для изучения программирования на С#. Немного позже стало 
понятно, что он имеет большой потенциал, в качестве моего 
первого крупного проекта.
</p>

<h1> История создания </h1>
<p>
Проект начался внезапно в начале лета 2023 года. Первоначальное
название ассистента было Popping Cat (Отсылка на мем). Идеей послужил
пост в телеграм канале с кастомной иконкой корзины в виде знаменитого 
кота. Позже, ближе к концу мая, переименован, так как хотелось внедрить
WakeWord, а с таким сложным именем было сложно к нему обращаться.
Уже за идею иконки и лица проекта был взят дракон. А вот сама идея 
создать своего голосового помощника появилась после того, как я 
посмотрел видео, где писали своего ассистента — Jarvis.
</p>

<h1> Благодарности </h1>
<p>
Хочу поблагодарить моих родителей и учителя информатики, за моральную
поддержку и помощь в создании этого проекта.
</p>

<h1> Технологии </h1>
<p>
Озвучено голосом Eugene при помощи <a href="https://github.com/snakers4/silero-models">Silero TTS</a>. Точно
распазноет голос <a href="https://github.com/alphacep/vosk-api">Voks API</a>.
</p>

<h1> Скриншоты приложения </h1>
<picture>
    <img src="./Assets/Repo/Home.png" alt="Home Page Screenshot">
</picture>
<picture>
    <img src="./Assets/Repo/Settings.png" alt="Settings Page Screenshot">
</picture>
<picture>
    <img src="./Assets/Repo/Chat.png" alt="Chat Page Screenshot">
</picture>
<picture>
    <img src="./Assets/Repo/Commands.png" alt="Commands Viewer Page Screenshot">
</picture>

<h1> Голосовые Команды </h1>
<p>
Что-бы активировать ассистента необходимо назвать его имя (<i>Лео</i>). Вы можете как назвать ассистента вместе
с запросом, так и отдельно, разницы нет.
</p>
<h2> Что он может: </h2>
<h3> Запускать приложения: </h3>
<ul>
    <li><code>Открой Телеграм</code> - Запускает Telegram Desktop.</li>
    <li><code>Открой консоль</code> - Запускает CMD</li>
    <li><code>Открой Яндекс Музыку</code> - Открывает приложение Яндекс Музыка</li>
</ul>
<h3> Открывать сайты: </h3>
<ul>
    <li><code>Открой ВКонтакте</code> - Открывает ВК</li>
    <li><code>Открой почту</code> - Открывает Gmail</li>
    <li><code>Открой YouTube</code> - Открывает YouTube</li>
</ul>
<h3> Помогать управлять компьютером: </h3>
<ul>
    <li><code>Почисти корину</code> - Очищает корзину</li>
    <li><code>Поставь на паузу</code> - Ставит на паузу музыку</li>
    <li><code>Включи обратно</code> - Снимает с паузы музыку</li>
    <li><code>Следующий/Предыдущий трек</code> - Управляет очередью</li>
    <li><code>Закрой</code> - Закрывает окно в фокусе (BETA)</li>
</ul>

<h1> Коды ошибок </h1>
<ul>
    <li><code>01</code> - Не удалось получить доступ к микрофону.</li>
    <li><code>02</code> - Не удалось создать запись в реестре</li>
    <li><code>03</code> - Не удалось изменить/удалить запись в реестре</li>
    <li><code>04</code> - Не удалось загрузить последние сообщения из чата</li>
    <li><code>05</code> - Не удалось сохранить последние сообщения из чата</li>
    <li><code>06</code> - Не удалось сохранить команды</li>
    <li><code>07</code> - Не удалось загрузить команды</li>
</ul>

<h1> Связаться со мной </h1>
<ul>
  <li> Почта: antrome@yandex.ru </li>
  <li> Telegram: @ant0nio_Way </li>
</ul>

<i>В проекте используются иконки с сайта <a href="https://icons8.ru/">Icons8.ru</a></i>
