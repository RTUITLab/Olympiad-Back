Feature: account search

Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }

Scenario: search without parameters
  Given path 'api/account'
  When method get
  Then status 200
  And match response == {data: '#array', total: '#number', offset: 0, limit: 50}

Scenario: search one user
  Given path 'api/account'
  And param limit = 1
  When method get
  Then status 200
  And match response == {data: '#[1]', total: '#number', offset: 0, limit: 1}

Scenario: search three users
  Given path 'api/account'
  And param limit = 3
  When method get
  Then status 200
  And match response == {data: '#[3]', total: '#number', offset: 0, limit: 3}

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

Scenario: search with skip users
  Given path 'api/account'
  And param offset = 3
  When method get
  Then status 200
  And match response == {data: '#array', total: '#number', offset: 3, limit: 50}
  And assert response.data.length <= response.total


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


