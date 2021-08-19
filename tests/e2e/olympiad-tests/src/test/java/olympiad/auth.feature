@ignore
Feature: authorize tests


Scenario:
  Given url baseUrl
  And path '/api/Auth/login'
  And request { login: 'admin@localhost.ru', password: "VeryStrongPass1" }
  When method post
  Then status 200
  * print response.token
  * def accessToken = response.token
  * def user = response