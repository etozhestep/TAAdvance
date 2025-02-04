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
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Clean') {
            steps {
               sh 'dotnet clean'
            }
        }

        stage('Restore') {
            steps {
               sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build ${SOLUTION_PATH}'
            }
        }

        stage('Test') {
            steps {
                catchError(buildResult: 'UNSTABLE', stageResult: 'UNSTABLE') {
                    sh 'dotnet test ${PROJECT_PATH} --logger "trx;LogFileName=test_results.trx"'
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
                endpoint: 'http://localhost:9090/',
                tokenCredentialsId: 'report-portal-token',
                launchName: "TAAdvance Build ${env.BUILD_NUMBER}",
                logPattern: '**/*.log',
                tags: ['CI', 'TAAdvance']
            )
            cleanWs()
        }
    }
}