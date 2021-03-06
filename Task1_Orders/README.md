**a. Написать консольное приложение с потоками, сделать так чтобы в результате обновления одного и того же Order его Amount был 
не равен его Items.Sum(x => x. Amount)**

![Результат](Assets/РезультатДо.png)

**b.Предложить, как минимум два способа устранения проблемы средствами БД, один из которых – изоляция транзакции
(выбрать минимально необходимый уровень изоляции), другой - неблокирующий запись таблицы БД.**

**1 способ**: минимальный уровень изоляции транзакции для решения проблемы - Repeatable Read, т.к. на уровнях ниже блокировка удерживается только на время работы с ресурсом, а в данном случае нужно, чтобы блокировка удерживалась на протяжении всей транзакции. Т.о. при уровне изоляции Repeatable Read одна из двух транзакций не сможет модифицировать ресурс(в данном случае изменение Amount при вставке новой строки) во время работы другой транзакции выполняющей также чтение и вставку строки. Вторая транзакция будет ожидать полного завершения первой транзакции.

Тестовый пример в БД
**Уровень изоляции - READ COMMITTED - не подходит**
![Пример с READ COMMITTED](Assets/ReadCommitted.png)

**Уровень изоляции - REPEATABLE READ - подходит**
![Пример с REPEATABLE READ](Assets/RepeatableRead.png)

**2 способ**: основан на версионности данных. СУБД создает новую версию строки для транзакции при каждом изменении данных в этой строке. 
С этой новой версией продолжает работать та транзакция, которая ее создала, но любая другая транзакция видит строку в том виде, 
в котором она была зафиксирована, т.е. в том виде, пока не началась транзакция, изменяющая эту строку. 

**Реализовать в консольном приложении не удалось. Пыталась применять уровни изоляции в коде C#, но для данной задачи с потоками не получилось сделать так, чтобы он выполнял несколько действий одной транзакцией с указанным уровнем изоляции, т.е., чтобы один поток при работе с ресурсом блокировал доступ второму потоку на чтение и обновление.**
