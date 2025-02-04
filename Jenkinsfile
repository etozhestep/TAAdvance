pipeline {
    agent any

    triggers {
        pollSCM('* * * * *') 
        cron('0 0 * * *')  
    }

    environment {
        SOLUTION_PATH = "${WORKSPACE}/TAAdvance.sln"
        PROJECT_PATH = "${WORKSPACE}/TAF/TAF.csproj"
        SLACK_CHANNEL = '#ci-cd'
        JIRA_SITE = 'JiraCloud'
        JIRA_PROJECT_KEY = 'TA'
        SONAR_HOST_URL = 'http://my-sonarqube:9000'
        PATH = "${env.PATH}:/root/.dotnet/tools"
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
                withCredentials([string(credentialsId: 'report-portal-token', variable: 'RP_UUID')]) {
                    catchError(buildResult: 'UNSTABLE', stageResult: 'UNSTABLE') {
                        sh 'dotnet test ${PROJECT_PATH} --logger "trx;LogFileName=test_results.trx"'
                    }
                }
            }
            post {
                always {
                    xunit(
                        tools: [
                            MSTest(
                                pattern: '**/test_results.trx',
                                skipNoTestFiles: false,
                                failIfNotNew: false,
                                deleteOutputFiles: true,
                                stopProcessingIfError: false
                            )
                        ]
                    )
                }
            }
        }

        stage('Update Jira') {
            steps {
                script {
                    // Adjust the path to your .trx file
                    def trxContent = readFile('test_results.trx')
                    def parsedXml = new XmlSlurper().parseText(trxContent)

                    // Parse the .trx file
                    def results = parsedXml.'Results'.'UnitTestResult'
                    def passed = results.findAll { it.@outcome == 'Passed' }.size()
                    def failed = results.findAll { it.@outcome == 'Failed' }.size()

                    // Add a comment to Jira
                    jiraAddComment(
                        site: env.JIRA_SITE,
                        issueKey: "${env.JIRA_PROJECT_KEY}-${env.BUILD_NUMBER}",
                        comment: "Test results: passed: ${passed}, failed: ${failed}"
                    )
                }
            }
        }
    }

    post {
        always {
            reportPortalPublisher(
                endpoint: 'http://reportportal:9090/',
                tokenCredentialsId: 'report-portal-token',
                launchName: "TAAdvance Build ${env.BUILD_NUMBER}",
                logPattern: '**/*.log',
                tags: ['CI', 'TAAdvance']
            )
            cleanWs()
        }
    }
}