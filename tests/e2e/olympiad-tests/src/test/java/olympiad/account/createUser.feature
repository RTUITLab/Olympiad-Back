@ignore
Feature: create user
Background:
  * def email = karate.get('email', 'localdevelop@mail.com')

Scenario: createUser
  Given url baseUrl
  Given path 'api', 'account'
  Given request
  """
    { email: '#(email)',
      password: 'tempPassword',
      firstName: 'Test account',
      lastName: 'Test lastname',
      studentId: 'test studentId',
      recaptchaToken: 'test recaptcha token'
    }
  """

  And method post
  Then status 200
  * def user = response
