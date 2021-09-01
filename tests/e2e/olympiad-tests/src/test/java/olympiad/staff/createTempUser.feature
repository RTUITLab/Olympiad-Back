@ignore
Feature: create temp user

  Background:
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

Scenario: create temp user
    * def createUserParams = getRandomUser()
    * def createdUserData = call read('classpath:olympiad/account/createUser.feature') createUserParams
    * def user = createdUserData.user
