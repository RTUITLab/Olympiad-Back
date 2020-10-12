Feature: challenges feature



Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }

Scenario: create challenge and get it
  Given path 'api/challenges'
  And request { "name": "karate challenge test", "startTime": "2020-09-11T15:58:08.543Z", "endTime": "2020-09-11T15:58:08.543Z", "challengeAccessType": 0 }
  When method post
  Then status 200

  * def createdSolutionId = response.id
  Given path 'api/challenges/', createdSolutionId
  When method get
  Then status 200
  Then match response contains {id: #(createdSolutionId)}