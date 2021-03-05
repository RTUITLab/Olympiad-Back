FROM maven:3-openjdk-8

WORKDIR /project
COPY . /project

RUN mvn clean install -Dmaven.test.skip=true

CMD mvn test -Dtest=TestParallel -DargLine="-Dkarate.env=e2e"