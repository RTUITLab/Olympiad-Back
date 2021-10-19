Feature: account create and delete

Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }

Scenario: create and delete user
  * def createdUser = call read('classpath:olympiad/account/createUser.feature') { email: test@mail.com }
  * def userId = createdUser.user.id

  Given path 'api', 'account', userId
  And method get
  Then status 200
  And match response contains { id: '#(userId)'}
  * print 'user can be found'

  Given path 'api', 'account'
  And request
  """
    { email: 'test@mail.com',
      password: 'tempPassword',
      firstName: 'Test account',
      lastName: 'Test lastname',
      studentId: 'test studentId',
      recaptchaToken: 'test recaptcha token'
    }
  """
  And method post
  Then status 400
  * print 'username already exists'

  Given path 'api', 'account', userId
  And method delete
  Then status 204

  Given path 'api', 'account', userId
  And method get
  Then status 404
  * print 'user can not be found'

