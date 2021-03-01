Feature: challenges feature



Background:
  * url baseUrl
  * configure headers = { Authorization: #(accessToken) }
  * def challengeTime =
  """
    function(s) {
      var now = new Date();
      var start = new Date(now.getTime());
      var end = new Date(now.getTime());
      start.setHours(start.getHours() + 2);
      end.setHours(end.getHours() + 4);
      return { startTime: start.toISOString(), endTime: end.toISOString() };
    }
  """
  * def parseTimeSpan =
  """
    function (timeSpan) {
      var regex = new RegExp("(\\d*).?(\\d\\d):(\\d\\d):(\\d\\d).?(\\d*)");
      var match = timeSpan.match(regex);
      return {
        d: parseInt(match[1]),
        h: parseInt(match[2]),
        m: parseInt(match[3]),
        s: parseInt(match[4]),
        ms: parseInt(match[5])
      };
    }
  """


  Scenario: create challenge and get it
  Given path 'api/challenges'
  And request { "name": "karate challenge test", "startTime": null, "endTime": null, "challengeAccessType": 0 }
  When method post
  Then status 200

  * def createdSolutionId = response.id
  Given path 'api/challenges/', createdSolutionId
  When method get
  Then status 200
  Then match response contains {id: #(createdSolutionId)}

Scenario: create challenge and get toStart and toEnd
  Given path 'api/challenges'
  * def time = challengeTime()
  And request { "name": "for v2 test", "startTime": #(time.startTime), "endTime": #(time.endTime), "challengeAccessType": 0 }
  When method post
  Then status 200


  * def createdSolutionId = response.id
  Given path 'api/challenges/', createdSolutionId
  When method get
  Then status 200
  Then match response contains {id: #(createdSolutionId)}
  * def toStart = parseTimeSpan(response.toStart)
  * print toStart
  * def toEnd = parseTimeSpan(response.toEnd)
  * print toEnd
  Then assert toStart.h == 1.0
  Then assert toEnd.h == 3.0

Scenario: check exercise
  * def uuid =
  """
      function() { return '' + java.util.UUID.randomUUID(); }
  """
  Given path 'api/executor/checklog', uuid()
  And request { "exampleIn": "1 2", "exampleOut": "3", "programOut": "3", "programErr": null, "duration": "0:0:1"}
  When method post
