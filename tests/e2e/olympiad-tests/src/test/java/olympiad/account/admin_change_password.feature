Feature: admin change password

Background:
  * url baseUrl
  * def getRandomUser =
  """
  function(){
    var id = java.util.UUID.randomUUID();
    return {
      email: 'change_default_user_info_' + id + '@mail.com',
      studentId: '_old_student_id_' + id,
      firstName: '_old_first_name_' + id,
    }
  }
  """

Scenario: change password by admin

  * def createUserParams = getRandomUser()
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * def createdUser = createdUserData.user

  * configure headers = { Authorization: '#("Bearer " + admin.token)'}
  Given path 'api', 'account', 'adminChangePassword', createdUser.id
  And header Content-Length = 0
  When method post
  Then status 200
  And match response == { newPassword: '#string' }

  * def loginWithNewPassword = call read('classpath:olympiad/auth/login.feature') { login: '#(createdUser.email)', password: '#(response.newPassword)' }

  Given path 'api', 'account', createdUser.id
  When method delete
  Then status 204

Scenario: change password by admin can't be invoked by plainUser

  * def createUserParams = getRandomUser()
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * def createdUser = createdUserData.user

  * configure headers = { Authorization: '#("Bearer " + plainUser.token)'}
  Given path 'api', 'account', 'adminChangePassword', createdUser.id
  And header Content-Length = 0
  When method post
  Then status 403

  * configure headers = { Authorization: '#("Bearer " + admin.token)'}
  Given path 'api', 'account', createdUser.id
  When method delete
  Then status 204

Scenario: change password by admin can't be invoked for non-existent user

  * configure headers = { Authorization: '#("Bearer " + admin.token)'}
  Given path 'api', 'account', 'adminChangePassword', '00000000-0000-0000-0000-000000000000'
  And header Content-Length = 0
  When method post
  Then status 404
