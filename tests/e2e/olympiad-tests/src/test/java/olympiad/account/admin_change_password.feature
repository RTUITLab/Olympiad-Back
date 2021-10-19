Feature: admin change password

Background:
  * url baseUrl


Scenario: change password by admin

  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  * configure headers = { Authorization: '#("Bearer " + admin.token)'}
  Given path 'api', 'account', 'adminChangePassword', createdUser.id
  And header Content-Length = 0
  When method post
  Then status 200
  And match response == { newPassword: '#string' }

  * def loginWithNewPassword = call read('classpath:olympiad/auth/login.feature') { login: '#(createdUser.email)', password: '#(response.newPassword)' }

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }


Scenario: change password by admin can't be invoked by plainUser

  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  * configure headers = { Authorization: '#("Bearer " + plainUser.token)'}
  Given path 'api', 'account', 'adminChangePassword', createdUser.id
  And header Content-Length = 0
  When method post
  Then status 403

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }


Scenario: change password by admin can't be invoked for non-existent user

  * configure headers = { Authorization: '#("Bearer " + admin.token)'}
  Given path 'api', 'account', 'adminChangePassword', '00000000-0000-0000-0000-000000000000'
  And header Content-Length = 0
  When method post
  Then status 404
