@ignore
Feature: authorize tests

Background:


Scenario:
  * def adminResponse = call read('classpath:olympiad/auth/login.feature') { login: 'admin@localhost.ru', password: 'VeryStrongPass1' }
  * def executorResponse = call read('classpath:olympiad/auth/login.feature') { login: 'executor@localhost.ru', password: 'ExecutorVeryStrongPass1' }
  * def plainUserResponse = call read('classpath:olympiad/auth/login.feature') { login: 'user@localhost.ru', password: '12345678' }

  * def admin = adminResponse.user
  * def executor = executorResponse.user
  * def plainUser = plainUserResponse.user

#  Use admin accessToken for most apis
# TODO: add tests for 403 and use correct users for APIs
  * def accessToken = admin.token

