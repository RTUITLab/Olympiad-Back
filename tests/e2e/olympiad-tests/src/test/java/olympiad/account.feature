Feature: account feature

Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }

Scenario: change password
  # change pass
  Given path 'api/account/changePassword'
  * def oldPassword = "VeryStrongPass1"
  * def newPassword = "NewPassword1"
  And request { "currentPassword": #(oldPassword), "newPassword": #(newPassword) }
  When method post
  Then status 200

  # old pass can't be user
  Given path '/api/auth/login'
  * configure headers = null
  And request { login: 'admin@localhost.ru', password: #(oldPassword) }
  When method post
  Then status 401

  # new pass works
  Given path '/api/auth/login'
  * configure headers = null
  And request { login: 'admin@localhost.ru', password: #(newPassword) }
  When method post
  Then status 200

  # return old password
  Given path 'api/account/changePassword'
  * configure headers = { Authorization: #(accessToken) }
  And request { "currentPassword": #(newPassword), "newPassword": #(oldPassword) }
  When method post
  Then status 200

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
  And match response == {data: '#[3]', total: 3, offset: 0, limit: 3}

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