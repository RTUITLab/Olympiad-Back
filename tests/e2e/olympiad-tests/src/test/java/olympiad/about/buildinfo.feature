Feature: Get build info

Background:
  * url baseUrl


Scenario: Returns string build info
  Given path 'api', 'about'
  When method get
  Then status 200
  And match response == { buildNumber: '#string' }
