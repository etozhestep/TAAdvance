pipeline {
    agent any
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
            steps {
                script {
                    withCredentials([string(credentialsId: 'reportportal-api-token', variable: 'RP_UUID')]) {
                        sh '''
                             export RP_SERVER_URL="http://reportportal-ui-1:8080/api/v1"
                             export RP_PROJECT="${env.REPORTPORTAL_PROJECT}"
                             export RP_UUID="${env.RP_TOKEN}"
                             dotnet test '${PROJECT_PATH}' \
                                    --logger "trx;LogFileName=./TestResults/test_results.trx"
                           '''
                    }
                }
            }
            post {
                always {
                    xunit(
                        tools: [
                            MSTest(
                                pattern: '**/TestResults/test_results.trx',
                                skipNoTestFiles: false,
                                failIfNotNew: false,     
                                deleteOutputFiles: true, 
                                stopProcessingIfError: true
                            )
                        ]
                    )
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