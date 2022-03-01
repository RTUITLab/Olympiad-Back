Feature: account get one

Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }

Scenario: get one user
  Given path 'api', 'account', currentUser.id
  When method get
  Then status 200
  And match response == { id: '#uuid', studentId: '#string', firstName: '#notnull', email: '#notnull' }
  And match response contains { id: '#(currentUser.id)'}

Scenario: get non-existent user
  Given path 'api', 'account', '00000000-0000-0000-0000-000000000000'
  When method get
  Then status 404
  And match response == '#string'


Scenario: get user with incorrect id
  Given path 'api', 'account', 'some strange id'
  When method get
  Then status 404



