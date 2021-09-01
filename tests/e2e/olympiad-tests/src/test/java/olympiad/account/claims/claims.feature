Feature: Work with claims

Background:
  * url baseUrl
  * configure headers = { Authorization: '#("Bearer " + admin.token)'}


Scenario: Get claims for new user
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  Given path 'api', 'account', createdUser.id, 'claims'
  When method get
  Then status 200
  And match response == '#array'

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }


Scenario: Create claim for user
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim', value: 'hello, test'  }
  Given request targetClaim
  When method post
  Then status 200
  And match response contains targetClaim

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }

Scenario: Can't create claims with whitespaces
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: ' test_claim', value: 'hello, test'  }
  Given request targetClaim
  When method post
  Then status 400

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim ', value: 'hello, test'  }
  Given request targetClaim
  When method post
  Then status 400

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim', value: ' hello, test'  }
  Given request targetClaim
  When method post
  Then status 400

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim', value: 'hello, test '  }
  Given request targetClaim
  When method post
  Then status 400

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }



Scenario: Create same claims for user
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim', value: 'hello, test'  }
  Given request targetClaim
  When method post
  Then status 200
  And match response contains targetClaim

  Given path 'api', 'account', createdUser.id, 'claims'
  Given request targetClaim
  When method post
  Then status 200

  Given path 'api', 'account', createdUser.id, 'claims'
  Given request {type: 'test_claim', 'value': 'another'}
  When method post
  Then status 200
# TODO: can't use & for filtering, create Issue
  * def targetClaimsByType = karate.jsonPath(response, "$.[?(@.type=='" + targetClaim.type + "')]")
  * def targetClaims = karate.jsonPath(targetClaimsByType, "$.[?(@.value=='" + targetClaim.value + "')]")

  And match targetClaims == '#[2]'


  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }

Scenario: Delete claim
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim', value: 'hello, test'  }
  Given request targetClaim
  When method post
  Then status 200
  And match response contains targetClaim

  Given path 'api', 'account', createdUser.id, 'claims'
  Given param type = targetClaim.type
  Given param value = targetClaim.value
  When method delete
  Then status 200
  And match response !contains targetClaim

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }

Scenario: Can try to delete any claim
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  * def targetClaim = { type: 'not_test_claim', value: 'hello, test'  }

  Given path 'api', 'account', createdUser.id, 'claims'
  Given param type = targetClaim.type
  Given param value = targetClaim.value
  When method delete
  Then status 200

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }


  Scenario: Delete claim removes all duplicates
  * def createdUserData = call read('classpath:olympiad/staff/createTempUser.feature')
  * def createdUser = createdUserData.user

  Given path 'api', 'account', createdUser.id, 'claims'
  * def targetClaim = { type: 'test_claim', value: 'hello, test'  }
  Given request targetClaim
  When method post
  Then status 200
  And match response contains targetClaim

  Given path 'api', 'account', createdUser.id, 'claims'
  Given request targetClaim
  When method post
  Then status 200
  And match response contains targetClaim

  And assert response.length >= 2

  Given path 'api', 'account', createdUser.id, 'claims'
  Given param type = targetClaim.type
  Given param value = targetClaim.value
  When method delete
  Then status 200
  And match response !contains targetClaim

  * call read('classpath:olympiad/staff/deleteUser.feature') { userId: '#(createdUser.id)' }
