Feature: change default user info

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

Scenario: edit created user by admin
  * def createUserParams = getRandomUser()
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }
  Given path 'api', 'account', createdUserData.user.id
  * def newStudentId = 'new' + createdUserData.user.studentId
  Given request { studentId: '#(newStudentId)', firstName: 'new first name' }
  When method put

  Then status 200
  And match response ==
  """
    { id: '#(createdUserData.user.id)',
      studentId: '#(newStudentId)',
      firstName: 'new first name',
      email: '#(createdUserData.user.email)' }
  """

  Given path 'api', 'account', createdUserData.user.id
  When method delete
  Then status 204

Scenario: studentId parameter is required
  * def createUserParams = getRandomUser()
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
  * def createUserParams = getRandomUser()
  * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
  * configure headers = { Authorization: '#("Bearer " + admin.token)' }

  Given path 'api', 'account', createdUserData.user.id
  * def newStudentId = 'new' + createdUserData.user.studentId
  Given request { studentId: '#(newStudentId)', firstName: '' }
  When method put
  Then status 400

  Given path 'api', 'account', createdUserData.user.id
  When method delete
  Then status 204

Scenario: can't set already exists student id
  * def createUserParams = getRandomUser()
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