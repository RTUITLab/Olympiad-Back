@ignore
Feature: create user
Background:
  * def email = karate.get('email', 'default_email@mail.com')
  * def studentId = karate.get('studentId', 'default_student_id')
  * def firstName = karate.get('firstName', 'default_first_name')

Scenario: createUser
  Given url baseUrl
  Given path 'api', 'account'
  Given request
  """
    { email: '#(email)',
      password: 'tempPassword',
      firstName: '#(firstName)',
      lastName: 'Test lastname',
      studentId: '#(studentId)',
      recaptchaToken: 'test recaptcha token'
    }
  """

  And method post
  Then status 200
  And match response == { id: '#uuid', studentId: '#(studentId)', firstName: '#(firstName)', email: '#(email)' }
  * def user = response
