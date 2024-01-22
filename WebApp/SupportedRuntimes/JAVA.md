java
Java
.java
# Формат сдачи

Необходимо прикладывать единственный файл с названием `Main.java`, в котором будет публичный класс `Main`.
В файле не должна использоваться конструкция `package`, так как при проверке файл запускается без окружения.

## Пример
```java
class Main {
    public static void main(String[] args) {
        System.out.println("Hello, World!");
    }
}
```

Программа собирается и выполняется при помощи JDK 21.

# Сборка
```bash
javac -encoding UTF8 /src/Main.java
```
# Запуск
```bash
java -cp /src Main
```
