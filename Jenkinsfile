pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock'
        }
    }
    
    triggers {
        pollSCM('* * * * *')  // Автоматический триггер при коммитах в master
        cron('0 0 * * *')     // Ежедневная сборка в 00:00
    }
    
    environment {
        SOLUTION_PATH = "${WORKSPACE}/TAAdvance.sln"
        SLACK_CHANNEL = '#ci-cd'
        JIRA_PROJECT_KEY = 'TA'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/master']],
                    extensions: [
                        [$class: 'CloneOption', depth: 0, noTags: false, shallow: false],
                        [$class: 'CleanBeforeCheckout']
                    ],
                    userRemoteConfigs: [[
                        url: 'git@github.com:your-account/your-repo.git',
                        credentialsId: 'jenkins-github-ssh'
                    ]]
                ])
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build ${SOLUTION_PATH} --configuration Release'
            }
        }
        
        stage('Unit Tests') {
            steps {
                sh '''
                dotnet test ${SOLUTION_PATH} \
                    --configuration Release \
                    --logger "trx;LogFileName=unit-tests.trx" \
                    --results-directory TestResults \
                    --filter "Category=Unit"
                '''
            }
            post {
                always {
                    junit 'TestResults/**/*.trx'
                    archiveArtifacts artifacts: 'TestResults/**/*', allowEmptyArchive: true
                }
            }
        }
        
        stage('UI Tests') {
            steps {
                sh '''
                dotnet test ${SOLUTION_PATH} \
                    --configuration Release \
                    --logger "trx;LogFileName=ui-tests.trx" \
                    --results-directory TestResults \
                    --filter "Category=UI"
                '''
            }
            post {
                always {
                    junit 'TestResults/**/*.trx'
                    script {
                        // Интеграция с Jira
                        def testResults = readJSON text: sh(script: 'cat TestResults/*.trx', returnStdout: true)
                        jiraUpdateIssue site: 'JIRA_SITE',
                            issueKey: "${JIRA_PROJECT_KEY}-${env.BUILD_NUMBER}",
                            testResults: testResults
                    }
                }
            }
        }
        
        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv('sonarqube-server') {
                    sh """
                    dotnet sonarscanner begin /k:"TAAdvance" \
                        /d:sonar.cs.vstest.reportsPaths="TestResults/*.trx" \
                        /d:sonar.exclusions="**/bin/**/*,**/obj/**/*"
                    dotnet build ${SOLUTION_PATH}
                    dotnet sonarscanner end
                    """
                }
            }
        }
    }
    
    post {
        always {
            // Отправка в Report Portal
            reportPortalPublisher(
                endpoint: 'https://report-portal.example',
                tokenCredentialsId: 'report-portal-token',
                launchName: 'TAAdvance Build ${env.BUILD_NUMBER}',
                logPattern: '**/*.log',
                tags: ['CI', 'TAAdvance'],
                description: 'Automated build and test run'
            )
            
            // Очистка workspace
            cleanWs()
        }
        
        success {
            slackSend channel: "${SLACK_CHANNEL}",
                color: 'good',
                message: """✅ Build SUCCESS
Project: ${env.JOB_NAME}
Build: ${env.BUILD_NUMBER}
Report: ${env.BUILD_URL}testReport"""
        }
        
        failure {
            slackSend channel: "${SLACK_CHANNEL}",
                color: 'danger',
                message: """❌ Build FAILED
Project: ${env.JOB_NAME}
Build: ${env.BUILD_NUMBER}
Logs: ${env.BUILD_URL}console"""
        }
        
        changed {
            slackSend channel: "${SLACK_CHANNEL}",
                color: 'warning',
                message: """⚠️ Build STATUS CHANGED
Previous: ${currentBuild.previousBuild.result}
Current: ${currentBuild.result}"""
        }
    }
}
