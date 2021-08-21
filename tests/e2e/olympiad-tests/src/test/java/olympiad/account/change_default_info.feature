Feature: change default user info

Background:
  * url baseUrl
  * def randomEmail =
  """
  function(){ return 'change_default_user_info_' + java.util.UUID.randomUUID() + '@mail.com' }
  """

Scenario: edit created user by admin
  * def createUserParams =
  """
  {
      email: '#(randomEmail())',
      studentId: 'old_student_id',
      firstName: 'old_first_name',
  }
  """
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }
  Given path 'api', 'account', createdUserData.user.id
  Given request { studentId: 'new_student_id', firstName: 'new first name' }
  When method put

  Then status 200
  And match response ==
  """
    { id: '#(createdUserData.user.id)',
      studentId: 'new_student_id',
      firstName: 'new first name',
      email: '#(createdUserData.user.email)' }
  """

  Given path 'api', 'account', createdUserData.user.id
  When method delete
  Then status 204

Scenario: studentId parameter is required
  * def createUserParams =
  """
  {
      email: '#(randomEmail())',
      studentId: 'old_student_id',
      firstName: 'old_first_name',
  }
  """
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }

  Given path 'api', 'account', createdUserData.user.id
  Given request { studentId: '', firstName: 'some first name' }
  When method put
  Then status 400

  Given path 'api', 'account', createdUserData.user.id
  When method delete
  Then status 204
Scenario: firstname parameter is required
  * def createUserParams =
  """
  {
      email: '#(randomEmail())',
      studentId: 'old_student_id',
      firstName: 'old_first_name',
  }
  """
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }

  Given path 'api', 'account', createdUserData.user.id
  Given request { studentId: 'some_student_id', firstName: '' }
  When method put
  Then status 400

  Given path 'api', 'account', createdUserData.user.id
  When method delete
  Then status 204

Scenario: can't set already exists student id
  * def createUserParams =
  """
  {
      email: '#(randomEmail())',
      studentId: 'old_student_id',
      firstName: 'old_first_name',
  }
  """
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }

  Given path 'api', 'account', createdUserData.user.id
  Given request { studentId: '#(plainUser.studentId)', firstName: 'new first name' }
  When method put
  Then status 400

  Given path 'api', 'account', createdUserData.user.id
  When method delete
  Then status 204

Scenario: plain user can't change info
  * configure headers = { Authorization: '#("Bearer " + plainUser.token)' }
  Given path 'api', 'account', plainUser.id
  Given request { studentId: 'new_student_id', firstName: 'new first name' }
  When method put
  Then status 403