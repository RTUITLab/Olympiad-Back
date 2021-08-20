@ignore
Feature: authorize tests

Background:
  * def login = karate.get('login', 'admin@localhost.ru')
  * def password = karate.get('password', 'VeryStrongPass1')


Scenario:
  Given url baseUrl
  And path '/api/Auth/login'
  And request { login: '#(login)', password: '#(password)' }
  When method post
  Then status 200
  * print response.token
  * def user = response