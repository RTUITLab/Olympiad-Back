Feature: Get build info

Background:
  * url baseUrl


Scenario:
  Given path 'api', 'about'
  When method get
  Then status 200
  And match response == { buildNumber: '#string' }
