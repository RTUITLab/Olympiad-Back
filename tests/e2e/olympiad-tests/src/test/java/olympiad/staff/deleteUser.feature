@ignore
Feature: create temp user

Scenario: delete user
  Given url baseUrl
  * configure headers = { Authorization: '#("Bearer " + admin.token)'}
  Given path 'api', 'account', userId
  When method delete
  Then status 204
