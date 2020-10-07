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