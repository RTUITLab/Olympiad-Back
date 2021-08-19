Feature: account search

Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }

Scenario: search without parameters
  Given path 'api/account'
  When method get
  Then status 200
  And match response == {data: '#[3]', total: 3, offset: 0, limit: 50}

Scenario: search one user
  Given path 'api/account'
  And param limit = 1
  When method get
  Then status 200
  And match response == {data: '#[1]', total: 3, offset: 0, limit: 1}

Scenario: search two users
  Given path 'api/account'
  And param limit = 2
  When method get
  Then status 200
  And match response == {data: '#[2]', total: 3, offset: 0, limit: 2}

Scenario: search three users
  Given path 'api/account'
  And param limit = 3
  When method get
  Then status 200
  And match response == {data: '#[3]', total: 3, offset: 0, limit: 3}

Scenario: search with incorrect limit
  Given path 'api/account'
  And param limit = 0
  When method get
  Then status 400

Scenario: search with max limit
  Given path 'api/account'
  And param limit = 200
  When method get
  Then status 200
  And match response == {data: '#[3]', total: 3, offset: 0, limit: 200}

Scenario: search with one more than max limit
  Given path 'api/account'
  And param limit = 201
  When method get
  Then status 400


Scenario: search with incorrect offset
  Given path 'api/account'
  And param offset = -1
  When method get
  Then status 400

Scenario: search with skip one user
  Given path 'api/account'
  And param offset = 1
  When method get
  Then status 200
  And match response == {data: '#[2]', total: 3, offset: 1, limit: 50}

Scenario: search with skip two users
  Given path 'api/account'
  And param offset = 2
  When method get
  Then status 200
  And match response == {data: '#[1]', total: 3, offset: 2, limit: 50}

Scenario: search with skip three users
  Given path 'api/account'
  And param offset = 3
  When method get
  Then status 200
  And match response == {data: '#[0]', total: 3, offset: 3, limit: 50}


Scenario: search match user
  Given path 'api/account'
  And param match = 'executor default'
  When method get
  Then status 200
  And match response == {data: '#[1]', total: 1, offset: 0, limit: 50}

Scenario: search incorrect user
  Given path 'api/account'
  And param match = 'nouserwiththattext'
  When method get
  Then status 200
  And match response == {data: '#[0]', total: 0, offset: 0, limit: 50}

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
  Then status 400



