Для сборки проекта потребуется:

* установленный netcore 1.1
* node.js с npm (устанавливаются вместе)
* bower
* java (JDK или JRE) - для сборки фронта используется closure compiler, ему нужна java

```
git clone https://github.com/gonzazoid/WeatherApp.git
cd WeatherApp
npm install // для установки пакетов необходимых для сборки фронтенда
npm install -g bower // возможно придется ставить под sudo

dotnet restore
dotnet build

// если запускаете приложение на своей базе - нужны миграции
dotnet ef migrations add InitialCreate
dotnet ef database update

// если на мой базе - верхние две строки пропускаем

// запуск приложения:

DB_PWD=пароль_от_базы dotnet run
```

Немного вкратце опишу по коду:

В условиях к задаче указывались требования:

* MS VisualStudio 2010
* .Net 4.0
* ASP.NET MVC4
* MSSQL Express for data storage
* jqGrid
* DI/IoC via Microsoft.Practices.Unity
* Data access via Entity CodeFirst
* Dependencies loaded via NuGet

К сожалению виндовой машины у меня под рукой нет (так что бы сесть и днями на ней работать), есть мак а на нем ставится только net core, собственно под нее и писал. Причем вторая не встала, писал под 1.1. В принципе сейчас разобрался уже достаточно, если это принципиальный момент - могу переписать и под .Net 4.0 (не стал сразу этого делать ввиду и без того затянувшихся сроков)

Отсюда же отсутствие Unity - в net core свой IoC, использовал его.

Отсюда же отсутствие jqGrid на сервере - под net core этой либы нет(обработка запросов на бэке), принимаю запросы ручками.

На счет VisualStudio - я так понял это требование к тому что проект должен собираться в этой IDE. в общем я работаю в vim-е, собирал с консоли, в VisualStudio должно собираться без проблем, никаких расширений и прочих плюшек не использовалось.

Относительно сборки - сервер собирается стандартно, для сборки фронтенда - я отключил то что стояло по умолчанию и подкрутил webpack. Фронт (громко сказано, там по сути всего два скрипта собираются) написан на typescript, прогоняется через линтер и сжимается closure compiler-ом. Остался рудимент в виде подкачки пакетов bowler-ом, его не стал пока трогать, вынес в npm скрипт, но вообще на реальном  проекте управление пакетами лучше отдать npm и нарезать бандл через webpack. Да, и по поводу дальнейшего развития - работать с jqGrid напрямую - это лапша в коде и ее даже модулями не особо спрячешь, если использовать в серьезном проекте (широком и долгострочном) то конечно лучше бы jqGrid обвернуть во что нибудь (react компонента, custom element, в общем исходя из того что используется)

Теперь по серверу.

Прежде чем продолжу, напомню - это тест скорее не на то как я могу написать back а на то насколько я разобрался в шарпах и стеке .Net за три недели. То есть вообще не настаиваю на решениях, к критике готов )

Из реализованного:
* strongly typed options, то есть у опций своя модель и они сервисом пробрасываются по коду.
* параметры запросов из json-а биндятся в объекты (на этом этапе проверки через аннотации) + прикрутил проверки fluentvalidation
* ответы провайдеров тоже описал моделями, также проверки на аннотациях, fluentvalidation прикрутил но правила не указал - аннотаций хватило, валидацию оставил для полноты, на развитие. И еще момент - yahoo давление отдает непонятнов чем, это не миллибары, несмотря на то что они указаны в юнитах. С этим не разобрался, в общем тех. долг.
* с правилами валидации немного поиграл с наследованием, на таком объеме не принципиально, но если писать полноценный API - то очень удобно.
* запросы отрабатываются асинхронно, с базой тоже в большинстве случаев работа асинхронная.
* по ORM особо сказать нечего, единственное - удаление сущностей реализовал каскадом.
* опрос провайдеров информации реализовал через hangFire, тут не настаиваю - просто пробежался по форумам, посмотрел обзоры таск менеджеров, его хвалили больше всего. Частота опроса - в настройках, cron expression. На сайте - ссылка HangFire - можно посмотреть нагрузку.

Кстати о нагрузке - поскольку непонятно с каким объемом данных придется работать я заложился на две точки роста для масштабирования. Если по мере роста проекта объемы будут расти (увеличивается число отслеживаемых городов/уменьшается интервал опроса) то будет необходим менеджер нагрузки. Его можно будет впилить либо в WeatherChecker - это то место где есть данные по объему предстоящей работы, либо в UserAgent - там данных нет, это обертка вокруг WebRequest, я ее ввел сознательно - если что можно быстро этот сервис сделать синглтоном и аккумулировать запросы, обрабатывая согласно настройкам (например пакетами по десять с перерывом в пять минут и тому подобное).

По теме развития - можно еще предположить что будет увеливаться число провайдеров, но здесь я особо не стал углубляться. Чекеры сидят методами в WeatherProviders, если реально надо больше двух провайдеров - то на мой взгляд лучше тогда каждый чекер оформлять сервисом, но тогда потребуется свой service locator. В общем оно сейчас собрано в одном месте, если что - можно быстро переделать. Небольшой задел на будущее без преждевременного усложнения.

По обработке ошибок - поставил кетчи, но поскольку нет условий в ТЗ - ничего не делаю. Все места перехвата выводятся в warnings, не убрал сознательно что бы всегда было видно. По warnings - первые два - это от tslinter-а, это нормально, там ишью по tslint-loader-у, ждем пока исправят (в конфиге вебпака есть ссылка)

Да, поскольку обработку jqGrid запросов делаю своим велосипедом - я не стал уже писать поиск, новых скилов это не продемонстрирует, просто много рутинной работы. Работает добавление, редактирование, пейджинг и сортировка.

Оба приложения (веб-сервер и чекер) используют Startup, у каждого свой набор сервисов, но я не стал разделять на два отдельных класса. Если это потребуется (например если мы захотим чекер вынести в отдельное приложение) то Startup надо будет разделять на базовый и два унаследованных, в наследуемых - реализовать соотв. ConfigureServices. Я к тому что это не реализовано (на таком объеме кода - это излишнее усложнение) - но я это вижу и по мере роста проекта этот момент должен быть учтен.

Да, по базе. Во первых на маке MSSQL не ставится, поэтому я базу взял на azure, думаю это не принципиальный момент. Второй момент - все время пока я програмил база у меня создавалась с кода (dbContext.Database.EnsureCreated()) причем реально создавалась, я помню как менял структуру и эти изменения заливались. Перед тем как отправлять код решил пройти процесс с нуля, снес базу и на этом все, EnsureCreated лезет на абсолютно чистую базу и сообщает что там все есть, ничего делать не надо. В чем дело - я так и не осилил, поэтому пока на миграциях.

В принципе все, дальше только код смотреть. Пароль к базе вышлю почтой.
