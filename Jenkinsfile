pipeline {
    agent any
    parameters {
        string(name: 'TIMEOUT_TIME', defaultValue: '30', description: 'Timeout duration for waiting webhook data')
        string(name: 'TIMEOUT_UNIT', defaultValue: 'SECONDS', description: 'Time unit for the timeout')
    }
    triggers {
        pollSCM('* * * * *')
        cron('0 0 * * *')
    }

    environment {
        SOLUTION_PATH = "${WORKSPACE}/TAAdvance.sln"
        PROJECT_PATH = "${WORKSPACE}/TAF/TAF.csproj"
        RP_CONFIG_PATH = "${WORKSPACE}/TAF/reportportal.config.json"
        SLACK_CHANNEL = '#ci-cd'
        JIRA_SITE = 'JiraCloud'
        JIRA_PROJECT_KEY = 'TA'
        REPORTPORTAL_PROJECT = 'default_personal'
        SONAR_HOST_URL = 'http://my-sonarqube:9000'
        REPORT_PORTAL_URL = 'http://172.23.0.15:9090'
        RP_ATTRIBUTES = 'k1%3Av1%3Bk2%3Av2%3Brp.webhook.key%3A'
        PATH = "${env.PATH}:/root/.dotnet/tools"
        RP_CREDS = 'report-portal-token'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('SonarQube Analysis') {
            steps {
                withCredentials([string(credentialsId: 'sonarqube-token', variable: 'SONAR_LOGIN')]) {
                    withEnv(["PATH=${env.PATH}:/var/jenkins_home/.dotnet/tools"]) {
                        sh '''
                            dotnet sonarscanner begin /k:"TAAdvance" /d:sonar.host.url="${SONAR_HOST_URL}" /d:sonar.login="${SONAR_LOGIN}"
                        '''
                        sh "dotnet build '${SOLUTION_PATH}'"
                        sh '''
                            dotnet sonarscanner end /d:sonar.login="${SONAR_LOGIN}"
                        '''
                    }
                }
            }
        }

        stage('Test') {
            environment {
                RP_API_TOKEN = credentials('report-portal-token')
            }
            steps {
                script {
                    catchError(buildResult: 'UNSTABLE', stageResult: 'UNSTABLE') {
                        sh label: 'Run Tests with RP', script: """
                    dotnet test '${PROJECT_PATH}' \
                        --logger "trx;LogFileName=./TestResults/test_results.trx" \
                        /p:RP.APIBaseUrl="${REPORT_PORTAL_URL}" \
                        /p:RP.UUID="${RP_API_TOKEN}" \
                        /p:RP.LaunchName="TAAdvance_Build_${env.BUILD_NUMBER}" \
                        /p:RP.attributes='k1%3Av1%3Bk2%3Av2%3Brp.webhook.key%3A${env.ENCODED_URL}'
                """
                    }
                }
            }
            post {
                always {
                    xunit(
                        tools: [MSTest(pattern: '**/TestResults/test_results.trx')]
                    )
                }
            }
        }
        stage('Link RP to Jenkins') {
            steps {
                script {
                    withCredentials([string(credentialsId: env.RP_CREDS, variable: 'token')]) {
                        def response = httpRequest(
                            url: "${env.REPORTPORTAL_URL}/api/v1/${env.REPORTPORTAL_PROJECT}/launch?page.page=1&page.size=1&page.sort=startTime,desc",
                            customHeaders: [
                                [
                                    name: 'Authorization',
                                    value: "Bearer ${token}",
                                    maskValue: true
                                ]
                            ]
                        )
                        def jsonResponse = readJSON text: response.content
                        def latestLaunchId = jsonResponse.content[0].id

                        def link = "<a href=\"${env.REPORTPORTAL_URL}/ui/#${env.REPORTPORTAL_PROJECT}/launches/all/${latestLaunchId}\">go to launch #${latestLaunchId}</a>"
                        currentBuild.description = link
                    }
                }
            }
        }

        stage('Update Jira') {
            steps {
                script {
                    def trxContent = readFile('TestResults/test_results.trx')
                    def parsedXml = new XmlSlurper().parseText(trxContent)

                    def results = parsedXml.'Results'.'UnitTestResult'
                    def passed = results.findAll { it.@outcome == 'Passed' }.size()
                    def failed = results.findAll { it.@outcome == 'Failed' }.size()

                    jiraAddComment(
                        site: env.JIRA_SITE,
                        issueKey: "${env.JIRA_PROJECT_KEY}-${env.BUILD_NUMBER}",
                        comment: "Test results: passed: ${passed}, failed: ${failed}"
                    )
                }
            }
        }
    }
}