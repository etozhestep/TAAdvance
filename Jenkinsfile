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
        PATH = "${env.PATH}:/root/.dotnet/tools"
        RP_CREDS = 'report-portal-token'
        TEST_RESULT_FILE = "${WORKSPACE}/TAF/TestResults/test_results.trx"
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
                    withCredentials([string(credentialsId: 'report-portal-token', variable: 'RP_UUID')]){
                        catchError(buildResult: 'UNSTABLE', stageResult: 'UNSTABLE')    {
                            sh '''
                                export RP_SERVER_URL="http://reportportal-api-1:8585/api/v1"
                                export RP_PROJECT="${REPORTPORTAL_PROJECT}"
                                export RP_UUID="${RP_UUID}"
                                dotnet test "${PROJECT_PATH}" \
                                   --logger "trx;LogFileName=${TEST_RESULT_FILE}"
                            '''
                        }
                    }
                    
                }
            }
            post {
                always {
                    xunit(
                        tools: [
                            MSTest(
                                pattern: '${TEST_RESULT_FILE}',
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
                    def trxContent = readFile('${TEST_RESULT_FILE}')
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