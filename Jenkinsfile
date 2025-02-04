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
        SLACK_CHANNEL = '#ci-cd'
        JIRA_SITE = 'JiraCloud'
        JIRA_PROJECT_KEY = 'TA'
        SONAR_HOST_URL = 'http://my-sonarqube:9000'
        REPORT_PORTAL_URL = 'https://http://172.23.0.15:9090/api/v1'
        PATH = "${env.PATH}:/root/.dotnet/tools"
    }

    stages {
        stage('Setup Webhook') {
            steps {
                script {
                    hook = registerWebhook() 
                    env.ENCODED_URL = sh(
                        script: "echo -n ${hook.getURL()} | base64 -w 0", 
                        returnStdout: true
                    ).trim()
                }
            }
        }
            
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
                        sh """
                            dotnet test '${PROJECT_PATH}' \
                                --logger \"trx;LogFileName=./TestResults/test_results.trx\" \
                                /p:RP.APIBaseUrl=\"${REPORT_PORTAL_URL}\" \
                                /p:RP.UUID=\"${RP_API_TOKEN}\" \
                                /p:RP.LaunchName=\"TAAdvance_Build_${env.BUILD_NUMBER}\" \
                                /p:RP.attributes=\"k1:v1;k2:v2;rp.webhook.key:${env.ENCODED_URL}\"
                        """
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

        stage('Wait for Webhook') {
            steps {
                script {
                    timeout(time: params.TIMEOUT_TIME as Integer, unit: params.TIMEOUT_UNIT) {
                        echo 'Waiting for RP processing...'
                        def data = waitForWebhook hook
                        echo "Processing finished... ${data}"
                        
                        def jsonData = readJSON text: data
                        if (jsonData['status'] != 'PASSED') {
                            error("Quality Gate failed: ${jsonData['status']}")
                        }
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