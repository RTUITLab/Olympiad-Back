Feature: account change password

Background:
  * url baseUrl

Scenario: get token for user by admin account
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }
  Given path 'api', 'auth', 'gettokenforuser', plainUser.id
  When method get
  Then status 200
  And match response == { token: '#string' }
  * configure headers = { Authorization: '#("Bearer " + response.token)' }
  Given path 'api', 'auth', 'getme'
  When method get
  Then status 200
  And match response contains { studentId: '#(plainUser.studentId)' }

Scenario: get token for user by non admin account
  * configure headers = { Authorization: '#("Bearer " + plainUser.token)' }
  Given path 'api', 'auth', 'gettokenforuser', plainUser.id
  When method get
  Then status 403


